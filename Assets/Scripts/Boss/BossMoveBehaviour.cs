using System.Collections;
using UnityEngine;

public abstract class BossMoveBehavior
{
    protected LayerMask groundLayer;
    public BossMoveBehavior(LayerMask layerMask)
    {
        groundLayer = layerMask;
    }
    public abstract IEnumerator Execute(BossContext ctx, PatternType patternType, float minDistance, float maxDistance);
}