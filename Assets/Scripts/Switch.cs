using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite switchedSprite;
    [SerializeField] private Door linkedDoor;
    private SpriteRenderer _renderer;
    private bool _isActivated;
    private bool _playerNearby;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.sprite = defaultSprite;
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
            _playerNearby = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _playerNearby = false;
    }

    private void Activate()
    {
        _isActivated = true;
        _renderer.sprite = switchedSprite;
        linkedDoor?.Open();
    }
}
