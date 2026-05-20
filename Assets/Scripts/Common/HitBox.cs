using UnityEngine;
using System.Collections.Generic;

public class Hitbox : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;
    private BoxCollider2D _col;
    private readonly HashSet<int> _hitOnce = new();
    private float _damage, _knockback, _facing = 1f;
    private bool _active;

    void Awake()
    {
        _col = GetComponent<BoxCollider2D>();
        _col.isTrigger = true;
        _col.enabled = false;
    }

    public void Enable(float damage, Vector2 offset, Vector2 size,
                       float knockback, float facing)
    {
        // TODO: 콜라이더 위치/크기 세팅 후 활성화.
        //       같은 스윙 내 중복 타격 방지(이미 맞은 대상 HashSet)는 필수.
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
        // TODO: 콜라이더 비활성 + 타격 대상 set 클리어
        _col.enabled = false;
        _active = false;
        _hitOnce.Clear();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_active) return;
        if (((1 << other.gameObject.layer) & targetLayer.value) == 0) return;
        if (!_hitOnce.Add(other.GetInstanceID())) return;
        if (!other.TryGetComponent<HurtBox>(out var hurtbox)) return;
        hurtbox.ReceiveHit(_damage);
        // Debug.Log($"[Hitbox] HIT {other.name} dmg={_damage} kb={_knockback} dir={_facing}");
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
