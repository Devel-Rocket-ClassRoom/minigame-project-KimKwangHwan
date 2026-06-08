using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using SaveDataV = SaveDataV1;

public class SaveManager : Singleton<SaveManager>
{
    public const int SlotCount = 3;
    private const string LastUsedSlotId = "LastUsedSlot";
    private const string ActiveSlotKey = "ActiveSlot";

    public int ActiveSlot { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        _ = Instance;
    }

    protected override void Awake()
    {
        base.Awake();
        ActiveSlot = PlayerPrefs.GetInt(ActiveSlotKey, 0);
    }

    public void SetActiveSlot(int slot)
    {
        if (!IsValidSlot(slot)) return;
        ActiveSlot = slot;
        PlayerPrefs.SetInt(ActiveSlotKey, slot);
        PlayerPrefs.Save();
    }

    private static string GetPath(int slot) => Path.Combine(Application.persistentDataPath, $"slot{slot}.sav");

    private static bool IsValidSlot(int slot) => slot >= 0 && slot < SlotCount;
    private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    {
        Converters = { new Vector2Converter(), new Vector3Converter(), new QuaternionConverter() }
    };
    public void Save(int slot, SaveDataV data)
    {
        if (!IsValidSlot(slot))
        {
            Debug.LogError($"[SaveManager] 잘못된 슬롯 번호: {slot}");
            return;
        }

        try
        {
            string json = JsonConvert.SerializeObject(data, Settings);
            byte[] encrypted = CryptoHelper.Encrypt(json);
            File.WriteAllBytes(GetPath(slot), encrypted);

            PlayerPrefs.SetInt(LastUsedSlotId, slot);
            PlayerPrefs.Save();

            Debug.Log($"[SaveManager] 슬롯 {slot} 저장 완료 → {GetPath(slot)}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveManager] 슬롯 {slot} 저장 실패: {e.Message}");
        }
    }

    public SaveDataV Load(int slot)
    {
        if (!IsValidSlot(slot)) return null;

        string path = GetPath(slot);
        if (!File.Exists(path)) return null;

        try
        {
            byte[] encrypted = File.ReadAllBytes(path);
            string json = CryptoHelper.Decrypt(encrypted);
            SaveDataV data = JsonConvert.DeserializeObject<SaveDataV>(json, Settings);
            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[SaveManager] 슬롯 {slot} 로드 실패(손상 가능): {e.Message}");
            return null;
        }
    }

    public bool HasSave(int slot)
        => IsValidSlot(slot) && File.Exists(GetPath(slot));

    public bool HasAnySave()
    {
        for (int i = 0; i < SlotCount; i++)
            if (HasSave(i)) return true;
        return false;
    }

    public void DeleteSave(int slot)
    {
        if (!IsValidSlot(slot)) return;

        string path = GetPath(slot);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"[SaveManager] 슬롯 {slot} 삭제됨");
        }
    }

    public SaveDataV LoadLastUsed()
    {
        if (!PlayerPrefs.HasKey(LastUsedSlotId)) return null;
        int slot = PlayerPrefs.GetInt(LastUsedSlotId);
        return Load(slot);
    }
}