using UnityEngine;
using System.Collections.Generic;

public class Hitbox : MonoBehaviour
{
    private enum HitboxMode { OneShot, Repeating }

    [SerializeField] private LayerMask targetLayer;
    private BoxCollider2D _col;
    private readonly HashSet<int> _hitOnce = new();
    private readonly Dictionary<int, float> _hitTimes = new();
    private float _damage, _knockback, _facing = 1f;
    private bool _active;
    private HitboxMode _mode = HitboxMode.OneShot;
    private float _rehitInterval;

    void Awake()
    {
        _col = GetComponent<BoxCollider2D>();
        _col.isTrigger = true;
        _col.enabled = false;
    }

    public void Enable(float damage, Vector2 offset, Vector2 size,
                       float knockback, float facing)
    {
        _mode = HitboxMode.OneShot;
        _damage = damage;
        _knockback = knockback;

        _col.size = size;
        _col.offset = new Vector2(offset.x, offset.y);

        _hitOnce.Clear();
        _col.enabled = true;
        _active = true;
    }

    public void Enable(float damage, Vector2 offset, Vector2 size,
                       float knockback, float facing, float rehitInterval)
    {
        _mode = HitboxMode.Repeating;
        _rehitInterval = rehitInterval;
        _hitTimes.Clear();
        _damage = damage;
        _knockback = knockback;

        _col.size = size;
        _col.offset = new Vector2(offset.x, offset.y);

        _hitOnce.Clear();
        _col.enabled = true;
        _active = true;
    }

    public void Disable()
    {
        _col.enabled = false;
        _active = false;
        _hitOnce.Clear();
        _hitTimes.Clear();
    }

    void OnTriggerEnter2D(Collider2D other) => TryHit(other);

    void OnTriggerStay2D(Collider2D other)
    {
        if (_mode == HitboxMode.Repeating) TryHit(other);
    }

    void TryHit(Collider2D other)
    {
        if (!_active) return;
        if (((1 << other.gameObject.layer) & targetLayer.value) == 0) return;
        int id = other.GetInstanceID();
        if (_mode == HitboxMode.OneShot)
        {
            if (!_hitOnce.Add(id)) return;
        }
        else
        {
            float now = Time.time;
            if (_hitTimes.TryGetValue(id, out float last) && now - last < _rehitInterval) return;
            _hitTimes[id] = now;
        }

        if (!other.TryGetComponent<HurtBox>(out var hurtbox)) return;
        hurtbox.ReceiveHit(_damage);
    }
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        var col = _col != null ? _col : GetComponent<BoxCollider2D>();
        if (col == null) return;

        Gizmos.color = _active ? new Color(1, 0, 0, 0.9f)    // 활성: 빨강
                               : new Color(1, 1, 0, 0.35f);  // 비활성: 노랑

        Gizmos.matrix = transform.localToWorldMatrix;        // 콜라이더 offset/size는 로컬 공간
        Gizmos.DrawWireCube(col.offset, col.size);
        Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.12f);
        Gizmos.DrawCube(col.offset, col.size);
    }
#endif
}
