using UnityEngine;
using System.Collections;
[CreateAssetMenu(menuName = "Enemy/Attack/Dash")]
public class DashAttack : EnemyAttackPattern
{
    [Header("타이밍")]
    public float telegraphTime = 0.6f;
    public float dashDuration = 0.4f;
    public float recoveryTime = 0.5f;

    [Header("이동")]
    public float dashSpeed = 14f;

    [Header("히트박스 (몸통 박치기)")]
    public float damage = 15f;
    public Vector2 hitboxOffset = new(0.5f, 0f);
    public Vector2 hitboxSize = new(1.2f, 1.4f);
    public float knockback = 6f;

    [Header("애니메이션")]
    public string animTelegraphTrigger = "DashTelegraph";
    public string animTrigger = "Dash";
    public override bool CanExecute(EnemyContext ctx)
    {
        float dist = Vector2.Distance(ctx.self.position, ctx.target.position);
        return dist <= range && dist > 2f;
    }

    public override IEnumerator Execute(EnemyContext ctx)
    {
        // 1) 예비 동작
        ctx.anim.SetTrigger(animTelegraphTrigger);
        float facing = ctx.Facing;
        Vector2 dir = new(facing, 0f);
        yield return new WaitForSeconds(telegraphTime);

        // 2) 돌진 시작 + 히트박스 ON
        ctx.anim.SetTrigger(animTrigger);
        Vector2 offset = new(hitboxOffset.x * facing, hitboxOffset.y);
        ctx.hitbox.Enable(damage, offset, hitboxSize, knockback, facing);

        float t = 0;
        while (t < dashDuration)
        {
            ctx.rb.linearVelocity = new Vector2(dir.x * dashSpeed, ctx.rb.linearVelocity.y);
            t += Time.deltaTime;
            yield return null;
        }

        // 3) 정지 + 히트박스 OFF
        ctx.hitbox.Disable();
        ctx.rb.linearVelocity = new Vector2(0, ctx.rb.linearVelocity.y);

        yield return new WaitForSeconds(recoveryTime);
    }
}
