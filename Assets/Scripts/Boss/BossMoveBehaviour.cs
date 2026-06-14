using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class BossMoveBehavior
{
    protected LayerMask groundLayer;
    public BossMoveBehavior(LayerMask layerMask)
    {
        groundLayer = layerMask;
    }
    public abstract UniTask Execute(BossContext ctx, PatternType patternType, float minDistance, float maxDistance, CancellationToken ct);
}