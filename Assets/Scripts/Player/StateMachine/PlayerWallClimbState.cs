using UnityEngine;

public class PlayerWallClimbState : PlayerAirState
{
    public PlayerWallClimbState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        float wallDir = player.Input.MoveX;
        player.WallFlip(wallDir);
        player.Motor.WallClimb();
        player.Animator.SetBool("WallClimb", true);
    }

    public override void Exit()
    {
        player.Motor.SeperateToWall();
        player.Animator.SetBool("WallClimb", false);
    }
}
