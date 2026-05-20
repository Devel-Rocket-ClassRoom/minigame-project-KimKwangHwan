using UnityEngine;

public interface IDamageable
{
    void TakeDamage();
    bool IsDead { get; set; }
}
