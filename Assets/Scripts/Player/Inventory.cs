using System;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private float healAmount = 30f;

    private PlayerStats _stats;
    private PlayerStats Stats => _stats != null ? _stats : (_stats = GetComponent<PlayerStats>());

    public int CurrentAmmo { get; private set; }
    public int MaxAmmo => Mathf.RoundToInt(Stats.MaxAmmo.FinalValue);
    public int CurrentHealItems { get; private set; }
    public int MaxHealItems => Mathf.RoundToInt(Stats.MaxHealItems.FinalValue);

    public event Action<int, int> OnAmmoChanged;
    public event Action<int, int> OnHealItemChanged;

    private void Awake()
    {
        CurrentAmmo = MaxAmmo;
        CurrentHealItems = MaxHealItems;
        Stats.MaxAmmo.OnValueChanged += OnMaxAmmoChanged;
        Stats.MaxHealItems.OnValueChanged += OnMaxHealItemsChanged;
    }

    private void OnDestroy()
    {
        if (_stats == null) return;
        _stats.MaxAmmo.OnValueChanged -= OnMaxAmmoChanged;
        _stats.MaxHealItems.OnValueChanged -= OnMaxHealItemsChanged;
    }

    private void OnMaxAmmoChanged(float newMax)
    {
        CurrentAmmo = Mathf.Min(CurrentAmmo, MaxAmmo);
        OnAmmoChanged?.Invoke(MaxAmmo, CurrentAmmo);
    }

    private void OnMaxHealItemsChanged(float newMax)
    {
        CurrentHealItems = Mathf.Min(CurrentHealItems, MaxHealItems);
        OnHealItemChanged?.Invoke(MaxHealItems, CurrentHealItems);
    }

    public void ForceNotify()
    {
        OnAmmoChanged?.Invoke(MaxAmmo, CurrentAmmo);
        OnHealItemChanged?.Invoke(MaxHealItems, CurrentHealItems);
    }

    public bool TryUseAmmo()
    {
        if (CurrentAmmo == 0) return false;
        CurrentAmmo--;
        OnAmmoChanged?.Invoke(MaxAmmo, CurrentAmmo);
        return true;
    }

    public bool TryUseHealItem(PlayerHealth health)
    {
        if (health.CurrentHp == health.MaxHp) return false;
        if (CurrentHealItems == 0) return false;
        CurrentHealItems--;
        health.Heal(healAmount);
        OnHealItemChanged?.Invoke(MaxHealItems, CurrentHealItems);
        return true;
    }

    public void AddAmmo(int amount)
    {
        CurrentAmmo = Mathf.Min(CurrentAmmo + amount, MaxAmmo);
        OnAmmoChanged?.Invoke(MaxAmmo, CurrentAmmo);
    }

    public void AddHealItem(int amount)
    {
        CurrentHealItems = Mathf.Min(CurrentHealItems + amount, MaxHealItems);
        OnHealItemChanged?.Invoke(MaxHealItems, CurrentHealItems);
    }
}
