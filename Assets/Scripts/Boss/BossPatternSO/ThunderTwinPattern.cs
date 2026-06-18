using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ThunderTwinPattern", menuName = "BossPatterns/ThunderTwinPattern")]
public class ThunderTwinPattern : BossPattern
{
    [Header("Projectile")]
    [SerializeField] private HomingProjectile projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileDamage = 30f;
    [SerializeField] private float projectileLifetime = 2f;
    [SerializeField] private Vector2 muzzleOffset;
    [SerializeField] private string[] animStates;

    public AudioClip chargingClip;
    public AudioClip launchClip;

    public override async UniTask Execute(BossContext ctx, CancellationToken ct = default)
    {
        ctx.AllFlip();
        ctx.animator.Play(animStates[0]);
        await ctx.WaitForAnimEvent("TelegraphEnd", ct: ct);
        ctx.animator.Play(animStates[1]);
        SFXManager.Instance.PlaySFX(chargingClip);
        await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: ct);
        ctx.animator.Play(animStates[2]);
        await ctx.WaitForAnimEvent("ProjectileFire", ct: ct);
        ctx.AllFlip();
        Vector2 origin = (Vector2)ctx.muzzle.position + muzzleOffset;
        bool playerIsRight = ctx.PlayerIsRight;
        Vector2 dir = ctx.DirToPlayer;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        var proj = PoolManager.Instance.Spawn(projectilePrefab.gameObject, origin, Quaternion.Euler(0, 0, angle));
        SFXManager.Instance.PlaySFX(launchClip);
        proj.GetComponent<HomingProjectile>().LaunchHoming(dir.normalized, projectileSpeed, projectileDamage, dir.x, ctx.playerTransform);
        await ctx.WaitForAnimEvent("RecoveryEnd", ct: ct);
        await UniTask.Delay(TimeSpan.FromSeconds(recoveryTime), cancellationToken: ct);
    }
}
