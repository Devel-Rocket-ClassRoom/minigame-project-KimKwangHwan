using System;

public interface IDamageable
{
    float MaxHp { get; }
    float CurrentHp { get; }
    void TakeDamage(float damage);
    bool IsDead { get; }
}
