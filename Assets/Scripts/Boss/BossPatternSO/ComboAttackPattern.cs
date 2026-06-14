using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ComboAttackPattern", menuName = "BossPatterns/ComboAttackPattern")]
public class ComboAttackPattern : BossPattern
{
    [Header("Combo")]
    [SerializeField] private int phase1HitCount = 2;
    [SerializeField] private int phase2HitCount = 3;
    [SerializeField] private float interHitDelay = 0.2f;

    [Header("Hits")]
    [SerializeField] private string[] hitAnimStates;
    [SerializeField] private Vector2[] hitboxSizes;
    [SerializeField] private Vector2[] hitboxOffsets;
    [SerializeField] private float damage = 10f;

    public AudioClip[] attackClip;
    public AudioClip[] hitClip;

    public override async UniTask Execute(BossContext ctx, CancellationToken ct = default)
    {
        int hitCount = ctx.currentPhase == 1 ? phase1HitCount : phase2HitCount;
        Rigidbody2D rb = ctx.bossTransform.GetComponent<Rigidbody2D>();
        for (int i = 0; i < hitCount; i++)
        {
            int index = Mathf.Min(i, hitAnimStates.Length - 1);
            string clip = hitAnimStates[index];
            ctx.animator.Play(clip);
            SFXManager.Instance.PlaySFX(attackClip[index]);
            await ctx.WaitForAnimEvent("HitboxOn", ct: ct);
            Vector2 dir = ctx.AllFlip();
            ctx.hitbox.Enable(damage, hitboxOffsets[index], hitboxSizes[index], 0f, 1f, hitClip[index]);
            rb.linearVelocityX = dir.x * 5f;
            await ctx.WaitForAnimEvent("HitboxOff", ct: ct);
            ctx.hitbox.Disable();
            rb.linearVelocityX = 0f;
            if (i < hitCount - 1)
                await UniTask.Delay(TimeSpan.FromSeconds(interHitDelay), cancellationToken: ct);
        }
        await UniTask.Delay(TimeSpan.FromSeconds(recoveryTime), cancellationToken: ct);
    }
}
