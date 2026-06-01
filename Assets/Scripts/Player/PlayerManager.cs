using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] private PlayerController _playerPrefab;
    private PlayerController _current;
    public PlayerController Current => _current;
    public bool HasPlayer => _current != null;

    // 정적으로 두면 Instance 준비 여부와 무관하게 구독 가능
    public static event System.Action<PlayerController> OnPlayerSet;
    public static event System.Action OnPlayerCleared;
    public PlayerController SpawnAt(Vector2 pos)
    {
        if (_current != null) return _current;
        if (_playerPrefab == null) return null;
        return Instantiate(_playerPrefab, pos, Quaternion.identity);
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public void Set(PlayerController player)
    {
        _current = player;
        OnPlayerSet?.Invoke(player);
    }

    public void Clear(PlayerController player)
    {
        if (_current != player) return;
        _current = null;
        OnPlayerCleared?.Invoke();
    }
}