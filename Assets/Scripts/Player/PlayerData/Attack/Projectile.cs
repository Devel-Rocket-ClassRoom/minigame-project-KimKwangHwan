using UnityEngine;

public sealed class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    [SerializeField] private LayerMask hitLayer;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        Destroy(gameObject, 5f);
    }
    private bool IsInMask(LayerMask mask, GameObject obj) => (mask.value & (1 << obj.layer)) != 0;
    public void Launch(Vector2 dir, float speed, float damage, float facing)
    {
        rb.linearVelocityX = dir.x * speed;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsInMask(hitLayer, other.gameObject)) return;
        animator.SetTrigger("Hit");
        rb.linearVelocity = Vector2.zero;
        Destroy(gameObject, 1f);
    }
}