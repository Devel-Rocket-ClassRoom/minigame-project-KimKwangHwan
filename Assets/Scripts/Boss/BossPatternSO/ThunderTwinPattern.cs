using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ThunderTwinPattern", menuName = "BossPatterns/ThunderTwinPattern")]
public class ThunderTwinPattern : BossPattern
{
    [Header("Projectile")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileDamage = 30f;
    [SerializeField] private float projectileLifetime = 2f;
    [SerializeField] private Vector2 muzzleOffset;
    [SerializeField] private string[] animStates;


    public override IEnumerator Execute(BossContext ctx)
    {
        ctx.animator.Play(animStates[0]);
        yield return ctx.WaitForAnimEvent("TelegraphEnd");
        ctx.animator.Play(animStates[1]);
        yield return new WaitForSeconds(1f);
        ctx.animator.Play(animStates[2]);
        yield return ctx.WaitForAnimEvent("ProjectileFire");
        Vector2 dir = new Vector2(1f, 0f);
        Vector2 origin = (Vector2)ctx.muzzle.position + muzzleOffset;
        var proj = Instantiate(projectilePrefab, origin, Quaternion.identity);
        if (proj.TryGetComponent<SpriteRenderer>(out var sr))
            sr.flipX = 1f < 0f;
        proj.Launch(dir.normalized, projectileSpeed, projectileDamage, 1f);
        yield return ctx.WaitForAnimEvent("RecoveryEnd");
        yield return new WaitForSeconds(recoveryTime);
    }
}
