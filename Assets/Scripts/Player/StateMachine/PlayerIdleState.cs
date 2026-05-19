using UnityEngine;

public class PlayerIdleState : PlayerGroundState
{
    public PlayerIdleState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        player.Animator.SetBool("IsGrounded", true);
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
        if (player.Combat.AttackBuffered)
        {
            stateMachine.ChangeState(player.attackState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        player.Animator.SetFloat("yVelocity", player.Motor.GetYVelocity());
        if (player.Motor.GetYVelocity() < 0.0f && !player.Motor.IsGrounded())
        {
            player.Animator.SetBool("IsGrounded", player.Motor.IsGrounded());
            stateMachine.ChangeState(player.fallState);
        }
    }

    public override void Update()
    {
        
    }
}
