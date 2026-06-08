using UnityEngine;

public class HurtBox : MonoBehaviour
{
    // [SerializeField] private Object ownerObject;
    private IDamageable owner;
    [SerializeField] private float defense;
    [SerializeField] private float resistance;
    [SerializeField] private float hitInterval;
    private BoxCollider2D hitArea;
    private float hitTimer; // 0이면 계속 데미지를 입음
    private void Awake()
    {
        owner = transform.parent.GetComponent<IDamageable>();
        hitArea = transform.GetComponent<BoxCollider2D>();
        CancelInvincible();
    }
    private void Update()
    {
        if (!hitArea.enabled)
        {
            hitTimer += Time.deltaTime;
            if (hitTimer >= hitInterval)
            {
                CancelInvincible();
                hitTimer = 0f;
            }
        }
    }
    public void ReceiveHit(float damage)
    {
        if (!hitArea.enabled) return;
        if (hitInterval > 0f)
            DoInvincible();
        float finalDamage = Mathf.Max(damage - defense, 1f);
        owner.TakeDamage(finalDamage);
    }
    public void DoInvincible()
    {
        hitArea.enabled = false;
    }
    public void CancelInvincible()
    {
        hitArea.enabled = true;
    }
}
