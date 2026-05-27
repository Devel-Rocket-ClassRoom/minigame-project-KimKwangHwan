using UnityEngine;

public class Stationary : MonoBehaviour
{
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private GameObject hitPrefab;
    [SerializeField] private string animStateName;
    private float damage = 20f;
    private PooledObject pooled;
    private PooledObject Pooled => pooled != null ? pooled : (pooled = GetComponent<PooledObject>());
    private BoxCollider2D hitbox;
    private Animator animator;

    private void Awake()
    {
        hitbox = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        animator.Play(animStateName, 0, 0f);
        animator.Update(0f);
        hitbox.enabled = false;
    }
    public void HitboxEnable()
    {
        hitbox.enabled = true;
    }
    public void HitboxDisable()
    {
        hitbox.enabled = false;
    }
    public void Recovery()
    {
        Pooled.Despawn();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hitPrefab != null)
            Instantiate(hitPrefab, transform.position, Quaternion.identity);
        if (((1 << other.gameObject.layer) & hitLayer.value) != 0)
        {
            if (other.TryGetComponent<HurtBox>(out var hurtbox))
            {
                hurtbox.ReceiveHit(this.damage);
            }
        }
    }
}
