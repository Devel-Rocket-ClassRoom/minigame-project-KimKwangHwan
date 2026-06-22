using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
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
    //[SerializeField] private Transform player;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundWidth;
    [SerializeField] private float groundHeight;
    [SerializeField] private BossRoom bossRoom;
    [SerializeField] private float groundCheckDist = 0.15f;
    [SerializeField] private float minNormalY = 0.5f;

    private PatternSelector selector;

    public bool IsGrounded
    {
        get
        {
            Vector2 c = groundCheck.position;
            float half = groundWidth * 0.5f - 0.05f;
            Vector2[] origins = { c + Vector2.left * half, c, c + Vector2.right * half };

            foreach (var o in origins)
            {
                RaycastHit2D hit = Physics2D.Raycast(o, Vector2.down, groundCheckDist, groundLayer);
                if (hit && hit.normal.y >= minNormalY) return true;
            }
            return false;
        }
    }

    public bool IsGroundedOn(LayerMask layer)
    {
        Vector2 c = groundCheck.position;
        float half = groundWidth * 0.5f - 0.05f;
        Vector2[] origins = { c + Vector2.left * half, c, c + Vector2.right * half };

        foreach (var o in origins)
        {
            RaycastHit2D hit = Physics2D.Raycast(o, Vector2.down, groundCheckDist, layer);
            if (hit && hit.normal.y >= minNormalY) return true;
        }
        return false;
    }
    public List<BossPattern> Patterns => patterns;
    public PatternSelector Selector => selector;
    public WitchAttackState attackState;
    public WitchIdleState idleState;
    public WitchMoveState moveState;
    public WitchEngageState engageState;
    public WitchDeathState deathState;
    public BossPattern PendingPattern { get; set; }
    public BossContext Ctx => ctx;
    public BossMoveBehavior teleportMove;
    public AudioClip teleportClip;
    protected override void Awake()
    {
        base.Awake();
        ctx = new BossContext
        {
            bossTransform = transform,
            animator = animator,
            animEvents = animEvents,
            hitbox = hitbox,
            muzzle = muzzle,
            bossRoom = bossRoom,
            currentPhase = currentPhase,
        };
        selector = new PatternSelector(patterns);
        idleState = new WitchIdleState(this, stateMachine);
        attackState = new WitchAttackState(this, stateMachine);
        moveState = new WitchMoveState(this, stateMachine);
        engageState = new WitchEngageState(this, stateMachine);
        deathState = new WitchDeathState(this, stateMachine);
        stateMachine.Initialize(engageState);
        teleportMove = new TeleportMove(groundLayer, teleportClip);
        ctx.bossRoom.OnPlayerEnter += OnPlayerEnterRoom;
        Health.OnDead += Dead;
    }

    private void OnPlayerEnterRoom()
    {
        ctx.playerTransform = PlayerManager.Instance.Current?.transform;
        //if (ctx.playerTransform == null) return;
        //if (stateMachine.CurrentState is WitchEngageState)
        //{
        //    stateMachine.ChangeState(idleState);
        //}
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.transform.position, new Vector3(groundWidth, groundHeight, 1f));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)hitbox.transform.position + hitboxOffset, hitboxSize);
    }
    private void Dead()
    {
        stateMachine.ChangeState(deathState);
    }

    public void StartFight()
    {
        if (stateMachine.CurrentState is WitchEngageState)
            stateMachine.ChangeState(idleState);
    }
}