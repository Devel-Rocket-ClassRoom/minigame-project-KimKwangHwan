using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] protected List<EnemyAttackPattern> patterns;
    [SerializeField] protected Hitbox hitbox;
    [SerializeField] protected Transform muzzle;
    [SerializeField] protected Animator animator;
    [SerializeField] protected EnemyAnimEvent animEvents;
    protected Dictionary<EnemyAttackPattern, float> lastUsedTime = new();
    protected bool isAttacking;
    [SerializeField] private Vector2 hitboxSize;
    [SerializeField] private Vector2 hitboxOffset;
    public EnemyContext Context { get; protected set; }

    protected virtual void Awake()
    {
        foreach (var p in patterns) lastUsedTime[p] = -999f;
    }
    protected virtual void Start()
    {
        Context = new EnemyContext
        {
            self = transform,
            target = PlayerManager.Instance.Current?.transform,
            rb = GetComponent<Rigidbody2D>(),
            anim = animator,
            runner = this,
            hitbox = hitbox,
            muzzle = muzzle,
            animEvents = animEvents
        };
    }
    public EnemyAttackPattern SelectPattern()
    {
        if (Context.target == null)
            Context.target = PlayerManager.Instance.Current?.transform;

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
    private void OnDrawGizmos()
    {
        if (hitbox != null)
            Gizmos.DrawWireCube((Vector2)hitbox.transform.position + hitboxOffset, hitboxSize);
    }
}
