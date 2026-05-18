using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

[RequireComponent(typeof(PlayerInputReader)), RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private PlayerInputReader playerInput;
    [SerializeField]
    private PlayerMotor playerMotor;
    [SerializeField]
    private Animator animator;
    private PlayerStateMachine stateMachine;
    public PlayerInputReader Input { get { return playerInput; } }
    public PlayerMotor Motor { get { return playerMotor; } }
    public PlayerState State { get { return stateMachine.CurrentState; } }
    public Animator Animator { get { return animator; } }

    public PlayerIdleState idleState;
    public PlayerRunState runState;
    public PlayerAirState airState;
    public PlayerJumpState jumpState;
    public PlayerFallState fallState;
    public PlayerWallClimbState wallClimbState;
    private float moveDirection; // +면 오른쪽, -면 왼쪽
    [SerializeField]
    private List<GameObject> Childrens;
    [SerializeField]
    private float flipOffset;
    private void Awake()
    {
        stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(this, stateMachine);
        runState = new PlayerRunState(this, stateMachine);
        airState = new PlayerAirState(this, stateMachine);
        jumpState = new PlayerJumpState(this, stateMachine);
        fallState = new PlayerFallState(this, stateMachine);
        wallClimbState = new PlayerWallClimbState(this, stateMachine);
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
    public void AllFlip(float x) // 플레이어의 모든 자식들을 Flip해야 함
    {
        // if (Motor.WallClimbing()) return;
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

}
