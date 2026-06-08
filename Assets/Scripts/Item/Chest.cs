using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private string chestId;
    [SerializeField] private Item item;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite openedSprite;
    [SerializeField] private GameObject key;
    [SerializeField] private float collectDelay = 0.6f;
    [SerializeField] private Transform spawnPoint;
    private Animator _animator;
    private SpriteRenderer _renderer;
    private bool _isOpened;
    private bool _playerNearby;

    private static readonly Dictionary<string, Chest> Registry = new Dictionary<string, Chest>();
    private static readonly HashSet<string> s_persistentActivated = new HashSet<string>();
    public AudioClip openClip;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.sprite = defaultSprite;
        if (key != null) key.SetActive(false);
    }

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(chestId)) return;
        Registry[chestId] = this;
        if (s_persistentActivated.Contains(chestId))
            RestoreSilent();
    }

    private void OnDisable()
    {
        if (!string.IsNullOrEmpty(chestId)
            && Registry.TryGetValue(chestId, out var chest) && chest == this)
            Registry.Remove(chestId);
    }

    private void Update()
    {
        if (_isOpened || !_playerNearby) return;
        var player = PlayerManager.Instance?.Current;
        if (player == null) return;
        if (player.Input.InteractPressed)
            Open(player);
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

    private void Open(PlayerController player)
    {
        _isOpened = true;
        if (key != null) key.SetActive(false);
        _animator.SetTrigger("Open");
        SFXManager.Instance?.PlaySFX(openClip);
        if (!string.IsNullOrEmpty(chestId))
            s_persistentActivated.Add(chestId);
        if (item != null)
        {
            var spawnedItem = Instantiate(item, spawnPoint.position, Quaternion.identity);
            StartCoroutine(CollectAfterDelay(spawnedItem, player));
        }
    }

    private IEnumerator CollectAfterDelay(Item spawnedItem, PlayerController player)
    {
        yield return new WaitForSeconds(collectDelay);
        spawnedItem.Collect(player);
    }

    private void RestoreSilent()
    {
        if (_isOpened) return;
        _isOpened = true;
        _animator.enabled = false;
        _renderer.sprite  = openedSprite;
        if (key != null) key.SetActive(false);
    }

    public static List<string> GetAllActivatedIds() => new List<string>(s_persistentActivated);
    public static void ClearPersistent() => s_persistentActivated.Clear();
    public static bool TryGet(string id, out Chest c) => Registry.TryGetValue(id, out c);

    public static void RestoreActivated(List<string> ids)
    {
        s_persistentActivated.Clear();
        foreach (var id in ids) s_persistentActivated.Add(id);
        foreach (var id in ids)
            if (Registry.TryGetValue(id, out var chest))
                chest.RestoreSilent();
    }
}
