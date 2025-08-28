using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private GameStateMachine gameStateMachine;
    [SerializeField] private CanvasGroup pauseMenuCanvasGroup;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Awake()
    {
        if (gameStateMachine == null)
        {
            gameStateMachine = GetComponent<GameStateMachine>();
        }

        if (pauseMenuCanvasGroup == null)
        {
            pauseMenuCanvasGroup = GetComponent<CanvasGroup>();
        }

        if (pausePanel == null)
        {
            pausePanel = this.gameObject;
        }

        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(OnResumeButtonClicked);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitButtonClicked);
        }

        HideImmediate();
    }

    public void ShowPausePanel()
    {
        gameObject.SetActive(true);
        pausePanel.SetActive(true);
        pauseMenuCanvasGroup.alpha = 1f;
        pauseMenuCanvasGroup.blocksRaycasts = true;
        pauseMenuCanvasGroup.interactable = true;
    }

    public void Hide()
    {
        pauseMenuCanvasGroup.alpha = 0f;
        pauseMenuCanvasGroup.blocksRaycasts = false;
        pauseMenuCanvasGroup.interactable = false;
        pausePanel.SetActive(false);
        gameObject.SetActive(false);
    }

    public void HideImmediate()
    {
        pauseMenuCanvasGroup.alpha = 0f;
        pauseMenuCanvasGroup.blocksRaycasts = false;
        pauseMenuCanvasGroup.interactable = false;
        pausePanel.SetActive(false);
        gameObject.SetActive(false);
    }

    private void OnResumeButtonClicked()
    {
        Debug.Log("PauseUI: Resume button clicked.");

        if (gameStateMachine != null)
        {
            gameStateMachine.ToGameplay();
        }
        else
        {
            Debug.LogWarning("PauseUI: GameStateMachine not found in the scene.");
        }
    }

    private void OnSettingsButtonClicked()
    {
        Debug.Log("PauseUI: Settings button clicked.");
    }

    private void OnMainMenuButtonClicked()
    {
        Debug.Log("PauseUI: Main Menu button clicked.");
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogWarning("PauseUI: Main menu scene name is not set.");
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
    }

    private void OnQuitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnCancel()
    {
        OnResumeButtonClicked();
    }
}
