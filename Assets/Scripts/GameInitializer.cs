using Cysharp.Threading.Tasks;
using UnityEditor;
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
        if (SceneManager.GetActiveScene().name == "TitleScene") return;

        var instance = Instance;
        if (!instance.autoLoadOnStart) return;
        BootstrapAsync().Forget();
    }

    private static async UniTaskVoid BootstrapAsync()
    {
        SaveDataV data = await SaveManager.Instance.LoadLastUsedAsync();
        Instance.InitWithFadeIn(data).Forget();
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
        SceneManager.LoadSceneAsync(sceneName).ToUniTask().Forget();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Additive) return;
        if (!_pendingGameStart) return;

        _pendingGameStart = false;
        OnSceneLoadAsync().Forget();
    }

    private async UniTaskVoid OnSceneLoadAsync()
    {
        SaveDataV data = await SaveManager.Instance.LoadAsync(SaveManager.Instance.ActiveSlot);
        InitWithFadeIn(data).Forget();
    }

    public void QuitToTitle()
    {
        QuitToTitleRoutine().Forget();
    }

    private async UniTask QuitToTitleRoutine()
    {
        await FadeController.Instance.FadeOut(fadeOutDuration);
        if (PlayerManager.Instance?.Current != null)
        {
            Destroy(PlayerManager.Instance.Current.gameObject);
            PlayerManager.Instance.Clear(PlayerManager.Instance.Current);
        }
        await MapManager.Instance.UnloadAll();
        PauseManager.Instance.Resume();
        SceneManager.LoadScene(titleSceneName);
    }

    public void Restart()
    {
        RestartRoutine().Forget();
    }

    private async UniTask RestartRoutine()
    {
        await FadeController.Instance.FadeOut(fadeOutDuration);

        Destroy(PlayerManager.Instance?.Current.gameObject);
        PlayerManager.Instance.Clear(PlayerManager.Instance?.Current);
        Switch.ClearPersistent();
        Chest.ClearPersistent();
        await MapManager.Instance.UnloadAll();
        SaveDataV data = SaveManager.Instance.Load(SaveManager.Instance.ActiveSlot);
        await SpawnPlayerRoutine(data);

        await FadeController.Instance.FadeIn(fadeInDuration);
    }

    private async UniTask InitWithFadeIn(SaveDataV data)
    {
        await SpawnPlayerRoutine(data);
        await FadeController.Instance.FadeIn(fadeInDuration);
    }

    private async UniTask SpawnPlayerRoutine(SaveDataV data)
    {
        await MapManager.Instance.Initialize(data?.mapId);
        if (data?.activatedSwitchIds != null)
            Switch.RestoreActivated(data.activatedSwitchIds);
        if (data?.activatedChestIds != null)
            Chest.RestoreActivated(data.activatedChestIds);
        Vector2 pos = data != null ? data.GetPosition() : defaultSpawnPos;
        PlayerManager.Instance.SpawnAt(pos);
        await UniTask.Yield();
        var player = PlayerManager.Instance.Current;
        if (player != null)
        {
            if (data?.playerStats != null)
            {
                player.Stats.AttackPower.SetBaseValue(data.playerStats.attackPower);
                player.Stats.MaxHp.SetBaseValue(data.playerStats.maxHp);
                player.Stats.MaxStamina.SetBaseValue(data.playerStats.maxStamina);
                player.Stats.MaxAmmo.SetBaseValue(data.playerStats.maxAmmo);
                player.Stats.MaxHealItems.SetBaseValue(data.playerStats.maxHealItems);
                player.AllRecovery();
            }
            player.Health.ForceNotify();
            player.Stamina.ForceNotify();
            player.Inventory.ForceNotify();
        }
        CameraController.Instance.SnapToPlayer();
    }
}
