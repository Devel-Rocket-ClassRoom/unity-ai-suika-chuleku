using UnityEngine;

namespace SubakGame.Gameplay
{
    public class EffectManager : MonoBehaviour
    {
        [SerializeField] private FruitDatabase database;
        [SerializeField] private GameObject mergeParticlePrefab;
        [SerializeField] private AudioClip mergeSound;

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        private void OnEnable()
        {
            Fruit.Merged += OnFruitMerged;
        }

        private void OnDisable()
        {
            Fruit.Merged -= OnFruitMerged;
        }

        private void OnFruitMerged(int tier, Vector2 position)
        {
            if (audioSource != null && mergeSound != null)
            {
                audioSource.PlayOneShot(mergeSound);
            }

            if (mergeParticlePrefab == null) return;

            GameObject obj = Instantiate(mergeParticlePrefab, position, Quaternion.identity);
ParticleSystem ps = obj.GetComponent<ParticleSystem>();
            
            if (ps != null && database != null)
            {
                FruitData data = database.GetByTier(tier);
                if (data != null)
                {
                    var main = ps.main;
                    main.startColor = data.color;
                    
                    // Scale particle size and shape radius based on fruit radius
                    float scaleFactor = data.radius * 2f;
                    main.startSize = new ParticleSystem.MinMaxCurve(0.1f * scaleFactor, 0.3f * scaleFactor);
                    
                    var shape = ps.shape;
                    shape.radius = data.radius;
                }
            }
        }
    }
}
