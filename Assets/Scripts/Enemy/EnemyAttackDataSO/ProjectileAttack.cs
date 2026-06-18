using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

[CreateAssetMenu(menuName = "Enemy/Attack/Projectile")]
public class ProjectileAttack : EnemyAttackPattern
{
    [Header("타이밍")]
    public float windupTime = 0.4f;
    public float recoveryTime = 0.3f;
    public float intervalBetweenShots = 0f;   // ← 이거 하나 추가. 0이면 동시, >0이면 순차

    [Header("투사체")]
    public Projectile projectilePrefab;
    public float speed = 8f;
    public float damage = 8f;

    [Header("발사 패턴")]
    public int projectileCount = 5;
    public int shootCount = 3;
    public float spreadAngle = 60f;
    public bool aimAtTarget = true;

    [Header("애니메이션")]
    public string animTrigger = "Shoot";
    public string animOutTrigger = "ShootStop";

    public AudioClip launchClip;
    public override async UniTask Execute(EnemyContext ctx, CancellationToken ct)
    {
        ctx.SuperArmor = true;
        ctx.anim.ResetTrigger(animTrigger);
        ctx.anim.SetTrigger(animTrigger);
        await ctx.WaitForAnimEvent("ProjectileFire", ct);
        SFXManager.Instance.PlaySFX(launchClip);
        float facing = ctx.Facing;
        Vector2 center = aimAtTarget
            ? ((Vector2)(ctx.target.position - ctx.self.position)).normalized
            : new Vector2(facing, 0f);

        float baseAngle = Mathf.Atan2(center.y, center.x) * Mathf.Rad2Deg;
        float step = projectileCount > 1 ? spreadAngle / (projectileCount - 1) : 0f;
        float startAngle = baseAngle - spreadAngle * 0.5f;
        // int instanceId = NextAttackInstanceId();
        for (int i = 0; i < projectileCount; i++)
        {
            //float ang = (projectileCount == 1 ? baseAngle : startAngle + step * i) * Mathf.Deg2Rad;
            for (int j = 0; j < shootCount; j++)
            {
                float ang = (baseAngle + Random.Range(-spreadAngle * 0.5f, spreadAngle * 0.5f)) * Mathf.Deg2Rad;
                Vector2 dir = new(Mathf.Cos(ang), Mathf.Sin(ang));
                var proj = PoolManager.Instance.Spawn(projectilePrefab.gameObject, ctx.muzzle.position, Quaternion.identity);
                proj.GetComponent<Projectile>().Launch(dir, speed, damage, facing);
            }

            // 마지막 발 아니면 대기
            if (intervalBetweenShots > 0f && i < projectileCount - 1)
                await UniTask.Delay((int)(intervalBetweenShots * 1000), cancellationToken: ct);
        }
        if (animOutTrigger != string.Empty)
        {
            ctx.anim.ResetTrigger(animOutTrigger);
            ctx.anim.SetTrigger(animOutTrigger);
        }
        ctx.SuperArmor = false;
        await ctx.WaitForAnimEvent("RecoveryEnd", ct);
        await UniTask.Delay((int)(recoveryTime * 1000), cancellationToken: ct);
    }
}