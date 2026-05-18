using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(PlayerController player, PlayerStateMachine stateMachine) 
        : base(player, stateMachine) { }

    public override void HandleInput()
    {
        player.AllFlip(player.Input.MoveX);
        if (player.Motor.CanJump() && player.Input.JumpPressed)
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        player.Motor.MoveHorizontal(player.Input.MoveX);
        player.Animator.SetFloat("yVelocity", player.Motor.GetYVelocity());

        if (player.Motor.IsLanded())
        {
            player.Animator.SetBool("IsGrounded", true);
            if (Mathf.Abs(player.Input.MoveX) != 0f)
                stateMachine.ChangeState(player.runState);
            else
                stateMachine.ChangeState(player.idleState);
        }
    }

    public override void Enter() { }
    public override void Exit() { }
    public override void Update() { }
}
