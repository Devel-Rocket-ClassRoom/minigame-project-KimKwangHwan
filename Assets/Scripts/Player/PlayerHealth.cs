using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    private PlayerStats _stats;
    private PlayerStats Stats => _stats != null ? _stats : (_stats = GetComponent<PlayerStats>());

    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip healClip;
    private float currentHp;
    public float MaxHp => Stats.MaxHp.FinalValue;
    public float CurrentHp => currentHp;
    public bool IsDead => currentHp <= 0;
    public event Action<float, float> OnHealthChanged;
    public event Action<float> OnDamaged;
    public event Action OnDead;

    private void Awake()
    {
        currentHp = MaxHp;
        Stats.MaxHp.OnValueChanged += OnMaxHpChanged;
    }

    private void OnDestroy()
    {
        if (_stats != null)
            _stats.MaxHp.OnValueChanged -= OnMaxHpChanged;
    }

    private void OnMaxHpChanged(float newMax)
    {
        currentHp = Mathf.Min(currentHp, newMax);
        OnHealthChanged?.Invoke(currentHp, MaxHp);
    }

    public void TakeDamage(float damage)
    {
        if (IsDead) return;
        currentHp -= damage;
        currentHp = Mathf.Max(0f, currentHp);
        OnDamaged?.Invoke(damage);
        OnHealthChanged?.Invoke(currentHp, MaxHp);
        SFXManager.Instance?.PlaySFX(hurtClip);
        if (currentHp <= 0f)
        {
            SFXManager.Instance?.PlaySFX(deathClip);
            OnDead?.Invoke();
        }
    }

    public void ForceNotify() => OnHealthChanged?.Invoke(currentHp, MaxHp);

    public void Heal(float amount)
    {
        if (currentHp < MaxHp)
        {
            currentHp += amount;
            if (currentHp > MaxHp)
                currentHp = MaxHp;
            SFXManager.Instance?.PlaySFX(healClip);
            OnHealthChanged?.Invoke(currentHp, MaxHp);
        }
    }
}
