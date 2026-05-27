using System.Collections;
using System.Linq;
using UnityEngine;
using static UnityEngine.UI.Image;
using Random = UnityEngine.Random;
public class TeleportMove : BossMoveBehavior
{
    public TeleportMove(LayerMask layerMask) : base(layerMask)
    {
        
    }

    public override IEnumerator Execute(BossContext ctx, PatternType patternType, float minDistance, float maxDistance)
    {
        
        ctx.animator.SetTrigger("Spin");
        yield return ctx.WaitForAnimEvent("TelegraphEnd");
        //Debug.Log($"[Teleport] groundLayer = {string.Join(", ", Enumerable.Range(0, 32).Where(i => (groundLayer.value & (1 << i)) != 0).Select(i => LayerMask.LayerToName(i)))}");
        Vector2 target;
        switch (patternType)
        {
            case PatternType.Random:
                target = ctx.bossRoom.RandomPoint();
                break;
            case PatternType.Melee:
                {
                    target = ctx.bossRoom.RandomPointAwayFrom(ctx.playerTransform.position, minDistance, maxDistance);
                    RaycastHit2D hit = Physics2D.Raycast(
                        target,
                        Vector2.down,
                        100f,
                        groundLayer
                    );
                    target = hit.point;
                    break;
                }
            case PatternType.Ranged:
                target = ctx.bossRoom.RandomPointAwayFrom(ctx.playerTransform.position, minDistance, maxDistance);
                break;
            case PatternType.Dash:
                {
                    Vector2 origin = ctx.bossRoom.RandomPointAwayFrom(ctx.playerTransform.position, minDistance, maxDistance);
                    target.y = ctx.playerTransform.position.y;
                    float wallOffset = Mathf.Max(1.5f, ctx.bossTransform.GetComponent<Collider2D>().bounds.extents.x + 0.1f);
                    Vector2 dir = Random.value >= 0.5f ? Vector2.right : Vector2.left;
                    RaycastHit2D hit = Physics2D.Raycast(
                        ctx.playerTransform.position,
                        dir,
                        100f,
                        groundLayer
                    );
                    if (hit.collider != null)
                        target.x = hit.point.x - dir.x * wallOffset;
                    else
                        target = origin;
                    break;
                }
            default:
                target = ctx.bossRoom.RandomPoint();
                break;
        }
        
        ctx.bossTransform.position = target;
        ctx.AllFlip();
        yield return ctx.WaitForAnimEvent("RecoveryEnd");
    }
}
