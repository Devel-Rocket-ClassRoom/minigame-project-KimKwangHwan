using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    // 이 공중 체공 동안 쓴 점프 수 — AirState 계열이 단독 소유
    protected static int jumpsUsed;
    protected const int maxJumps = 2;

    // 다음 JumpState가 벽 점프인지 / 어느 쪽 벽인지
    protected static bool pendingWallJump;
    protected static float pendingWallSide;

    protected bool CanJump => jumpsUsed < maxJumps;

    public override void HandleInput()
    {
        player.AllFlip(player.Input.MoveX);
        if (CanJump && player.Input.JumpPressed)
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }
        if (player.Combat.AttackBuffered)
        {
            stateMachine.ChangeState(player.attackState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        player.Motor.MoveHorizontal(player.Input.MoveX * 0.8f);
        player.Animator.SetFloat("yVelocity", player.Motor.GetYVelocity());

        if (player.Motor.IsLanded())
        {
            jumpsUsed = 0;                 // 착지 → 리셋. Motor 아님, 여기서.
            player.Animator.SetBool("IsGrounded", true);
            if (Mathf.Abs(player.Input.MoveX) != 0f)
                stateMachine.ChangeState(player.runState);
            else
                stateMachine.ChangeState(player.idleState);
        }
    }

    public override void Enter(PlayerState prevState) { }
    public override void Exit() { }
    public override void Update() { }
}