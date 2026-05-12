using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace SubakGame.UI
{
    public class PauseUI : MonoBehaviour
    {
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button quitButton;

        private bool isPaused;

        private void Awake()
        {
            if (pausePanel != null)
                pausePanel.SetActive(false);

            if (pauseButton != null)
                pauseButton.onClick.AddListener(TogglePause);

            if (resumeButton != null)
                resumeButton.onClick.AddListener(Resume);

            if (restartButton != null)
                restartButton.onClick.AddListener(RestartGame);

            if (quitButton != null)
                quitButton.onClick.AddListener(QuitGame);
        }

        public void TogglePause()
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0f : 1f;
            
            if (pausePanel != null)
                pausePanel.SetActive(isPaused);

            if (pauseButton != null)
                pauseButton.gameObject.SetActive(!isPaused);
        }

        public void Resume()
        {
            isPaused = false;
            Time.timeScale = 1f;
            if (pausePanel != null)
                pausePanel.SetActive(false);

            if (pauseButton != null)
                pauseButton.gameObject.SetActive(true);
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
