using System;
using UnityEngine;
public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHp = 100;
    private float currentHp;
    public float MaxHp => maxHp;
    public float CurrentHp => currentHp;
    public bool IsDead => currentHp <= 0;
    public event Action<float> OnDamaged;
    public event Action OnDead;
    private void Awake()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead) return;
        currentHp -= damage;
        OnDamaged?.Invoke(damage);
        if (currentHp <= 0f)
        {
            OnDead?.Invoke();
        }
    }
}
