using UnityEngine;
using System.Collections.Generic;

[DefaultExecutionOrder(-10)]
public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float baseAttackPower = 10f;
    [SerializeField] private float baseMaxHp = 100f;
    [SerializeField] private float baseMaxStamina = 50f;
    [SerializeField] private float baseMaxAmmo = 30f;
    [SerializeField] private float baseMaxHealItems = 3f;

    public StatContainer AttackPower => _statContainers[StatType.AttackPower];
    public StatContainer MaxHp => _statContainers[StatType.MaxHp];
    public StatContainer MaxStamina => _statContainers[StatType.MaxStamina];
    public StatContainer MaxAmmo => _statContainers[StatType.MaxAmmo];
    public StatContainer MaxHealItems => _statContainers[StatType.MaxHealItems];

    private Dictionary<StatType, StatContainer> _statContainers;
    public Dictionary<StatType, StatContainer> AllStats => _statContainers;

    private void Awake()
    {
        _statContainers = new Dictionary<StatType, StatContainer>
        {
            [StatType.AttackPower] = new StatContainer(baseAttackPower),
            [StatType.MaxHp] = new StatContainer(baseMaxHp),
            [StatType.MaxStamina] = new StatContainer(baseMaxStamina),
            [StatType.MaxAmmo] = new StatContainer(baseMaxAmmo),
            [StatType.MaxHealItems] = new StatContainer(baseMaxHealItems),
        };
    }
}

public enum StatType
{
    AttackPower,
    MaxHp,
    MaxStamina,
    MaxAmmo,
    MaxHealItems
}
