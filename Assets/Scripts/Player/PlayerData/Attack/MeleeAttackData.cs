using UnityEngine;
using UnityEngine.UIElements.Experimental;

[CreateAssetMenu(menuName = "Combat/Melee Attack")]
public sealed class MeleeAttackData : AttackData
{
    public float damage = 10f;
    public Vector2 hitboxOffset;
    public Vector2 hitboxSize = Vector2.one;
    public float knockback = 4f;
    public override void OnActiveEnd(AttackRuntime rt)
    {
        rt.Hitbox.Disable();
    }

    public override void OnActiveStart(AttackRuntime rt)
    {
        var offset = hitboxOffset;
        rt.Hitbox.Enable(damage, offset, hitboxSize, knockback, rt.Facing);
    }

    public override void OnEnter(AttackRuntime rt)
    {
    }

    public override void OnExit(AttackRuntime rt)
    {
        rt.Hitbox.Disable();
    }
}