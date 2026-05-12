using UnityEngine;

namespace SubakGame.Gameplay
{
    // 좌표 규칙: 박스의 중심이 transform.position과 일치한다.
    // 박스 내부 영역 y = [-height/2, +height/2], x = [-width/2, +width/2]
    public class BoxContainer : MonoBehaviour
    {
        [Header("크기 (Unity unit)")]
        [SerializeField, Min(1f)] private float width = 5f;
        [SerializeField, Min(1f)] private float height = 7f;
        [SerializeField, Min(0.05f)] private float wallThickness = 0.4f;

        [Header("데드라인")]
        [SerializeField, Range(0.1f, 0.99f), Tooltip("박스 바닥 기준 데드라인 위치 비율 (0=바닥, 1=천장)")]
        private float deadlineRatio = 0.85f;

        [Header("자식 참조")]
        [SerializeField] private BoxCollider2D leftWall;
        [SerializeField] private BoxCollider2D rightWall;
        [SerializeField] private BoxCollider2D bottomWall;
        [SerializeField] private DeadlineTrigger deadline;

        public float Width => width;
        public float Height => height;
        public float HalfWidth => width * 0.5f;
        public float HalfHeight => height * 0.5f;
        public float DeadlineYLocal => (deadlineRatio - 0.5f) * height;
        public float DropYLocal => height * 0.5f + 0.5f;

        private void OnValidate() => UpdateLayout();
        private void Awake() => UpdateLayout();

        [ContextMenu("Layout 재계산")]
        public void UpdateLayout()
        {
            float halfW = width * 0.5f;
            float halfH = height * 0.5f;
            float halfT = wallThickness * 0.5f;

            if (leftWall != null)
            {
                leftWall.transform.localPosition = new Vector3(-(halfW + halfT), 0f, 0f);
                leftWall.size = new Vector2(wallThickness, height + wallThickness * 2f);
            }
            if (rightWall != null)
            {
                rightWall.transform.localPosition = new Vector3(halfW + halfT, 0f, 0f);
                rightWall.size = new Vector2(wallThickness, height + wallThickness * 2f);
            }
            if (bottomWall != null)
            {
                bottomWall.transform.localPosition = new Vector3(0f, -(halfH + halfT), 0f);
                bottomWall.size = new Vector2(width + wallThickness * 2f, wallThickness);
            }
            if (deadline != null)
            {
                deadline.transform.localPosition = new Vector3(0f, DeadlineYLocal, 0f);
                deadline.SetSize(new Vector2(width, 0.1f));
            }
        }

        private void OnDrawGizmos()
        {
            Vector3 origin = transform.position;
            Gizmos.color = new Color(0.4f, 0.9f, 0.4f, 0.8f);
            Gizmos.DrawWireCube(origin, new Vector3(width, height, 0f));

            Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.9f);
            float dyWorld = DeadlineYLocal;
            Vector3 a = origin + new Vector3(-width * 0.5f, dyWorld, 0f);
            Vector3 b = origin + new Vector3(width * 0.5f, dyWorld, 0f);
            Gizmos.DrawLine(a, b);
        }
    }
}
