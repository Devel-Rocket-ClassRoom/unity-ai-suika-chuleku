using UnityEngine;
using TMPro;
using SubakGame.Systems;

namespace SubakGame.UI
{
    // 우측 점수 패널의 텍스트를 ScoreManager 이벤트에 따라 갱신.
    public class ScoreHud : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text bestText;

        [Header("표시 형식")]
        [SerializeField] private string scoreLabel = "Score";
        [SerializeField] private string bestLabel = "Best";

        private void OnEnable()
        {
            ScoreManager.ScoreChanged += UpdateScore;
            ScoreManager.BestChanged += UpdateBest;
            ScoreManager.NewRecord += HandleNewRecord;

            if (ScoreManager.Instance != null)
            {
                UpdateScore(ScoreManager.Instance.Score);
                UpdateBest(ScoreManager.Instance.Best);
            }
            else
            {
                UpdateScore(0);
                UpdateBest(PlayerPrefs.GetInt(ScoreManager.BestPrefKey, 0));
            }
        }

        private void OnDisable()
        {
            ScoreManager.ScoreChanged -= UpdateScore;
            ScoreManager.BestChanged -= UpdateBest;
            ScoreManager.NewRecord -= HandleNewRecord;
        }

        private void UpdateScore(int v)
        {
            if (scoreText != null) scoreText.text = $"{scoreLabel}\n{v}";
        }

        private void UpdateBest(int v)
        {
            if (bestText != null) bestText.text = $"{bestLabel}\n{v}";
        }

        private void HandleNewRecord()
        {
            // 추후 신기록 연출(흔들림/색 변화)이 들어갈 자리
            if (bestText != null) bestText.color = new Color(1f, 0.85f, 0.2f, 1f);
        }
    }
}
