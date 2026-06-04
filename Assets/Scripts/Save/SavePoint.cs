using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using SaveDataV = SaveDataV1;

[RequireComponent(typeof(Collider2D))]
public class SavePoint : MonoBehaviour
{
    [SerializeField] private string savePointId = "sp_01";
    [SerializeField] private SpriteRenderer glowSprite; // 활성 시 강조용(선택)

    private bool _playerInRange;
    private PlayerController _player;

    private static readonly Dictionary<string, SavePoint> Registry = new Dictionary<string, SavePoint>();
    public string Id => savePointId;
    public Vector2 SpawnPos => transform.position;

    private void Awake()
    {
        if (glowSprite != null) glowSprite.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[SavePoint] Trigger Enter: {other.name}");
        var player = other.GetComponent<PlayerController>();
        if (player == null) return;

        _playerInRange = true;
        _player = player;

        if (glowSprite != null) glowSprite.enabled = true;
        //if (SaveSlotUI.Instance != null) SaveSlotUI.Instance.ShowPrompt(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player == null) return;

        _playerInRange = false;
        _player = null;

        if (glowSprite != null) glowSprite.enabled = false;
        //if (SaveSlotUI.Instance != null) SaveSlotUI.Instance.ShowPrompt(false);
    }

    private void Update()
    {
        if (!_playerInRange || _player == null) return;
        // if (Keyboard.current == null) return;

        // 이미 슬롯 패널이 열려 있으면(타임스케일 0) 중복 오픈 방지
        //if (SaveSlotUI.Instance != null && SaveSlotUI.Instance.IsOpen) return;

        if (_player.Input.InteractPressed)
        {
            Debug.Log("[SavePoint] 저장 키 입력 감지");
            //SaveSlotUI.Instance.OpenForSave(_player, this);
            ExecuteSave(0, _player); // 테스트용: 슬롯 패널 없이 바로 저장
            _player.AllRecovery();
        }
    }

    public void ExecuteSave(int slot, PlayerController player)
    {
        SaveDataV data = new SaveDataV();
        data.savePointId = savePointId;
        data.mapId = MapManager.Instance.CurrentMap?.mapId;
        data.savedAt = System.DateTime.Now;
        data.activatedSwitchIds = Switch.GetAllActivatedIds();
        SaveManager.Instance.Save(slot, data);
    }
    private void OnEnable() => Registry[savePointId] = this;
    private void OnDisable()
    {
        if (Registry.TryGetValue(savePointId, out var sp) && sp == this)
            Registry.Remove(savePointId);
    }
    public static bool TryGet(string id, out SavePoint point) => Registry.TryGetValue(id, out point);
}