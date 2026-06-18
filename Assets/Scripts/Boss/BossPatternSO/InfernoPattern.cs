using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "InfernoPattern", menuName = "BossPatterns/InfernoPattern")]
public class InfernoPattern : BossPattern
{
    [SerializeField] private Stationary stationaryPrefab;
    [SerializeField] private int infernoCount = 5;
    [SerializeField] private float infernoTime = 0.5f;
    [SerializeField] private string animState;
    [SerializeField] private LayerMask groundLayer;

    private CancellationTokenSource _infernoCts;

    public AudioClip castClip;

    public override async UniTask Execute(BossContext ctx, CancellationToken ct = default)
    {
        ctx.AllFlip();
        ctx.animator.Play(animState);
        await ctx.WaitForAnimEvent("TelegraphEnd", ct: ct);
        SFXManager.Instance.PlaySFX(castClip);

        _infernoCts?.Cancel();
        _infernoCts?.Dispose();
        _infernoCts = new CancellationTokenSource();
        CoInferno(ctx, _infernoCts.Token).Forget();

        await UniTask.Delay(TimeSpan.FromSeconds(recoveryTime), cancellationToken: ct);
    }

    private async UniTask CoInferno(BossContext ctx, CancellationToken ct)
    {
        for (int j = 0; j < infernoCount; j++)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                ctx.bossRoom.RandomPoint(),
                Vector2.down,
                100f,
                groundLayer
            );
            PoolManager.Instance.Spawn(stationaryPrefab.gameObject, hit.point, Quaternion.identity);
            await UniTask.Delay(TimeSpan.FromSeconds(infernoTime), cancellationToken: ct);
        }
    }
}
