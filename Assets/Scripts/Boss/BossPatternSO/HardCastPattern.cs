using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "HardCast", menuName = "BossPatterns/HardCast")]
public class HardCastPattern : BossPattern
{
    public override async UniTask Execute(BossContext ctx, CancellationToken ct = default)
    {
        throw new System.NotImplementedException();
    }
}
