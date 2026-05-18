using UnityEngine;

public class PlayerFallState : PlayerAirState
{
    public PlayerFallState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        Debug.Log("FallState");
        player.Animator.SetBool("IsGrounded", false);
    }

    public override void Update()
    {
        base.Update();
        Debug.Log(player.Motor.ClimbCheck());
        if (player.Motor.ClimbCheck())
        {
            stateMachine.ChangeState(player.wallClimbState);
        }
    }
}
