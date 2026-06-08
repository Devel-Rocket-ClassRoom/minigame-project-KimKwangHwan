using UnityEngine;
using UnityEngine.UI;

public class Projectile : MonoBehaviour
{
    protected Rigidbody2D rb;
    [SerializeField] protected LayerMask hitLayer;
    [SerializeField] protected GameObject hitPrefab;
    protected float damage;
    [SerializeField] protected float duration = 1.3f;
    [SerializeField] protected bool isBreakable = true;
    protected PooledObject pooled;
    protected PooledObject Pooled => pooled != null ? pooled : (pooled = GetComponent<PooledObject>());
    protected float timer;

    [SerializeField] protected AudioClip hitClip;
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    protected virtual void OnEnable()
    {
        timer = 0f;
    }
    protected virtual void Update()
    {
        timer += Time.deltaTime;
        if (timer > duration)
        {
            Pooled.Despawn();
        }
    }
    public virtual void Launch(Vector2 dir, float speed, float damage, float facing)
    {
        rb.linearVelocity = dir * speed;
        this.damage = damage;
    }
    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & hitLayer.value) != 0)
        {
            if (other.TryGetComponent<HurtBox>(out var hurtbox))
            {
                hurtbox.ReceiveHit(this.damage);
                SFXManager.Instance?.PlaySFX(hitClip);
            }
        }
        else
            return;
        if (hitPrefab != null)
            Instantiate(hitPrefab, transform.position, Quaternion.identity);
        if (isBreakable)
            Pooled.Despawn();
    }
}