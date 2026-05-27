using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class Witch : EnemyController
{
    [SerializeField] private List<BossPattern> patterns;
    private BossContext ctx;
    private BossPattern lastPattern;
    private int currentPhase = 2;
    [SerializeField] private BossAnimEvents animEvents;
    [SerializeField] private Hitbox hitbox;
    [SerializeField] private Vector2 hitboxOffset;
    [SerializeField] private Vector2 hitboxSize;
    [SerializeField] private Transform muzzle;
    [SerializeField] private Transform player;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundWidth;
    [SerializeField] private float groundHeight;
    private PatternSelector selector;
    public bool IsGrounded => Physics2D.OverlapBox(
        groundCheck.position, new Vector2(groundWidth, groundHeight), 0f, groundLayer);
    public List<BossPattern> Patterns => patterns;
    public PatternSelector Selector => selector;
    public WitchAttackState attackState;
    public BossContext Ctx => ctx;
    protected override void Awake()
    {
        base.Awake();
        ctx = new BossContext
        {
            bossTransform = transform,
            playerTransform = player,
            animator = animator,
            animEvents = animEvents,
            hitbox = hitbox,
            muzzle = muzzle,
            currentPhase = currentPhase
        };
        selector = new PatternSelector(patterns);
        attackState = new WitchAttackState(this, stateMachine);
        stateMachine.Initialize(attackState);
    }
    //private void Start()
    //{
    //    StartCoroutine(BehaviorLoop());
    //}
    //private IEnumerator BehaviorLoop()
    //{
    //    while (true)
    //    {
    //        var pattern = Selector.SelectNext(ctx);
    //        yield return StartCoroutine(ExecutePattern(pattern));
    //    }

    //}
    //private IEnumerator ExecutePattern(BossPattern pattern)
    //{
    //    lastPattern = pattern;
    //    pattern.lastUsedTime = Time.time;

    //    yield return StartCoroutine(pattern.Execute(ctx));
    //}

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.transform.position, new Vector3(groundWidth, groundHeight, 1f));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)hitbox.transform.position + hitboxOffset, hitboxSize);
    }
}