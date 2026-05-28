using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHp = 100;
    private float currentHp;
    public float MaxHp => maxHp;
    public float CurrentHp => currentHp;
    public bool IsDead => currentHp <= 0;
    public event Action<float, float> OnHealthChanged;
    public event Action<float> OnDamaged;
    public event Action OnDead;
    private void Awake()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"Take Damage: {damage}");
        if (IsDead) return;
        currentHp -= damage;
        currentHp = Mathf.Max(0f, currentHp);
        OnDamaged?.Invoke(damage);
        OnHealthChanged?.Invoke(currentHp, maxHp);
        if (currentHp <= 0f)
        {
            OnDead?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (currentHp < maxHp)
        {
            currentHp += amount;
            if (currentHp > maxHp)
            {
                currentHp = maxHp;
            }
            OnHealthChanged?.Invoke(currentHp, maxHp);
        }
    }
}
