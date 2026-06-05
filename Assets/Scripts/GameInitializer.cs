using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using SaveDataV = SaveDataV1;

public class GameInitializer : Singleton<GameInitializer>
{
    [SerializeField] private Vector2 defaultSpawnPos = Vector2.zero;
    [SerializeField] private bool autoLoadOnStart = true;
    [SerializeField] private string titleSceneName = "TitleScene";
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    private bool _pendingGameStart;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        // TitleScene에서는 게임 시스템을 초기화하지 않음
        if (SceneManager.GetActiveScene().name == "TitleScene") return;

        var instance = Instance;
        if (!instance.autoLoadOnStart) return;
        SaveDataV data = SaveManager.Instance.LoadLastUsed();
        instance.StartCoroutine(instance.InitWithFadeIn(data));
    }

    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // TitleSceneController에서 호출 — 게임 씬 로드 예약
    public void LoadGameScene(string sceneName)
    {
        _pendingGameStart = true;
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 어디티브 맵 로드는 무시, TitleScene에서 넘어온 Main 씬 로드만 처리
        if (mode == LoadSceneMode.Additive) return;
        if (!_pendingGameStart) return;

        _pendingGameStart = false;
        SaveDataV data = SaveManager.Instance.Load(SaveManager.Instance.ActiveSlot);
        StartCoroutine(InitWithFadeIn(data));
    }

    public void Restart()
    {
        StartCoroutine(RestartRoutine());
    }

    private IEnumerator RestartRoutine()
    {
        yield return FadeController.Instance.FadeOut(fadeOutDuration);

        Destroy(PlayerManager.Instance?.Current.gameObject);
        PlayerManager.Instance.Clear(PlayerManager.Instance?.Current);
        Switch.ClearPersistent();
        yield return MapManager.Instance.UnloadAll();
        SaveDataV data = SaveManager.Instance.Load(SaveManager.Instance.ActiveSlot);
        yield return SpawnPlayerRoutine(data);

        yield return FadeController.Instance.FadeIn(fadeInDuration);
    }

    private IEnumerator InitWithFadeIn(SaveDataV data)
    {
        yield return SpawnPlayerRoutine(data);
        yield return FadeController.Instance.FadeIn(fadeInDuration);
    }

    private IEnumerator SpawnPlayerRoutine(SaveDataV data)
    {
        yield return MapManager.Instance.Initialize(data?.mapId);
        if (data?.activatedSwitchIds != null)
            Switch.RestoreActivated(data.activatedSwitchIds);
        Vector2 pos = data != null ? data.GetPosition() : defaultSpawnPos;
        PlayerManager.Instance.SpawnAt(pos);
        yield return null;
        var player = PlayerManager.Instance.Current;
        if (player != null)
        {
            player.Health.ForceNotify();
            player.Stamina.ForceNotify();
            player.Inventory.ForceNotify();
        }
        CameraController.Instance.SnapToPlayer();
    }
}
