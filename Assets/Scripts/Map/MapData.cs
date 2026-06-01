using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Map/MapData")]
public class MapData : ScriptableObject
{
    public string mapId;
    public string sceneName;
    public Vector2 sceneOffset;
    public Vector2 boundsCenter;
    public Vector2 boundsSize;
    public List<MapConnection> connections = new List<MapConnection>();

    public Rect GetCameraBounds()
    {
        Vector2 center = sceneOffset + boundsCenter;
        return new Rect(center - boundsSize * 0.5f, boundsSize);
    }
}

[Serializable]
public class MapConnection
{
    public MapData targetMap;
    public string exitZoneId;
    public string entryZoneId;
}
