using System;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private int maxHealItems = 3;
    [SerializeField] private float healAmount = 30f;

    public int CurrentAmmo { get; private set; }
    public int MaxAmmo { get { return maxAmmo; } }
    public int CurrentHealItems { get; private set; }
    public int MaxHealItems => maxHealItems;
    public event Action<int, int> OnAmmoChanged;
    public event Action<int, int> OnHealItemChanged;

    private void Awake()
    {
        CurrentAmmo = maxAmmo;
        CurrentHealItems = maxHealItems;
    }

    public void ForceNotify()
    {
        OnAmmoChanged?.Invoke(MaxAmmo, CurrentAmmo);
        OnHealItemChanged?.Invoke(MaxHealItems, CurrentHealItems);
    }

    public bool TryUseAmmo()
    {
        if (CurrentAmmo == 0)
            return false;
        CurrentAmmo--;
        OnAmmoChanged?.Invoke(MaxAmmo, CurrentAmmo);
        return true;
    }
    public bool TryUseHealItem(PlayerHealth health)
    {
        if (health.CurrentHp == health.MaxHp)
            return false;
        if (CurrentHealItems == 0)
            return false;
        CurrentHealItems--;
        health.Heal(healAmount);
        OnHealItemChanged?.Invoke(MaxHealItems, CurrentHealItems);
        return true;
    }
    public void AddAmmo(int amount)
    {
        CurrentAmmo += amount;
        if (CurrentAmmo > MaxAmmo)
            CurrentAmmo = MaxAmmo;
        OnAmmoChanged?.Invoke(MaxAmmo, CurrentAmmo);
    }
    public void AddHealItem(int amount)
    {
        CurrentHealItems += amount;
        if (CurrentHealItems > MaxHealItems)
            CurrentHealItems = MaxHealItems;
        OnHealItemChanged?.Invoke(MaxHealItems, CurrentHealItems);
    }
}
