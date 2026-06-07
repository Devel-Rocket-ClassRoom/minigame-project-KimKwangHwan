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

    public AudioClip castClip;

    public override IEnumerator Execute(BossContext ctx)
    {
        bool playerIsRight = ctx.PlayerIsRight;
        ctx.animator.Play(fireAnimState);
        yield return ctx.WaitForAnimEvent("ProjectileFire");
        SFXManager.Instance.PlaySFX(castClip);
        ctx.AllFlip();
        // Vector2 dir = new Vector2(playerIsRight ? 1f : -1f, 0f);
        Vector2 origin = (Vector2)ctx.muzzle.position + muzzleOffset;
        Vector2 dir = ctx.DirToPlayer;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        var proj = PoolManager.Instance.Spawn(projectilePrefab.gameObject, origin, Quaternion.Euler(0, 0, angle));
        //if (proj.TryGetComponent<SpriteRenderer>(out var sr))
        //{
        //    sr.flipX = !playerIsRight;
        //}
        proj.GetComponent<Projectile>().Launch(dir.normalized, projectileSpeed, projectileDamage, dir.x);
        yield return new WaitForSeconds(recoveryTime);
    }
}