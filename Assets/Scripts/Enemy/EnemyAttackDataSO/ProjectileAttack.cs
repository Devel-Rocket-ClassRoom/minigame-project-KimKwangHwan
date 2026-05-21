using UnityEngine;
using System.Collections;

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
    public float spreadAngle = 60f;
    public bool aimAtTarget = true;

    [Header("애니메이션")]
    public string animTrigger = "Shoot";

    public override IEnumerator Execute(EnemyContext ctx)
    {
        ctx.anim.SetTrigger(animTrigger);
        yield return new WaitForSeconds(windupTime);

        float facing = ctx.Facing;
        Vector2 center = aimAtTarget
            ? ((Vector2)(ctx.target.position - ctx.self.position)).normalized
            : new Vector2(facing, 0f);

        float baseAngle = Mathf.Atan2(center.y, center.x) * Mathf.Rad2Deg;
        float step = projectileCount > 1 ? spreadAngle / (projectileCount - 1) : 0f;
        float startAngle = baseAngle - spreadAngle * 0.5f;

        for (int i = 0; i < projectileCount; i++)
        {
            float ang = (projectileCount == 1 ? baseAngle : startAngle + step * i) * Mathf.Deg2Rad;
            Vector2 dir = new(Mathf.Cos(ang), Mathf.Sin(ang));

            var proj = Object.Instantiate(projectilePrefab, ctx.self.position, Quaternion.identity);
            proj.Launch(dir, speed, damage, facing);

            // 마지막 발 아니면 대기
            if (intervalBetweenShots > 0f && i < projectileCount - 1)
                yield return new WaitForSeconds(intervalBetweenShots);
        }

        yield return new WaitForSeconds(recoveryTime);
    }
}