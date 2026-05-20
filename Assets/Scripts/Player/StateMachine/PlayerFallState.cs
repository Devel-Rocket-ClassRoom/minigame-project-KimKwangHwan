using UnityEngine;

public class PlayerFallState : PlayerAirState
{
    public PlayerFallState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override void Enter(PlayerState prevState)
    {
        this.prevState = prevState;
        player.Animator.SetBool("IsGrounded", false);

        // 점프를 한 번도 안 썼는데 떨어진다 = 난간 낙하 → 1단 소모
        // 점프 후 하강(jumpsUsed>=1)이나 벽 점프 후엔 그대로 둠
        if (jumpsUsed == 0)
            jumpsUsed = 1;
    }

    public override void Update()
    {
        base.Update();
        if (player.Motor.ClimbCheck())
        {
            stateMachine.ChangeState(player.wallClimbState);
        }
    }
}