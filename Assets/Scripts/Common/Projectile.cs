using UnityEngine;
using UnityEngine.UI;

public sealed class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private GameObject hitPrefab;
    private float _damage;
    [SerializeField] private float duration = 1.3f;
    [SerializeField] private bool isBreakable = true;
    private PooledObject pooled;
    private PooledObject Pooled => pooled != null ? pooled : (pooled = GetComponent<PooledObject>());
    private float timer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        timer = 0f;
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > duration)
        {
            Pooled.Despawn();
        }
    }
    public void Launch(Vector2 dir, float speed, float damage, float facing)
    {
        rb.linearVelocity = dir * speed;
        _damage = damage;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hitPrefab != null)
            Instantiate(hitPrefab, transform.position, Quaternion.identity);
        if (((1 << other.gameObject.layer) & hitLayer.value) != 0)
        {
            if (other.TryGetComponent<HurtBox>(out var hurtbox))
            {
                hurtbox.ReceiveHit(_damage);
            }
        }
        if (isBreakable)
            Pooled.Despawn();
    }
}