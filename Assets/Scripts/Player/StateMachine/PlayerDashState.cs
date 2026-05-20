using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter(PlayerState prevState)
    {
        this.prevState = prevState;
        // player.Animator
    }

    public override void Exit()
    {
    }

    public override void HandleInput()
    {
    }

    public override void PhysicsUpdate()
    {
    }

    public override void Update()
    {
    }
}
