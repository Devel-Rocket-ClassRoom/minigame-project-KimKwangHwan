using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "FireAttackPattern", menuName = "BossPatterns/FireBallPattern")]
public class FireBallPattern : BossPattern
{
    [Header("Projectile")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileDamage = 8f;
    [SerializeField] private float projectileLifetime = 2f;
    [SerializeField] private Vector2 muzzleOffset;
    [SerializeField] private string fireAnimState;

    public AudioClip castClip;

    public override async UniTask Execute(BossContext ctx, CancellationToken ct = default)
    {
        ctx.animator.Play(fireAnimState);
        await ctx.WaitForAnimEvent("ProjectileFire", ct: ct);
        SFXManager.Instance.PlaySFX(castClip);
        ctx.AllFlip();
        Vector2 origin = (Vector2)ctx.muzzle.position + muzzleOffset;
        Vector2 dir = ctx.DirToPlayer;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        var proj = PoolManager.Instance.Spawn(projectilePrefab.gameObject, origin, Quaternion.Euler(0, 0, angle));
        proj.GetComponent<Projectile>().Launch(dir.normalized, projectileSpeed, projectileDamage, dir.x);
        await UniTask.Delay(TimeSpan.FromSeconds(recoveryTime), cancellationToken: ct);
    }
}
