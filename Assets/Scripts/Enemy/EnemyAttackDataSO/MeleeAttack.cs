using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

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

    [Header("점프")]
    public bool useJump = false;
    [Range(0f, 89f)] public float jumpAngle = 30f;  // 지면 기준 발사각
    public float jumpSpeed = 8f;

    [Header("애니메이션")]
    public string animTrigger = "Attack";

    public AudioClip sfxClip;
    public AudioClip jumpClip;
    public AudioClip hitClip;

    public override async UniTask Execute(EnemyContext ctx, CancellationToken ct)
    {
        ctx.SuperArmor = true;
        ctx.anim.SetTrigger(animTrigger);
        var motor = ctx.self.GetComponent<EnemyMotor>();
        if (useJump)
        {
            motor.SuspendControl();
            DoJump(ctx);
            SFXManager.Instance.PlaySFX(jumpClip);
        }
        //var hitboxOff = ctx.ArmEvent("HitboxOff");
        await ctx.WaitForAnimEvent("HitboxOn", ct);
        SFXManager.Instance.PlaySFX(sfxClip);
        float facing = ctx.Facing;
        Vector2 offset = new(hitboxOffset.x, hitboxOffset.y);

        ctx.hitbox.Enable(damage, offset, hitboxSize, knockback, facing, hitClip);
        await ctx.WaitForAnimEvent("HitboxOff", ct);
        ctx.hitbox.Disable();

        if (useJump)
        {
            motor.ResumeControl();
        }
        motor.MoveStop();
        ctx.SuperArmor = false;
        await ctx.WaitForAnimEvent("RecoveryEnd", ct);
        await UniTask.Delay((int)(recoveryTime * 1000), cancellationToken: ct);
    }
    void DoJump(EnemyContext ctx)
    {
        // target이 없으면 현재 facing 방향으로 점프
        float dirX = ctx.target != null
            ? Mathf.Sign(ctx.target.position.x - ctx.self.position.x)
            : Mathf.Sign(ctx.Facing);

        float rad = jumpAngle * Mathf.Deg2Rad;
        Vector2 v = new(Mathf.Cos(rad) * jumpSpeed * dirX,
                        Mathf.Sin(rad) * jumpSpeed);

        ctx.rb.linearVelocity = v;  // 구버전 Unity는 ctx.rb.velocity
    }
}
