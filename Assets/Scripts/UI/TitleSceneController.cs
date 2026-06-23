using Cysharp.Threading.Tasks;
using UnityEngine;

public class TitleSceneController : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "MainScene";
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private SaveSlotUI saveSlotUI;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject titleContents;
    [SerializeField] private LoginUI loginUI;
    private bool _isTransitioning;

    private async UniTaskVoid Start()
    {
        titleContents.SetActive(false);

        await UniTask.WaitUntil(() => AuthManager.Instance.IsInitialized);
        AuthManager.Instance.LoginStatusChanged += OnLoginStatusChanged;

        await RefreshUIAsync(AuthManager.Instance.IsLoggedIn);
        FadeController.Instance.FadeIn(fadeInDuration).Forget();
    }

    private void OnDestroy()
    {
        if (AuthManager.Instance != null)
            AuthManager.Instance.LoginStatusChanged -= OnLoginStatusChanged;
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

    public void OnLogoutClick()
    {
        if (_isTransitioning) return;
        AuthManager.Instance.SignOut();
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
        StartGameRoutine().Forget();
    }

    private async UniTask StartGameRoutine()
    {
        await FadeController.Instance.FadeOut(fadeOutDuration);
        GameInitializer.Instance.LoadGameScene(gameSceneName);
    }
    private void OnLoginStatusChanged(bool isLoggedIn)
    {
        RefreshUIAsync(isLoggedIn).Forget();
    }
    private async UniTask RefreshUIAsync(bool isLoggedIn)
    {
        titleContents.SetActive(isLoggedIn);
        loginUI.gameObject.SetActive(!isLoggedIn);
        if (isLoggedIn)
        {
            bool hasSave = await FirebaseManager.Instance.HasAnySaveAsync();
            continueButton.SetActive(hasSave);
        }
    }
}
