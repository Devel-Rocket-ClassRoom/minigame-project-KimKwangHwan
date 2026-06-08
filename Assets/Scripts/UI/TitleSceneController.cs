using System.Collections;
using UnityEngine;

public class TitleSceneController : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "MainScene";
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private SaveSlotUI saveSlotUI;
    [SerializeField] private GameObject continueButton;

    private bool _isTransitioning;

    private void Start()
    {
        continueButton.SetActive(SaveManager.Instance.HasAnySave());
        StartCoroutine(FadeController.Instance.FadeIn(fadeInDuration));
    }

    public void OnNewGameClick()
    {
        if (_isTransitioning) return;
        saveSlotUI.Open(SaveSlotUI.Mode.NewGame, OnSlotSelected);
    }

    public void OnContinueClick()
    {
        if (_isTransitioning) return;
        saveSlotUI.Open(SaveSlotUI.Mode.Continue, OnSlotSelected);
    }

    public void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnSlotSelected(int slot)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;
        SaveManager.Instance.SetActiveSlot(slot);
        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        yield return FadeController.Instance.FadeOut(fadeOutDuration);
        GameInitializer.Instance.LoadGameScene(gameSceneName);
    }
}
