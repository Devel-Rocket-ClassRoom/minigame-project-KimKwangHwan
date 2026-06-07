using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Enemy/Attack/ArcProjectile")]
public class ArcProjectileAttack : EnemyAttackPattern
{
    [Header("투사체")]
    public float recoveryTime = 0.3f;
    public ArcProjectile projectilePrefab;
    public float damage = 10f;
    public float speed = 10f;

    [Header("발사 패턴")]
    public int projectileCount = 1;
    public float spreadRadius = 0.5f;   // 착지 위치 분산 반경 (0이면 정확히 타겟)
    public float intervalBetweenShots = 0f;

    [Header("거리 기반 각도 조정")]
    [Tooltip("이 수평 거리 이하이거나 플레이어가 아래에 있으면 저각으로 발사")]
    public float closeRangeThreshold = 4f;

    [Header("애니메이션")]
    public string animTrigger = "Shoot";
    public string animOutTrigger = "ShootStop";
    public AudioClip launchClip;
    public override IEnumerator Execute(EnemyContext ctx)
    {
        ctx.SuperArmor = true;
        ctx.anim.ResetTrigger(animTrigger);
        ctx.anim.SetTrigger(animTrigger);
        yield return ctx.WaitForAnimEvent("ProjectileFire");
        SFXManager.Instance.PlaySFX(launchClip);
        Vector2 muzzlePos = ctx.muzzle.position;
        float rawDx = ctx.target.position.x - muzzlePos.x;
        float dy = ctx.target.position.y - muzzlePos.y;
        bool useLowArc = Mathf.Abs(rawDx) < closeRangeThreshold || dy < 0f;

        for (int i = 0; i < projectileCount; i++)
        {
            Vector2 landPos = ctx.target.position;
            if (spreadRadius > 0f)
                landPos += Random.insideUnitCircle * spreadRadius;

            var go = PoolManager.Instance.Spawn(projectilePrefab.gameObject, muzzlePos, Quaternion.identity);
            go.GetComponent<ArcProjectile>().LaunchArc(muzzlePos, landPos, speed, damage, useLowArc);

            if (intervalBetweenShots > 0f && i < projectileCount - 1)
                yield return new WaitForSeconds(intervalBetweenShots);
        }

        if (animOutTrigger != string.Empty)
        {
            ctx.anim.ResetTrigger(animOutTrigger);
            ctx.anim.SetTrigger(animOutTrigger);
        }
        ctx.SuperArmor = false;
        yield return ctx.WaitForAnimEvent("RecoveryEnd");
        yield return new WaitForSeconds(recoveryTime);
    }
}
