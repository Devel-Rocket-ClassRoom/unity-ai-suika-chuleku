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

        public void Bind(FruitData fruitData)
        {
            data = fruitData;
        }
    }
}
