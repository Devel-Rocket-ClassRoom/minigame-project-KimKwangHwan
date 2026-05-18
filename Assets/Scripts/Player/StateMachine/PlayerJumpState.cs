using UnityEngine;

public class PlayerJumpState : PlayerAirState
{
    public PlayerJumpState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override void Enter()
    {
        player.Animator.SetBool("IsGrounded", false);
        player.Motor.JumpVertical();
        Debug.Log(player.Motor.JumpCount);
        if (player.Motor.JumpCount == 1)
        {
            player.Animator.SetTrigger("Jump");
        }
        else
        {
            player.Animator.SetTrigger("DoubleJump");
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();  // 공통 로직 (이동, 착지 감지)

        // 하강 시작하면 FallState로
        if (player.Motor.GetYVelocity() < 0f)
            stateMachine.ChangeState(player.fallState);
    }
}
