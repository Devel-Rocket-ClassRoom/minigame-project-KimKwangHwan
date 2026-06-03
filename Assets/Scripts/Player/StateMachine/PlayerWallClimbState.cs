using UnityEngine;

public class PlayerWallClimbState : PlayerAirState
{
    public PlayerWallClimbState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    private float wallSide;
    private float _releaseTimer;

    public override void Enter(PlayerState prevState)
    {
        this.prevState = prevState;
        player.Animator.SetBool("IsGrounded", false);
        float wallDir = player.Input.MoveX;
        if (wallDir == 0)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }
        wallSide = Mathf.Sign(wallDir);
        player.WallFlip(wallDir);
        jumpUsed = 0;
        dashUsed = 0;
        pendingWallSide = wallSide;
        _releaseTimer = 0f;
        player.Animator.ResetTrigger("Jump");
        player.Animator.ResetTrigger("DoubleJump");
        player.Animator.SetBool("WallClimb", true);
    }

    // base(PlayerAirState) 안 탐 — 벽 물리 직접 통제
    public override void PhysicsUpdate()
    {
        player.Motor.WallStick(wallSide);
        player.Animator.SetFloat("yVelocity", player.Motor.GetYVelocity());

        if (player.Motor.IsLanded())
        {
            jumpUsed = 0;
            player.Animator.SetBool("IsGrounded", true);
            stateMachine.ChangeState(player.idleState);
            return;
        }
    }

    public override void HandleInput()
    {
        if (CanJump && player.Input.JumpPressed)
        {
            pendingWallJump = true;
            stateMachine.ChangeState(player.jumpState);
            return;
        }
    }

    public override void Update()
    {
        base.Update();
        float input = player.Input.MoveX;
        if (input != 0f && Mathf.Sign(input) == -wallSide)
        {
            _releaseTimer += Time.deltaTime;
            if (_releaseTimer >= player.Motor.WallReleaseHoldTime)
            {
                player.Motor.WallDetach(wallSide);
                stateMachine.ChangeState(player.fallState);
                return;
            }
        }
        else
        {
            _releaseTimer = 0f;
        }

        if (!player.Motor.IsTouchingWall())
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }
    }

    public override void Exit()
    {
        player.Animator.SetBool("WallClimb", false);
    }
}