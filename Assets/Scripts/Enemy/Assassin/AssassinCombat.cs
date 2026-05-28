using System.Collections.Generic;
using UnityEngine;

public class AssassinCombat : EnemyCombat
{
    public EnemyAttackPattern SelectPattern()
    {
        // 쿨다운 + CanExecute 통과한 것 중 priority 높은 거
        EnemyAttackPattern best = null;
        int bestPriority = int.MinValue;
        foreach (var p in patterns)
        {
            if (Time.time - lastUsedTime[p] < p.cooldown) continue;
            if (!p.CanExecute(Context)) continue;
            if (p.priority > bestPriority) { best = p; bestPriority = p.priority; }
        }
        return best;
    }
    public void MarkPatternUsed(EnemyAttackPattern p) => lastUsedTime[p] = Time.time;
}
