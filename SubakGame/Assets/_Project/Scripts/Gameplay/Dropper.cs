using UnityEngine;
using UnityEngine.InputSystem;

namespace SubakGame.Gameplay
{
    // 박스 상단의 다음 과일을 좌우로 움직이고, 입력 시 낙하시키는 컨트롤러.
    // PC: 마우스 좌우(호버)로 위치, 좌클릭/스페이스로 드롭
    // 모바일: 터치 드래그로 위치, 터치 업 시 드롭
    public class Dropper : MonoBehaviour
    {
        [Header("참조")]
        [SerializeField] private BoxContainer box;
        [SerializeField] private FruitDatabase database;
        [SerializeField] private Camera viewCamera;

        [Header("입력")]
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private string actionMapName = "Suika";
        [SerializeField] private string pointerActionName = "PointerPosition";
        [SerializeField] private string dropActionName = "Drop";

        [Header("동작")]
        [SerializeField, Min(0f)] private float cooldown = 0.5f;
        [SerializeField, Range(0f, 1f)] private float previewAlpha = 0.7f;
        [SerializeField, Range(0f, 1f), Tooltip("게임오버 시 적용할 시간 스케일 (0=정지)")]
        private float gameOverTimeScale = 0.1f;

        private InputAction pointerAction;
        private InputAction dropAction;

        private FruitData currentFruit;
        private FruitData nextFruit;
        private GameObject previewGO;
        private SpriteRenderer previewSR;
        private float unlockAt;
        private bool isLocked;
        private bool isGameOver;

        public static event System.Action<FruitData> NextFruitChanged;

        private void Awake()
        {
            if (viewCamera == null) viewCamera = Camera.main;
        }

        private void OnEnable()
        {
            // 게임오버 직후 종료된 이전 세션이 timeScale 을 남길 수 있으므로 리셋
            Time.timeScale = 1f;
            isGameOver = false;

            if (inputActions == null)
            {
                Debug.LogError("[Dropper] InputActionAsset 미설정");
                return;
            }
            var map = inputActions.FindActionMap(actionMapName, throwIfNotFound: true);
            pointerAction = map.FindAction(pointerActionName, throwIfNotFound: true);
            dropAction = map.FindAction(dropActionName, throwIfNotFound: true);
            map.Enable();
            dropAction.performed += OnDropPerformed;
            DeadlineTrigger.GameOver += HandleGameOver;
        }

        private void OnDisable()
        {
            if (dropAction != null) dropAction.performed -= OnDropPerformed;
            inputActions?.FindActionMap(actionMapName)?.Disable();
            DeadlineTrigger.GameOver -= HandleGameOver;
        }

        private void HandleGameOver()
        {
            isGameOver = true;
            if (previewGO != null) previewGO.SetActive(false);
            Time.timeScale = gameOverTimeScale;
        }

        private void Start()
        {
            // Initial fill
            nextFruit = database != null ? database.GetRandomDroppable() : null;
            PickNextFruit();
        }

        private void Update()
        {
            if (isGameOver || Time.timeScale <= 0f) return;
            if (isLocked && Time.time >= unlockAt) isLocked = false;
            UpdatePreviewPosition();
        }

        private void UpdatePreviewPosition()
        {
            if (currentFruit == null || previewGO == null || box == null) return;

            Vector2 pointerScreen = pointerAction != null
                ? pointerAction.ReadValue<Vector2>()
                : (Vector2)Input.mousePosition;

            float zDepth = -viewCamera.transform.position.z;
            Vector3 world = viewCamera.ScreenToWorldPoint(new Vector3(pointerScreen.x, pointerScreen.y, zDepth));

            float halfRange = Mathf.Max(0f, box.HalfWidth - currentFruit.radius);
            float localX = Mathf.Clamp(world.x - box.transform.position.x, -halfRange, halfRange);

            previewGO.transform.position = new Vector3(
                box.transform.position.x + localX,
                box.transform.position.y + box.DropYLocal,
                0f);
        }

        private void OnDropPerformed(InputAction.CallbackContext ctx)
        {
            if (isGameOver || isLocked || currentFruit == null || previewGO == null || Time.timeScale <= 0f) return;
            if (currentFruit.prefab == null)
            {
                Debug.LogWarning($"[Dropper] {currentFruit.displayName} 의 prefab 미설정");
                return;
            }

            Vector3 spawnPos = previewGO.transform.position;
            var instance = Instantiate(currentFruit.prefab, spawnPos, Quaternion.identity);
            var fruit = instance.GetComponent<Fruit>();
            if (fruit != null) fruit.Bind(currentFruit);

            isLocked = true;
            unlockAt = Time.time + cooldown;

            PickNextFruit();
        }

        private void PickNextFruit()
        {
            currentFruit = nextFruit;
            nextFruit = database != null ? database.GetRandomDroppable() : null;
            
            NextFruitChanged?.Invoke(nextFruit);

            if (currentFruit == null)
            {
                if (previewGO != null) previewGO.SetActive(false);
                return;
            }

            if (previewGO == null)
            {
                previewGO = new GameObject("DropPreview");
                previewGO.transform.SetParent(transform, false);
                previewSR = previewGO.AddComponent<SpriteRenderer>();
                previewSR.sortingOrder = 100;
            }

            previewGO.SetActive(true);
            previewSR.sprite = currentFruit.sprite;
            var c = currentFruit.color;
            c.a = previewAlpha;
            previewSR.color = c;

            // 프리팹 비주얼 스케일과 동일하게 맞춤 (반지름 * 2 / 스프라이트 PPU 기반)
            // 단순화: 프리팹의 transform.localScale 을 그대로 사용
            previewGO.transform.localScale = currentFruit.prefab != null
                ? currentFruit.prefab.transform.localScale
                : Vector3.one;
        }
}
}
