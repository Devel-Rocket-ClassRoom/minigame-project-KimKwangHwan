using UnityEngine;

public class HurtBox : MonoBehaviour
{
    [SerializeField] private IDamageable owner;
    [SerializeField] private float defense;
    [SerializeField] private float resistance;

    public void ReceiveHit(float damage)
    {
        float finalDamage = Mathf.Max(damage - defense, 1f);
        owner.TakeDamage(finalDamage);
    }
}
