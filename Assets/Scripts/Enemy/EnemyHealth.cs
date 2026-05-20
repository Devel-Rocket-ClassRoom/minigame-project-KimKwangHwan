using UnityEngine;
public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHp = 100;
    private float currentHp;
    public float MaxHp => maxHp;
    public float CurrentHp => currentHp;
    public bool IsDead => currentHp <= 0;

    private void Awake()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;
    }
}
