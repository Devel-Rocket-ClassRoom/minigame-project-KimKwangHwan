using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(menuName = "Combat/Ranged Attack")]
public sealed class RangedAttackData : AttackData
{
    public enum FireMode
    {
        Straight,       // 1발: 정직선. N발: spreadDegrees 안에 부채꼴 균등 분배.
        RandomSpread,   // shotCount만큼을 ±spreadDegrees/2 안에서 각각 랜덤 각도로.
    }
    [Header("Projectile")]
    public Projectile projectilePrefab;
    public Vector2 muzzleOffset;
    public float projectileSpeed = 12f;
    public float projectileDamage = 8f;
    [Header("Pattern")]
    public FireMode fireMode = FireMode.Straight;
    public int shotCount = 1;
    public float spreadDegrees = 0f;
    [Header("VFX")]
    [SerializeField] private GameObject muzzleEffectPrefab;
    public AudioClip sfxClip;
    public override void OnActiveStart(AttackRuntime rt)
    {
        if (projectilePrefab == null) return;

        Vector3 origin = rt.MuzzleOrigin.position + new Vector3(muzzleOffset.x * rt.Facing, muzzleOffset.y, 0f);

        SpawnMuzzleEffect(origin, rt.Facing);

        switch (fireMode)
        {
            case FireMode.Straight: FireStraight(rt, origin); break;
            case FireMode.RandomSpread: FireRandomSpread(rt, origin); break;
        }
    }

    private void FireStraight(AttackRuntime rt, Vector3 origin)
    {
        for (int i = 0; i < shotCount; ++i)
        {
            float t = shotCount == 1 ? 0f : (i / (float)(shotCount - 1)) - 0.5f;
            float angle = t * spreadDegrees;
            SpawnProjectile(rt, origin, angle);
        }
    }
    void FireRandomSpread(AttackRuntime rt, Vector3 origin)
    {
        float half = spreadDegrees * 0.5f;
        for (int i = 0; i < shotCount; i++)
        {
            float angle = Random.Range(-half, half);
            SpawnProjectile(rt, origin, angle);
        }
    }

    private void SpawnProjectile(AttackRuntime rt, Vector3 origin, float angleOffsetDeg)
    {
        Vector2 dir = Quaternion.Euler(0, 0, angleOffsetDeg) * new Vector2(rt.Facing, 0f);

        // var proj = Object.Instantiate(projectilePrefab, origin, Quaternion.identity);
        var proj = PoolManager.Instance.Spawn(projectilePrefab.gameObject, origin, Quaternion.identity);
        if (proj.TryGetComponent<SpriteRenderer>(out var sr))
            sr.flipX = rt.Facing < 0f;
        proj.GetComponent<Projectile>().Launch(dir.normalized, projectileSpeed, rt.AttackPower * projectileDamage, rt.Facing);
    }
    private void SpawnMuzzleEffect(Vector3 origin, float facing)
    {
        if (muzzleEffectPrefab == null) return;
        var effect = Object.Instantiate(muzzleEffectPrefab, origin, Quaternion.identity);
        SFXManager.Instance.PlaySFX(sfxClip);
        var s = effect.transform.localScale;
        s.x = facing > 0f ? Mathf.Abs(s.x) : -Mathf.Abs(s.x);
        effect.transform.localScale = s;
    }

    public override void OnEnter(AttackRuntime rt)
    {
    }

    public override void OnActiveEnd(AttackRuntime rt)
    {
    }

    public override void OnExit(AttackRuntime rt)
    {
    }
}
