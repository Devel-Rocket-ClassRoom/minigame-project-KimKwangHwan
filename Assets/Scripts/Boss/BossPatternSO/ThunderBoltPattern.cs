using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ThunderBoltPattern", menuName = "BossPatterns/ThunderBoltPattern")]
public class ThunderBoltPattern : BossPattern
{
    [SerializeField] private Beam BeamPrefab;
    [SerializeField] private Vector2 muzzleOffset;
    [SerializeField] private string[] animStates;

    public override async UniTask Execute(BossContext ctx, CancellationToken ct = default)
    {
        ctx.animator.Play(animStates[0]);
        await ctx.WaitForAnimEvent("TelegraphEnd", ct: ct);

        ctx.animator.Play(animStates[1]);
        await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: ct);

        ctx.animator.Play(animStates[2]);
        await ctx.WaitForAnimEvent("ProjectileFire", ct: ct);

        ctx.animator.Play(animStates[3]);
        Vector2 origin = (Vector2)ctx.muzzle.position + muzzleOffset;
        var beam = Instantiate(BeamPrefab, origin, Quaternion.identity, ctx.bossTransform);
        await beam.WaitForAnimEvent("ProjectileFire", ct);
        beam.BeamEnable();
        ctx.bossTransform.GetComponent<Rigidbody2D>().linearVelocityX = 1f;
        await UniTask.Delay(TimeSpan.FromSeconds(5f), cancellationToken: ct);

        ctx.animator.Play(animStates[4]);
        beam.BeamDisable();
        await beam.WaitForAnimEvent("ProjectileFireEnd", ct);

        ctx.bossTransform.GetComponent<Rigidbody2D>().linearVelocityX = 0f;
        await ctx.WaitForAnimEvent("RecoveryEnd", ct: ct);
        Destroy(beam.gameObject);
    }
}
