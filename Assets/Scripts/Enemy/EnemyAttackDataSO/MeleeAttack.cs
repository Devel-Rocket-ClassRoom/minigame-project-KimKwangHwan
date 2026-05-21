using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Enemy/Attack/Melee")]
public class MeleeAttack : EnemyAttackPattern
{
    [Header("타이밍")]
    public float windupTime = 0.3f;
    public float activeTime = 0.15f;
    public float recoveryTime = 0.25f;

    [Header("히트박스")]
    public float damage = 10f;
    public Vector2 hitboxOffset = new(1f, 0f);  // x는 facing으로 부호 결정됨
    public Vector2 hitboxSize = new(1.5f, 1.2f);
    public float knockback = 4f;

    [Header("애니메이션")]
    public string animTrigger = "Attack";

    public override IEnumerator Execute(EnemyContext ctx)
    {
        ctx.anim.SetTrigger(animTrigger);
        yield return new WaitForSeconds(windupTime);

        // facing 적용: 왼쪽 보면 offset.x 반전
        float facing = ctx.Facing;
        Vector2 offset = new(hitboxOffset.x * facing, hitboxOffset.y);

        ctx.hitbox.Enable(damage, offset, hitboxSize, knockback, facing);
        yield return new WaitForSeconds(activeTime);
        ctx.hitbox.Disable();

        yield return new WaitForSeconds(recoveryTime);
    }
}
