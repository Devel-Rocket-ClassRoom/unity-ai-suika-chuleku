using System.Collections.Generic;
using UnityEngine;

namespace SubakGame.Gameplay
{
    [CreateAssetMenu(fileName = "FruitDatabase", menuName = "SubakGame/Fruit Database")]
    public class FruitDatabase : ScriptableObject
    {
        [SerializeField] private List<FruitData> fruits = new();

        public IReadOnlyList<FruitData> All => fruits;

        public FruitData GetByTier(int tier)
        {
            foreach (var f in fruits)
            {
                if (f != null && f.tier == tier) return f;
            }
            return null;
        }

        public FruitData GetRandomDroppable()
        {
            var pool = new List<FruitData>(5);
            foreach (var f in fruits)
            {
                if (f != null && f.IsDroppable) pool.Add(f);
            }
            if (pool.Count == 0) return null;
            return pool[Random.Range(0, pool.Count)];
        }
    }
}
