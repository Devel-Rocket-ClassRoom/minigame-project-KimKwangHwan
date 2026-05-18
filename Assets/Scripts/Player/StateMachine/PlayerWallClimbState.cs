using UnityEngine;

public class PlayerWallClimbState : PlayerAirState
{
    public PlayerWallClimbState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("WallStateEnter");
        float wallDir = player.Input.MoveX;
        player.WallFlip(wallDir);
    }
}
