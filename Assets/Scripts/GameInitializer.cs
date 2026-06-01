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

    private IEnumerator SpawnPlayerRoutine(SaveDataV data)
    {
        yield return MapManager.Instance.Initialize(data?.mapId);
        Vector2 pos = data != null ? data.GetPosition() : defaultSpawnPos;
        PlayerManager.Instance.SpawnAt(pos);
    }
}
