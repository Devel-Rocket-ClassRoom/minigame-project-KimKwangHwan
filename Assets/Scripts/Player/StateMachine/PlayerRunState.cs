using UnityEngine;

public class PlayerRunState : PlayerState
{
    public PlayerRunState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        player.Animator.SetBool("Run", true);
    }

    public override void Exit()
    {
        player.Animator.SetBool("Run", false);
    }

    public override void HandleInput()
    {
        if (Mathf.Abs(player.Input.MoveX) == 0.0f)
        {
            player.Motor.MoveStop();
            stateMachine.ChangeState(player.idleState);
            return;
        }
        if (player.Input.JumpPressed && player.Motor.IsGrounded())
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }
        
        player.AllFlip(player.Input.MoveX);
    }

    public override void PhysicsUpdate()
    {
        player.Motor.MoveHorizontal(player.Input.MoveX);
        if (player.Motor.GetYVelocity() < 0 && !player.Motor.IsGrounded())
        {
            player.Animator.SetFloat("yVelocity", player.Motor.GetYVelocity());
            stateMachine.ChangeState(player.fallState);
        }
    }

    public override void Update()
    {
        
    }
}
