using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "DashPattern", menuName = "BossPatterns/DashPattern")]
public class DashPattern : BossPattern
{
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 2f;
    [SerializeField] private string[] animStates;
    [SerializeField] private Vector2 hitboxSize;
    [SerializeField] private Vector2 hitboxOffset;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float rehitInterval = 0.3f;

    public AudioClip dashClip;
    public AudioClip hitClip;

    public override async UniTask Execute(BossContext ctx, CancellationToken ct = default)
    {
        Vector2 dir = ctx.AllFlip();
        var rb = ctx.bossTransform.GetComponent<Rigidbody2D>();
        ctx.animator.Play(animStates[0]);

        await ctx.WaitForAnimEvent("HitboxOn", ct: ct);
        SFXManager.Instance.PlaySFX(dashClip);
        ctx.hitbox.Enable(damage, hitboxOffset, hitboxSize, 0f, 1f, hitClip, rehitInterval);
        ctx.animator.Play(animStates[1]);
        rb.linearVelocity = dir * dashSpeed;
        await UniTask.Delay(TimeSpan.FromSeconds(dashDuration), cancellationToken: ct);

        ctx.animator.Play(animStates[2]);
        await ctx.WaitForAnimEvent("HitboxOff", ct: ct);
        ctx.hitbox.Disable();

        await ctx.WaitForAnimEvent("RecoveryEnd", ct: ct);
        await UniTask.Delay(TimeSpan.FromSeconds(recoveryTime), cancellationToken: ct);
    }
}
