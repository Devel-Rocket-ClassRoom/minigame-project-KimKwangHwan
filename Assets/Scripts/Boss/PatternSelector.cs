using System.Collections.Generic;
using UnityEngine;

public class PatternSelector
{
    private readonly List<BossPattern> _patterns;
    private readonly List<BossPattern> _candidates = new();  // 재사용용 버퍼

    public PatternSelector(List<BossPattern> patterns)
    {
        _patterns = patterns;
    }

    public BossPattern SelectNext(BossContext ctx)
    {
        _candidates.Clear();
        float now = Time.time;
        float distance = ctx.DistToPlayer;

        foreach (var p in _patterns)
        {
            if (!p.IsReady(now)) continue;
            if (p.unlockPhase > ctx.currentPhase) continue;
            if (distance < p.minDistance || distance > p.maxDistance) continue;
            _candidates.Add(p);
        }

        if (_candidates.Count == 0) return null;

        // 가중치 기반 랜덤
        float totalWeight = 0f;
        foreach (var p in _candidates) totalWeight += p.weight;

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;
        foreach (var p in _candidates)
        {
            cumulative += p.weight;
            if (roll <= cumulative) return p;
        }

        return _candidates[_candidates.Count - 1];
    }
}
