using Mono.Cecil;
using System.Collections;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using static UnityEngine.UI.Image;

[CreateAssetMenu(fileName = "ThunderStrikePattern", menuName = "BossPatterns/ThunderStrikePattern")]
public class ThunderStrikePattern : BossPattern
{
    [SerializeField] private Stationary stationaryPrefab;
    [SerializeField] private string[] animStates;
    [SerializeField] private int thunderCount = 8;
    [SerializeField] private float thunderTime = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    public override IEnumerator Execute(BossContext ctx)
    {
        bool playerIsRight = ctx.PlayerIsRight;
        ctx.AllFlip();
        ctx.animator.Play(animStates[0]);
        yield return ctx.WaitForAnimEvent("TelegraphEnd");

        ctx.animator.Play(animStates[1]);
        yield return new WaitForSeconds(2f);

        ctx.animator.Play(animStates[2]);
        yield return ctx.WaitForAnimEvent("TelegraphEnd");
        ctx.animator.Play(animStates[3]);
        Vector2 dir = ctx.AllFlip();
        for (int i = 0; i < thunderCount; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                ctx.playerTransform.position,
                Vector2.down,
                100f,
                groundLayer
            );
            var stationary = PoolManager.Instance.Spawn(stationaryPrefab.gameObject, hit.point, Quaternion.identity);
            if (stationary.TryGetComponent<SpriteRenderer>(out var sr))
            {
                sr.flipX = !playerIsRight;
            }
            yield return new WaitForSeconds(thunderTime);
        }
        
        ctx.animator.Play(animStates[4]);
        yield return ctx.WaitForAnimEvent("RecoveryEnd");
    }
}
