using UnityEngine;

namespace SubakGame.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Fruit : MonoBehaviour
    {
        [SerializeField] private FruitData data;

        public FruitData Data => data;
        public int Tier => data != null ? data.tier : 0;
        public bool IsMerged { get; private set; }
        public float SpawnedAt { get; private set; }

        // 합치기 발생 시 발화: (생성된 새 단계, 월드 좌표)
        // 점수/SFX/파티클이 구독.
        public static event System.Action<int, Vector2> Merged;

        private void Awake()
        {
            SpawnedAt = Time.time;
        }

        public void Bind(FruitData fruitData)
        {
            data = fruitData;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (IsMerged || data == null) return;

            var other = collision.gameObject.GetComponent<Fruit>();
            if (other == null || other.IsMerged) return;
            if (other.Tier != Tier) return;

            // 양쪽이 동시에 OnCollisionEnter2D를 받는 것 방지:
            // InstanceID 가 작은 쪽만 합치기 실행을 담당한다.
            if (GetInstanceID() >= other.GetInstanceID()) return;

            IsMerged = true;
            other.IsMerged = true;

            Vector2 midpoint = ((Vector2)transform.position + (Vector2)other.transform.position) * 0.5f;
            FruitData nextData = data.nextTier;

            // SetActive(false) 로 즉시 물리/렌더에서 제외 → 새 과일과 잔재 충돌 방지
            gameObject.SetActive(false);
            other.gameObject.SetActive(false);
            Destroy(other.gameObject);
            Destroy(gameObject);

            if (nextData != null && nextData.prefab != null)
            {
                var spawn = Instantiate(nextData.prefab, midpoint, Quaternion.identity);
                var newFruit = spawn.GetComponent<Fruit>();
                if (newFruit != null) newFruit.Bind(nextData);
                Merged?.Invoke(nextData.tier, midpoint);
            }
            // nextData == null: 수박+수박 (최종 단계) → 특수 처리는 #15 참고
        }
    }
}
