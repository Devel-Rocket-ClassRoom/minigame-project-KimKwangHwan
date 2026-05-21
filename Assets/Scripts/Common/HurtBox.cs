using UnityEngine;

public class HurtBox : MonoBehaviour
{
    // [SerializeField] private Object ownerObject;
    private IDamageable owner;
    [SerializeField] private float defense;
    [SerializeField] private float resistance;
    private void Awake()
    {
        owner = transform.parent.GetComponent<IDamageable>();
    }
    public void ReceiveHit(float damage)
    {
        float finalDamage = Mathf.Max(damage - defense, 1f);
        owner.TakeDamage(finalDamage);
    }
}
