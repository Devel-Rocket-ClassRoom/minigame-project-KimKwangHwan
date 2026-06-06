using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private string titleSceneName = "TitleScene";

    private void Awake()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    private void OnEnable()
    {
        PauseManager.OnPauseChanged += OnPauseChanged;
    }

    private void OnDisable()
    {
        PauseManager.OnPauseChanged -= OnPauseChanged;
    }

    private void Update()
    {
        if (PlayerManager.Instance.Current != null && PlayerManager.Instance.Current.Input.PausePressed)
            PauseManager.Instance.Toggle();
    }

    private void OnPauseChanged(bool isPaused)
    {
        canvasGroup.alpha = isPaused ? 1f : 0f;
        canvasGroup.blocksRaycasts = isPaused;
        canvasGroup.interactable = isPaused;
    }

    public void OnResumeClick()
    {
        PauseManager.Instance.Resume();
    }

    public void OnQuitToTitleClick()
    {
        PauseManager.Instance.Resume();
        SceneManager.LoadScene(titleSceneName);
    }
}
