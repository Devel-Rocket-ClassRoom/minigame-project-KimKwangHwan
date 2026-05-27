using System.Collections;
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


    public override IEnumerator Execute(BossContext ctx)
    {
        ctx.AllFlip();
        ctx.animator.Play(animStates[0]);
        yield return ctx.WaitForAnimEvent("TelegraphEnd");
        ctx.animator.Play(animStates[1]);
        yield return new WaitForSeconds(1f);
        ctx.animator.Play(animStates[2]);
        yield return ctx.WaitForAnimEvent("ProjectileFire");
        ctx.AllFlip();
        Vector2 origin = (Vector2)ctx.muzzle.position + muzzleOffset;
        bool playerIsRight = ctx.PlayerIsRight;
        Vector2 dir = ctx.DirToPlayer;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        var proj = PoolManager.Instance.Spawn(projectilePrefab.gameObject, origin, Quaternion.Euler(0, 0, angle));
        //if (proj.TryGetComponent<SpriteRenderer>(out var sr))
        //{
        //    sr.flipX = !playerIsRight;
        //}
        proj.GetComponent<HomingProjectile>().LaunchHoming(dir.normalized, projectileSpeed, projectileDamage, dir.x, ctx.playerTransform);
        yield return ctx.WaitForAnimEvent("RecoveryEnd");
        yield return new WaitForSeconds(recoveryTime);
    }
}
