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
        bool playerIsRight = ctx.PlayerIsRight;
        ctx.animator.Play(fireAnimState);
        yield return ctx.WaitForAnimEvent("ProjectileFire");
        Vector2 dir = ctx.AllFlip();
        // Vector2 dir = new Vector2(playerIsRight ? 1f : -1f, 0f);
        Vector2 origin = (Vector2)ctx.muzzle.position + muzzleOffset;
        var proj = PoolManager.Instance.Spawn(projectilePrefab.gameObject, origin, Quaternion.identity);
        if (proj.TryGetComponent<SpriteRenderer>(out var sr))
        {
            sr.flipX = !playerIsRight;
        }
        proj.GetComponent<Projectile>().Launch(dir.normalized, projectileSpeed, projectileDamage, dir.x);
        yield return new WaitForSeconds(recoveryTime);
    }
}