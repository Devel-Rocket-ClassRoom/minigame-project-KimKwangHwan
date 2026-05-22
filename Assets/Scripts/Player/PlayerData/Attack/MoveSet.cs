using System;
using UnityEngine;

[Serializable]
public sealed class AttackBranch
{
    public AttackData[] grounded;
    public AttackData[] airborne;
    public AttackData[] running;

    AttackData[] Pick(AttackContext ctx) => ctx switch
    {
        AttackContext.Grounded => grounded,
        AttackContext.Airborne => airborne,
        AttackContext.Running => running,
        _ => null,
    };

    public AttackData Resolve(AttackContext ctx, int comboIndex)
    {
        var arr = Pick(ctx);
        if (arr == null || arr.Length == 0) return null;
        return arr[Mathf.Min(comboIndex, arr.Length - 1)];
    }

    public bool HasComboAt(AttackContext ctx, int index)
    {
        var arr = Pick(ctx);
        return arr != null && index < arr.Length;
    }
}

[CreateAssetMenu(menuName = "Combat/MoveSet")]
public sealed class MoveSet : ScriptableObject
{
    [SerializeField] AttackBranch melee = new();
    [SerializeField] AttackBranch ranged = new();

    AttackBranch Branch(AttackType type)
        => type == AttackType.Melee ? melee : ranged;

    public AttackData Resolve(AttackType type, AttackContext ctx, int comboIndex)
        => Branch(type).Resolve(ctx, comboIndex);
    public bool HasComboAt(AttackType type, AttackContext ctx, int index)
        => Branch(type).HasComboAt(ctx, index);
}