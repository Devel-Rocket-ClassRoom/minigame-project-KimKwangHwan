using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageSkeletonCombat : EnemyCombat
{
    [SerializeField] private List<EnemyAttackPattern> patterns;
    [SerializeField] private Hitbox hitbox;
    [SerializeField] private Transform muzzle;
    [SerializeField] private Animator animator;
    private Dictionary<EnemyAttackPattern, float> lastUsedTime = new();
    private bool isAttacking;
    public EnemyContext Context { get; private set; }
    public Hitbox Hitbox => hitbox;
    void Awake()
    {
        Context = new EnemyContext
        {
            self = transform,
            target = GameObject.FindWithTag("Player").transform, // 나중에 바꿔야 할 듯
            rb = GetComponent<Rigidbody2D>(),
            anim = animator,
            runner = this,
            hitbox = hitbox,
            muzzle = muzzle,
        };
        foreach (var p in patterns) lastUsedTime[p] = -999f;
    }
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
