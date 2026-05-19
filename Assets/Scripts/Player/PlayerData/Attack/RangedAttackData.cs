using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(menuName = "Combat/Ranged Attack")]
public sealed class RangedAttackData : AttackData
{
    public Projectile projectilePrefab;
    public Vector2 muzzleOffset;
    public float projectileSpeed = 12f;
    public float projectileDamage = 8f;
    public int shotCount = 1;
    public float spreadDegrees = 0f;
    [SerializeField] private GameObject muzzleEffectPrefab;
    public override void OnActiveEnd(AttackRuntime rt)
    {
    }

    public override void OnActiveStart(AttackRuntime rt)
    {
        if (projectilePrefab == null) return;

        Vector3 origin = rt.MuzzleOrigin.position + new Vector3(muzzleOffset.x * rt.Facing, muzzleOffset.y, 0f);
        var effect = Object.Instantiate(muzzleEffectPrefab, origin, Quaternion.identity);
        effect.transform.localScale = new Vector3(rt.Facing > 0f ? 1f : -1f, effect.transform.localScale.y, effect.transform.localScale.z);
        for (int i = 0; i < shotCount; i++)
        {
            float t = shotCount == 1 ? 0f : (i / (float)(shotCount - 1)) - 0.5f;
            float angle = t * spreadDegrees;

            Vector2 dir = Quaternion.Euler(0, 0, angle) * new Vector2(rt.Facing, 0f);

            var proj = Object.Instantiate(projectilePrefab, origin, Quaternion.identity);
            proj.GetComponent<SpriteRenderer>().flipX = rt.Facing < 0f;
            proj.Launch(dir.normalized, projectileSpeed, projectileDamage, rt.Facing);
        }
    }

    public override void OnEnter(AttackRuntime rt)
    {
    }

    public override void OnExit(AttackRuntime rt)
    {
    }
}
