using UnityEngine;
using UnityEngine.UI;

public sealed class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private GameObject hitPrefab;
    private float _damage;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        Destroy(gameObject, 1.3f);
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
        
        Destroy(gameObject);
    }
}