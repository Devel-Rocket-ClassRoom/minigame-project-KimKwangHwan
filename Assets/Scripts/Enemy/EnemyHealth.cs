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

    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip deathClip;
    private void Awake()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead) return;
        currentHp -= damage;
        OnDamaged?.Invoke(damage);
        SFXManager.Instance?.PlaySFX(hurtClip);
        if (currentHp <= 0f)
        {
            SFXManager.Instance?.PlaySFX(deathClip);
            OnDead?.Invoke();
        }
    }
}
