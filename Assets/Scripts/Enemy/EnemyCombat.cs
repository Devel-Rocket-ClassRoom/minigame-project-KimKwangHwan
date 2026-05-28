using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] protected List<EnemyAttackPattern> patterns;
    [SerializeField] protected Hitbox hitbox;
    [SerializeField] protected Transform muzzle;
    [SerializeField] protected Animator animator;
    protected Dictionary<EnemyAttackPattern, float> lastUsedTime = new();
    protected bool isAttacking;
    [SerializeField] private Vector2 hitboxSize;
    [SerializeField] private Vector2 hitboxOffset;
    public EnemyContext Context { get; protected set; }

    protected virtual void Awake()
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
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube((Vector2)hitbox.transform.position + hitboxOffset, hitboxSize);
    }
}
