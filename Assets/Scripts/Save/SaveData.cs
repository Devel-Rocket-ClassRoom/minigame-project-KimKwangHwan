using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class SaveData
{
    public int Version { get; protected set; }
    public abstract SaveData VersionUp();
}

[System.Serializable]
public class SaveDataV1 : SaveData
{
    public string savePointId;
    public string mapId;
    public System.DateTime savedAt;
    public List<string> activatedSwitchIds = new List<string>();
    public SaveDataV1()
    {
        Version = 1;
    }
    public override SaveData VersionUp()
    {
        return this;
    }
    public Vector2 GetPosition()
    {
        SavePoint sp;
        if (SavePoint.TryGet(savePointId, out sp))
        {
            return sp.SpawnPos;
        }
        else
        {
            Debug.LogWarning($"Save point with id {savePointId} not found. Defaulting to (0,0).");
            return Vector2.zero;
        }
    }
}