using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ThunderBoltPattern", menuName = "BossPatterns/ThunderBoltPattern")]
public class ThunderBoltPattern : BossPattern
{
    [SerializeField] private Beam BeamPrefab;
    [SerializeField] private Vector2 muzzleOffset;
    [SerializeField] private string[] animStates;
    public override IEnumerator Execute(BossContext ctx)
    {
        ctx.animator.Play(animStates[0]);
        yield return ctx.WaitForAnimEvent("TelegraphEnd");

        ctx.animator.Play(animStates[1]);
        yield return new WaitForSeconds(2f);

        ctx.animator.Play(animStates[2]);
        yield return ctx.WaitForAnimEvent("ProjectileFire");

        ctx.animator.Play(animStates[3]);
        Vector2 origin = (Vector2)ctx.muzzle.position + muzzleOffset;
        var beam = Instantiate(BeamPrefab, origin, Quaternion.identity, ctx.bossTransform);
        yield return beam.WaitForAnimEvent("ProjectileFire");
        beam.BeamEnable();
        ctx.bossTransform.GetComponent<Rigidbody2D>().linearVelocityX = 1f;
        yield return new WaitForSeconds(5f);

        ctx.animator.Play(animStates[4]);
        beam.BeamDisable();
        yield return beam.WaitForAnimEvent("ProjectileFireEnd");

        ctx.bossTransform.GetComponent<Rigidbody2D>().linearVelocityX = 0f;
        yield return ctx.WaitForAnimEvent("RecoveryEnd");
        Destroy(beam.gameObject);
    }
}
