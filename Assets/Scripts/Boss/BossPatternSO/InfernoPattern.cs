using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "InfernoPattern", menuName = "BossPatterns/InfernoPattern")]
public class InfernoPattern : BossPattern
{
    [SerializeField] private Stationary stationaryPrefab;
    [SerializeField] private int infernoCount = 5;
    [SerializeField] private float infernoTime = 0.5f;
    [SerializeField] private string animState;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Coroutine coInferno = null;

    public AudioClip castClip;

    public override IEnumerator Execute(BossContext ctx)
    {
        ctx.AllFlip();
        ctx.animator.Play(animState);
        yield return ctx.WaitForAnimEvent("TelegraphEnd");
        SFXManager.Instance.PlaySFX(castClip);
        Vector2 divePos = new Vector2(ctx.bossTransform.position.x, ctx.bossTransform.position.y);
        if (coInferno != null) ctx.bossTransform.GetComponent<Witch>().StopCoroutine(coInferno);
        coInferno = ctx.bossTransform.GetComponent<Witch>().StartCoroutine(CoInferno(ctx));
        yield return new WaitForSeconds(recoveryTime);
    }
    public IEnumerator CoInferno(BossContext ctx)
    {
        for (int j = 0; j < infernoCount; j++)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                ctx.bossRoom.RandomPoint(),
                Vector2.down,
                100f,
                groundLayer
            );
            PoolManager.Instance.Spawn(stationaryPrefab.gameObject, hit.point, Quaternion.identity);

            yield return new WaitForSeconds(infernoTime);
        }
    }
}
