using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "CometDivePattern", menuName = "BossPatterns/CometDivePattern")]
public class CometDivePattern : BossPattern
{
    [SerializeField] private Stationary stationaryPrefab;
    [SerializeField] private int infernoCount = 5;
    [SerializeField] private float infernoInterval = 1f;
    [SerializeField] private float infernoTime = 0.3f;

    [SerializeField] private float diveSpeed = 10f;

    [SerializeField] private string[] sequenceAnimStates;
    [SerializeField] private Vector2[] hitboxSizes;
    [SerializeField] private Vector2[] hitboxOffsets;
    [SerializeField] private float damage = 10f;

    [SerializeField] private int hitCount = 3;
    [SerializeField] private float interHitDelay = 0.2f;

    public AudioClip upClip;
    public AudioClip diveClip;
    public AudioClip impactClip;
    public AudioClip diveHitClip;
    public AudioClip impactHitClip;

    public override async UniTask Execute(BossContext ctx, CancellationToken ct = default)
    {
        var rb = ctx.bossTransform.GetComponent<Rigidbody2D>();
        for (int i = 0; i < hitCount; i++)
        {
            ctx.bossTransform.position = new Vector2(ctx.PlayerPos.x, ctx.bossRoom.Max.y);
            ctx.animator.Play(sequenceAnimStates[0]);
            rb.gravityScale = 0f;
            SFXManager.Instance.PlaySFX(upClip);
            await ctx.WaitForAnimEvent("TelegraphEnd", ct: ct);
            ctx.animator.Play(sequenceAnimStates[1]);
            rb.gravityScale = 1f;
            rb.linearVelocity = Vector2.down * diveSpeed;
            await ctx.WaitForAnimEvent("HitboxOn", ct: ct);
            SFXManager.Instance.PlaySFX(diveClip);
            ctx.hitbox.Enable(damage, hitboxOffsets[0], hitboxSizes[0], 0f, 1f, diveHitClip);
            await UniTask.WaitUntil(() => ctx.bossTransform.GetComponent<Witch>().IsGrounded, cancellationToken: ct);
            await ctx.WaitForAnimEvent("HitboxOff", ct: ct);
            ctx.hitbox.Disable();

            ctx.animator.Play(sequenceAnimStates[2]);
            rb.linearVelocity = Vector2.zero;
            Vector2 divePos = new Vector2(ctx.bossTransform.position.x, ctx.bossTransform.position.y);
            CoInferno(divePos, ct).Forget();
            SFXManager.Instance.PlaySFX(impactClip);
            await ctx.WaitForAnimEvent("HitboxOn", ct: ct);
            ctx.hitbox.Enable(damage, hitboxOffsets[1], hitboxSizes[1], 0f, 1f, impactHitClip);
            await ctx.WaitForAnimEvent("HitboxOff", ct: ct);
            ctx.hitbox.Disable();
            await ctx.WaitForAnimEvent("RecoveryEnd", ct: ct);
            await UniTask.Delay(TimeSpan.FromSeconds(interHitDelay), cancellationToken: ct);
        }
        rb.gravityScale = 0f;
        await UniTask.Delay(TimeSpan.FromSeconds(recoveryTime), cancellationToken: ct);
    }

    private async UniTask CoInferno(Vector2 pos, CancellationToken ct)
    {
        for (int j = 0; j < infernoCount; j++)
        {
            PoolManager.Instance.Spawn(stationaryPrefab.gameObject, new Vector2(pos.x + (j + 1) * infernoInterval, pos.y), Quaternion.identity);
            PoolManager.Instance.Spawn(stationaryPrefab.gameObject, new Vector2(pos.x + -(j + 1) * infernoInterval, pos.y), Quaternion.identity);
            await UniTask.Delay(TimeSpan.FromSeconds(infernoTime), cancellationToken: ct);
        }
    }
}
