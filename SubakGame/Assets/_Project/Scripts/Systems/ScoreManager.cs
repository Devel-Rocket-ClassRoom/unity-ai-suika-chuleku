using UnityEngine;
using SubakGame.Gameplay;

namespace SubakGame.Systems
{
    // 합치기 발생 시 결과 단계의 scoreValue 만큼 가산하고
    // 게임오버 시 베스트 점수를 PlayerPrefs 로 영구 저장.
    public class ScoreManager : MonoBehaviour
    {
        public const string BestPrefKey = "SubakGame.BestScore";

        [SerializeField] private FruitDatabase database;

        public static ScoreManager Instance { get; private set; }

        public int Score { get; private set; }
        public int Best { get; private set; }
        public bool IsNewRecord { get; private set; }

        public static event System.Action<int> ScoreChanged;
        public static event System.Action<int> BestChanged;
        public static event System.Action NewRecord;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            Best = PlayerPrefs.GetInt(BestPrefKey, 0);
        }

        private void OnEnable()
        {
            Score = 0;
            IsNewRecord = false;
            Fruit.Merged += HandleMerged;
            DeadlineTrigger.GameOver += HandleGameOver;
            ScoreChanged?.Invoke(Score);
            BestChanged?.Invoke(Best);
        }

        private void OnDisable()
        {
            Fruit.Merged -= HandleMerged;
            DeadlineTrigger.GameOver -= HandleGameOver;
            if (Instance == this) Instance = null;
        }

        private void HandleMerged(int newTier, Vector2 pos)
        {
            int delta = 0;
            if (database != null)
            {
                var data = database.GetByTier(newTier);
                if (data != null) delta = data.scoreValue;
            }
            if (delta <= 0) delta = newTier; // 폴백: 데이터 누락 시 단계 자체를 점수로
            Score += delta;
            ScoreChanged?.Invoke(Score);
        }

        private void HandleGameOver()
        {
            if (Score > Best)
            {
                Best = Score;
                IsNewRecord = true;
                PlayerPrefs.SetInt(BestPrefKey, Best);
                PlayerPrefs.Save();
                BestChanged?.Invoke(Best);
                NewRecord?.Invoke();
            }
        }

        public void ResetScore()
        {
            Score = 0;
            IsNewRecord = false;
            ScoreChanged?.Invoke(Score);
        }
    }
}
