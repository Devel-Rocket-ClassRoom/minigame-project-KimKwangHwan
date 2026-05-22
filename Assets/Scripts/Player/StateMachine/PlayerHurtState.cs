using UnityEngine;

public class PlayerHurtState : PlayerState
{
    private float hurtTimer;
    public PlayerHurtState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter(PlayerState prevState)
    {
        player.Animator.SetTrigger("Hurt");
        player.Motor.MoveStop();
        hurtTimer = 0f;
    }

    public override void Exit()
    {
        player.Animator.ResetTrigger("Hurt");
    }

    public override void HandleInput()
    {
        if (player.Input.DashPressed && hurtTimer >= player.hurtEscapeTime)
        {
            stateMachine.ChangeState(player.dashState);
        }
    }

    public override void PhysicsUpdate()
    {
    }

    public override void Update()
    {
        hurtTimer += Time.deltaTime;
        if (hurtTimer >= player.hurtDuration)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
