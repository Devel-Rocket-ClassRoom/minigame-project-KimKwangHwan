using System.Collections;
using UnityEngine;
using SaveDataV = SaveDataV1;

public class GameInitializer : Singleton<GameInitializer>
{
    [SerializeField] private Vector2 defaultSpawnPos = Vector2.zero;
    [SerializeField] private bool autoLoadOnStart = true;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        var instance = Instance;
        if (!instance.autoLoadOnStart) return;
        SaveDataV data = SaveManager.Instance.LoadLastUsed();
        instance.StartCoroutine(instance.SpawnPlayerRoutine(data));
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public void ApplyLoad(int slot)
    {
        SaveDataV data = SaveManager.Instance.Load(slot);
        StartCoroutine(SpawnPlayerRoutine(data));
    }

    public void Restart()
    {
        StartCoroutine(RestartRoutine());
    }

    private IEnumerator RestartRoutine()
    {
        Destroy(PlayerManager.Instance?.Current.gameObject);
        PlayerManager.Instance.Clear(PlayerManager.Instance?.Current);
        Switch.ClearPersistent();
        yield return MapManager.Instance.UnloadAll();
        SaveDataV data = SaveManager.Instance.LoadLastUsed();
        yield return SpawnPlayerRoutine(data);
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
