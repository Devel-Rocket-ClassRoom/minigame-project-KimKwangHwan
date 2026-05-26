using UnityEngine;

public class PlayerGroundState : PlayerState
{
    public PlayerGroundState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter(PlayerState prevState)
    {
        this.prevState = prevState;
        PlayerAirState.ResetAirCounters();
    }

    public override void Exit()
    {
    }

    public override void HandleInput()
    {
        if (player.Input.DashPressed && player.CanDash())
        {
            stateMachine.ChangeState(player.dashState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
    }

    public override void Update()
    {
    }
}
