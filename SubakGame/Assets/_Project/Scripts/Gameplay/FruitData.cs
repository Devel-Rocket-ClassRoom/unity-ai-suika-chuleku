using UnityEngine;

namespace SubakGame.Gameplay
{
    [CreateAssetMenu(fileName = "Fruit", menuName = "SubakGame/Fruit Data")]
    public class FruitData : ScriptableObject
    {
        [Header("기본")]
        [Tooltip("1=체리, 11=수박")]
        public int tier;
        public string displayName;

        [Header("점수")]
        [Tooltip("이 단계로 합쳐졌을 때 가산되는 점수 (GDD §3)")]
        public int scoreValue;

        [Header("물리")]
        [Tooltip("CircleCollider2D radius (Unity 단위)")]
        public float radius;
        [Tooltip("Rigidbody2D mass")]
        public float mass;

        [Header("비주얼")]
        [Tooltip("스프라이트 도착 전 임시 색상")]
        public Color color = Color.white;
        public Sprite sprite;

        [Header("프리팹")]
        [Tooltip("드롭/머지 시 생성할 Fruit 프리팹")]
        public GameObject prefab;

        [Header("진화")]
        [Tooltip("두 개가 합쳐졌을 때 생성될 다음 단계. null이면 최종(수박)")]
        public FruitData nextTier;

        public bool IsMaxTier => nextTier == null;
        public bool IsDroppable => tier >= 1 && tier <= 5;
    }
}
