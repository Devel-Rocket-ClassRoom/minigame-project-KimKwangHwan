using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ThunderStrikePattern", menuName = "BossPatterns/ThunderStrikePattern")]
public class ThunderStrikePattern : BossPattern
{
    [SerializeField] private Stationary stationaryPrefab;
    [SerializeField] private string[] animStates;
    [SerializeField] private int thunderCount = 8;
    [SerializeField] private float thunderTime = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    public AudioClip chargingClip;
    public AudioClip castingClip;

    public override async UniTask Execute(BossContext ctx, CancellationToken ct = default)
    {
        bool playerIsRight = ctx.PlayerIsRight;
        ctx.AllFlip();
        ctx.animator.Play(animStates[0]);
        await ctx.WaitForAnimEvent("TelegraphEnd", ct: ct);

        ctx.animator.Play(animStates[1]);
        SFXManager.Instance.PlaySFX(chargingClip);
        await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: ct);

        ctx.animator.Play(animStates[2]);
        await ctx.WaitForAnimEvent("TelegraphEnd", ct: ct);
        ctx.animator.Play(animStates[3]);
        ctx.AllFlip();
        SFXManager.Instance.PlaySFX(castingClip);
        for (int i = 0; i < thunderCount; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                ctx.playerTransform.position,
                Vector2.down,
                100f,
                groundLayer
            );
            var stationary = PoolManager.Instance.Spawn(stationaryPrefab.gameObject, hit.point, Quaternion.identity);
            if (stationary.TryGetComponent<SpriteRenderer>(out var sr))
            {
                sr.flipX = !playerIsRight;
            }
            await UniTask.Delay(TimeSpan.FromSeconds(thunderTime), cancellationToken: ct);
        }
        ctx.animator.Play(animStates[4]);
        await ctx.WaitForAnimEvent("RecoveryEnd", ct: ct);
        await UniTask.Delay(TimeSpan.FromSeconds(recoveryTime), cancellationToken: ct);
    }
}
