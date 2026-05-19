using UnityEngine;

public class PlayerIdleState : PlayerGroundState
{
    public PlayerIdleState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        
    }

    public override void Exit()
    {
        
    }

    public override void HandleInput()
    {
        if (Mathf.Abs(player.Input.MoveX) != 0.0f)
        {
            stateMachine.ChangeState(player.runState);
            return;
        }
        if (player.Input.JumpPressed && player.Motor.IsGrounded())
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        if (player.Motor.GetYVelocity() < 0 && !player.Motor.IsGrounded())
        {
            player.Animator.SetFloat("yVelocity", player.Motor.GetYVelocity());
            player.Animator.SetBool("IsGrounded", player.Motor.IsGrounded());
            stateMachine.ChangeState(player.fallState);
        }
    }

    public override void Update()
    {
        
    }
}
