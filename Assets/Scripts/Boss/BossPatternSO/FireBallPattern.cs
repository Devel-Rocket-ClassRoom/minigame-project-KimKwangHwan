using System.Collections;
using UnityEngine;
using static UnityEngine.UI.Image;

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

    public override IEnumerator Execute(BossContext ctx)
    {
        ctx.animator.Play(fireAnimState);
        yield return ctx.WaitForAnimEvent("ProjectileFire");
        Vector2 dir = new Vector2(1f, 0f);
        Vector2 origin = (Vector2)ctx.muzzle.position + muzzleOffset;
        var proj = Object.Instantiate(projectilePrefab, origin, Quaternion.identity);
        if (proj.TryGetComponent<SpriteRenderer>(out var sr))
            sr.flipX = 1f < 0f;
        proj.Launch(dir.normalized, projectileSpeed, projectileDamage, 1f);
        yield return new WaitForSeconds(recoveryTime);
    }
}