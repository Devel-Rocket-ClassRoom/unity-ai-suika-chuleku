using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SubakGame.Gameplay;

namespace SubakGame.UI
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private GameObject rootPanel;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button quitButton;

        private void Awake()
        {
            if (rootPanel != null)
                rootPanel.SetActive(false);

            if (restartButton != null)
                restartButton.onClick.AddListener(RestartGame);

            if (quitButton != null)
                quitButton.onClick.AddListener(QuitGame);
        }

        private void OnEnable()
        {
            DeadlineTrigger.GameOver += Show;
        }

        private void OnDisable()
        {
            DeadlineTrigger.GameOver -= Show;
        }

        public void Show()
        {
            if (rootPanel != null)
                rootPanel.SetActive(true);
            
            // Hide Pause button if present
            var pauseUI = Object.FindAnyObjectByType<PauseUI>();
            if (pauseUI != null)
            {
                pauseUI.gameObject.SetActive(false);
            }
        }

        private void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void QuitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}
