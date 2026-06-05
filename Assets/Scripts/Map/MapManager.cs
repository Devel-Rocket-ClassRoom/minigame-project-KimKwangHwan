using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : Singleton<MapManager>
{
    [SerializeField] private List<MapData> allMaps = new List<MapData>();
    [SerializeField] private MapData defaultMap;

    public MapData CurrentMap { get; private set; }
    public Rect CurrentBounds => CurrentMap != null ? CurrentMap.GetCameraBounds() : new Rect();

    private readonly Dictionary<MapData, Scene> _loadedScenes = new();
    private bool _isTransitioning;

    public IEnumerator Initialize(string mapId)
    {
        MapData map = allMaps.Find(m => m.mapId == mapId);
        if (map == null) map = defaultMap;
        if (map == null)
        {
            Debug.LogWarning("[MapManager] 로드할 맵이 없습니다.");
            yield break;
        }

        CurrentMap = map;
        yield return LoadMap(map);

        foreach (var conn in map.connections)
        {
            if (conn.targetMap != null)
                yield return LoadMap(conn.targetMap);
        }

        if (CameraController.Instance != null)
            CameraController.Instance.SetBounds(CurrentBounds);
    }

    public void StartTransition(MapData target, string entryZoneId)
    {
        if (_isTransitioning || target == null) return;
        StartCoroutine(TransitionTo(target, entryZoneId));
    }

    private IEnumerator TransitionTo(MapData target, string entryZoneId)
    {
        _isTransitioning = true;

        yield return LoadMap(target);

        if (MapTransitionZone.TryGet(entryZoneId, out var zone))
        {
            PlayerManager.Instance.Current.Motor.WarpTo(zone.SpawnPosition);
        }
        else
        {
            Debug.LogWarning($"[MapManager] entryZone '{entryZoneId}'을 찾을 수 없습니다.");
        }

        CurrentMap = target;

        if (CameraController.Instance != null)
            CameraController.Instance.SetBounds(CurrentBounds);

        foreach (var conn in target.connections)
        {
            if (conn.targetMap != null)
                yield return LoadMap(conn.targetMap);
        }

        var needed = new HashSet<MapData>(target.connections
            .Where(c => c.targetMap != null)
            .Select(c => c.targetMap)) { target };

        foreach (var map in _loadedScenes.Keys.Except(needed).ToList())
            yield return UnloadMap(map);

        _isTransitioning = false;
    }

    private IEnumerator LoadMap(MapData map)
    {
        if (_loadedScenes.ContainsKey(map)) yield break;

        var op = SceneManager.LoadSceneAsync(map.sceneName, LoadSceneMode.Additive);
        yield return op;

        Scene scene = SceneManager.GetSceneByName(map.sceneName);
        if (!scene.IsValid())
        {
            Debug.LogError($"[MapManager] '{map.sceneName}' 씬을 찾을 수 없습니다. Build Settings에 추가됐는지 확인하세요.");
            yield break;
        }

        foreach (var root in scene.GetRootGameObjects())
            root.transform.position += (Vector3)map.sceneOffset;

        _loadedScenes[map] = scene;
    }

    public IEnumerator UnloadAll()
    {
        _isTransitioning = false;
        foreach (var map in _loadedScenes.Keys.ToList())
            yield return UnloadMap(map);
        CurrentMap = null;
    }

    private IEnumerator UnloadMap(MapData map)
    {
        if (!_loadedScenes.TryGetValue(map, out var scene)) yield break;
        yield return SceneManager.UnloadSceneAsync(scene);
        _loadedScenes.Remove(map);
    }
}
