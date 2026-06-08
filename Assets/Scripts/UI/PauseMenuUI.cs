using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        SetPaused(false);
    }

    private void OnEnable()  { PauseManager.OnPauseChanged += OnPauseChanged; }
    private void OnDisable() { PauseManager.OnPauseChanged -= OnPauseChanged; }

    private void Update()
    {
        if (PlayerManager.Instance.Current != null && PlayerManager.Instance.Current.Input.PausePressed)
            PauseManager.Instance.Toggle();
    }

    private void OnPauseChanged(bool isPaused) => SetPaused(isPaused);

    private void SetPaused(bool isPaused)
    {
        panel.SetActive(isPaused);
        if (_canvasGroup != null)
        {
            _canvasGroup.blocksRaycasts = isPaused;
            _canvasGroup.interactable = isPaused;
        }
    }

    public void OnResumeClick()      { PauseManager.Instance.Resume(); }
    public void OnQuitToTitleClick() { GameInitializer.Instance.QuitToTitle(); }
}
