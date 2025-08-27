using System;
using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup pauseMenuCanvasGroup;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [SerializeField] private float fadeDuration = 0.8f;

    private Coroutine fadeCoroutine;
    private EventSystem eventSystem;

    private void Reset()
    {
        pauseMenuCanvasGroup = GetComponent<CanvasGroup>();

        if (pausePanel == null)
        {
            pausePanel = transform.Find("PausePanel")?.gameObject ?? gameObject;
        }

        if (resumeButton == null)
        {
            resumeButton = resumeButton ?? transform.Find("Buttons/ResumeButton")?.GetComponent<Button>();
        }

        if (settingsButton == null)
        {
            settingsButton = settingsButton ?? transform.Find("Buttons/SettingsButton")?.GetComponent<Button>();
        }

        if (mainMenuButton == null)
        {
            mainMenuButton = mainMenuButton ?? transform.Find("Buttons/MainMenuButton")?.GetComponent<Button>();
        }

        if (quitButton == null)
        {
            quitButton = quitButton ?? transform.Find("Buttons/QuitButton")?.GetComponent<Button>();
        }
    }

    private void Awake()
    {
        if (pauseMenuCanvasGroup == null)
        {
            pauseMenuCanvasGroup = GetComponent<CanvasGroup>();
        }

        eventSystem = EventSystem.current;

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

        pauseMenuCanvasGroup.blocksRaycasts = true;
        pauseMenuCanvasGroup.interactable = true;

        if (eventSystem != null && resumeButton != null)
        {
            SetSelectedButton(resumeButton);
        }

        StartFade(0f, 1f, fadeDuration);
        Debug.Log("PauseUI: ShowPausePanel called.");
    }

    public void Hide()
    {
        StartFade(pauseMenuCanvasGroup.alpha, 0f, fadeDuration, onComplete: () =>
        {
            pauseMenuCanvasGroup.blocksRaycasts = false;
            pauseMenuCanvasGroup.interactable = false;
            pausePanel.SetActive(false);
            gameObject.SetActive(false);
        });
        Debug.Log("PauseUI: Hide called.");
    }

    public void HideImmediate()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        pauseMenuCanvasGroup.alpha = 0f;
        pauseMenuCanvasGroup.blocksRaycasts = false;
        pauseMenuCanvasGroup.interactable = false;
        pausePanel.SetActive(false);
        gameObject.SetActive(false);
    }

    private void StartFade(float from, float to, float duration, Action onComplete = null)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeRoutine(from, to, duration, onComplete));
    }

    private IEnumerator FadeRoutine(float from, float to, float duration, Action onComplete = null)
    {
        float elapsedTime = 0f;
        pauseMenuCanvasGroup.alpha = from;

        if (Mathf.Approximately(duration, 0f))
        {
            pauseMenuCanvasGroup.alpha = to;
            onComplete?.Invoke();
            yield break;
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            pauseMenuCanvasGroup.alpha = Mathf.Lerp(from, to, elapsedTime / duration);
            yield return null;
        }
        pauseMenuCanvasGroup.alpha = to;
        onComplete?.Invoke();
    }

    private void OnResumeButtonClicked()
    {
        Debug.Log("PauseUI: Resume button clicked.");

        GameStateMachine gameStateMachine = FindObjectOfType<GameStateMachine>();
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

    public void SetSelectedButton(Button button)
    {
        if (button == null)
        {
            return;
        }

        if (eventSystem == null)
        {
            eventSystem = EventSystem.current;
        }

        if (eventSystem == null)
        {
            Debug.LogWarning("PauseUI: EventSystem not found. Navigation will not work.");
            return;
        }

        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(button.gameObject);
    }

    public void OnCancel()
    {
        OnResumeButtonClicked();
    }
}
