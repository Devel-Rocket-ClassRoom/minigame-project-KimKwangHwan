using UnityEngine;

[RequireComponent(typeof(PlayerInputReader)), RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private PlayerInputReader playerInput;
    [SerializeField]
    private PlayerMotor playerMotor;
    [SerializeField]
    private PlayerCombat playerCombat;
    [SerializeField]
    private Animator animator;
    private PlayerStateMachine stateMachine;
    public PlayerInputReader Input { get { return playerInput; } }
    public PlayerMotor Motor { get { return playerMotor; } }
    public PlayerCombat Combat { get { return playerCombat; } }
    public PlayerState State { get { return stateMachine.CurrentState; } }
    public Animator Animator { get { return animator; } }
    public float Facing { get { return Mathf.Sign(transform.localScale.x); } } 

    public PlayerIdleState idleState;
    public PlayerRunState runState;
    public PlayerAirState airState;
    public PlayerJumpState jumpState;
    public PlayerFallState fallState;
    public PlayerWallClimbState wallClimbState;
    public PlayerAttackState attackState;
    public PlayerDashState dashState;
    private float moveDirection; // +면 오른쪽, -면 왼쪽
    public float dashSpeed = 20f;
    public float dashDuration = 0.7f;
    public float dashCooldown = 0.5f;

    [HideInInspector] public float lastDashTime = -Mathf.Infinity;
    [HideInInspector] public int airDashLeft;
    public GameObject afterImagePrefab;
    public SpriteRenderer spriteRenderer; 
    public float afterImageInterval = 0.04f;
    public float afterImageLifetime = 0.3f;
    public Color afterImageColor = new Color(0.5f, 0.8f, 1f, 0.6f);
    private void Awake()
    {
        stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(this, stateMachine);
        runState = new PlayerRunState(this, stateMachine);
        airState = new PlayerAirState(this, stateMachine);
        jumpState = new PlayerJumpState(this, stateMachine);
        fallState = new PlayerFallState(this, stateMachine);
        wallClimbState = new PlayerWallClimbState(this, stateMachine);
        attackState = new PlayerAttackState(this, stateMachine);
        dashState = new PlayerDashState(this, stateMachine);

        stateMachine.Initialize(idleState);
        moveDirection = 1f;
    }
    private void Update()
    {
        stateMachine.CurrentState.HandleInput();
        stateMachine.CurrentState.Update();
    }
    private void FixedUpdate()
    {
        stateMachine.CurrentState.PhysicsUpdate();
    }
    public void AllFlip(float x)
    {
        if (x != 0f)
        {
            if (moveDirection * x < 0f)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            moveDirection = x > 0f ? 1f : -1f;
        }
    }
    public void WallFlip(float wallDirection) // wallDirection: 벽이 있는 방향 (1 or -1)
    {
        AllFlip(-wallDirection); // 벽 반대 방향을 바라봄
    }
    public bool CanDash()
    {
        if (stateMachine.CurrentState is PlayerDashState) return false;
        if (Time.time < lastDashTime + dashCooldown) return false;
        return true;
    }
}
