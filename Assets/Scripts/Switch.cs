using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] private string switchId;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite switchedSprite;
    [SerializeField] private Door linkedDoor;
    [SerializeField] private GameObject key;
    private SpriteRenderer _renderer;
    private bool _isActivated;
    private bool _playerNearby;

    private static readonly Dictionary<string, Switch> Registry = new Dictionary<string, Switch>();
    private static readonly HashSet<string> s_persistentActivated = new HashSet<string>();
    public AudioClip sfxClip;

    private void OnEnable()
    {
        if (!string.IsNullOrEmpty(switchId))
        {
            Registry[switchId] = this;
            if (s_persistentActivated.Contains(switchId))
            {
                ActivateSilent();
            }
        }
    }

    private void OnDisable()
    {
        if (!string.IsNullOrEmpty(switchId) && Registry.TryGetValue(switchId, out var sw) && sw == this)
            Registry.Remove(switchId);
    }

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.sprite = defaultSprite;
        if (key != null) key.SetActive(false);
    }

    private void Update()
    {
        if (_isActivated || !_playerNearby) return;

        var player = PlayerManager.Instance?.Current;
        if (player == null) return;

        if (player.Input.InteractPressed)
            Activate();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerNearby = true;
            if (key != null) key.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerNearby = false;
            if (key != null) key.SetActive(false);
        }
    }

    private void Activate()
    {
        _isActivated = true;
        _renderer.sprite = switchedSprite;
        SFXManager.Instance?.PlaySFX(sfxClip);
        linkedDoor?.Open();
        s_persistentActivated.Add(switchId);
    }

    public void ActivateSilent()
    {
        if (_isActivated) return;
        _isActivated = true;
        _renderer.sprite = switchedSprite;
        linkedDoor?.OpenImmediately();
    }

    public static List<string> GetAllActivatedIds()
    {
        return new List<string>(s_persistentActivated);
    }

    public static void RestoreActivated(List<string> ids)
    {
        s_persistentActivated.Clear();
        foreach (var id in ids) s_persistentActivated.Add(id);
        foreach (var id in ids)
            if (Registry.TryGetValue(id, out var sw))
                sw.ActivateSilent();
    }

    public static void ClearPersistent() => s_persistentActivated.Clear();

    public static bool TryGet(string id, out Switch sw) => Registry.TryGetValue(id, out sw);
}
