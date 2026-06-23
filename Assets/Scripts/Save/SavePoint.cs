using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using SaveDataV = SaveDataV1;

[RequireComponent(typeof(Collider2D))]
public class SavePoint : MonoBehaviour
{
    [SerializeField] private string savePointId = "sp_01";
    [SerializeField] private GameObject key;

    private bool _playerInRange;
    private PlayerController _player;

    private static readonly Dictionary<string, SavePoint> Registry = new Dictionary<string, SavePoint>();
    public string Id => savePointId;
    public Vector2 SpawnPos => transform.position;

    private void Awake()
    {
        if (key != null) key.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player == null) return;

        _playerInRange = true;
        _player = player;

        if (key != null) key.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player == null) return;

        _playerInRange = false;
        _player = null;

        if (key != null) key.SetActive(false);
    }

    private void Update()
    {
        if (!_playerInRange || _player == null) return;

        if (_player.Input.InteractPressed)
        {
            ExecuteSave(SaveManager.Instance.ActiveSlot, _player);
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
        data.activatedChestIds  = Chest.GetAllActivatedIds();
        data.playerStats = new PlayerStatsData
        {
            attackPower  = player.Stats.AttackPower.FinalValue,
            maxHp        = player.Stats.MaxHp.FinalValue,
            maxStamina   = player.Stats.MaxStamina.FinalValue,
            maxAmmo      = player.Stats.MaxAmmo.FinalValue,
            maxHealItems = player.Stats.MaxHealItems.FinalValue,
        };
        //SaveManager.Instance.Save(slot, data);
        SaveManager.Instance.SaveAsync(slot, data).Forget();
    }
    private void OnEnable() => Registry[savePointId] = this;
    private void OnDisable()
    {
        if (Registry.TryGetValue(savePointId, out var sp) && sp == this)
            Registry.Remove(savePointId);
    }
    public static bool TryGet(string id, out SavePoint point) => Registry.TryGetValue(id, out point);
}