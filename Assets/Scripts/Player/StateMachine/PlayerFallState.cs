using UnityEngine;

public class PlayerFallState : PlayerAirState
{
    public PlayerFallState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    // 난간 낙하 직후 1단 점프를 그대로 쓸 수 있는 짧은 윈도우
    private const float coyoteTime = 0.12f;
    private float coyoteTimer;
    private bool coyoteActive;

    public override void Enter(PlayerState prevState)
    {
        this.prevState = prevState;
        player.Animator.SetBool("IsGrounded", false);
        if (prevState is PlayerDashState dashState)
        {
            dashUsed = 1;
        }

        coyoteActive = false;
        if (jumpUsed == 0)
        {
            // 지상에서 바로 fall로 떨어진 경우 → 코요테 윈도우 동안 1단 점프 유지
            if (prevState is PlayerIdleState || prevState is PlayerRunState)
            {
                coyoteTimer = coyoteTime;
                coyoteActive = true;
            }
            else
            {
                // 그 외 경로(예: dash 후 등)에서 jumpUsed=0이면 기존 안전망
                jumpUsed = 1;
            }
        }
    }

    public override void Update()
    {
        base.Update();

        if (coyoteActive)
        {
            coyoteTimer -= Time.deltaTime;
            if (coyoteTimer <= 0f)
            {
                jumpUsed = 1;
                coyoteActive = false;
            }
        }

        if (player.Motor.ClimbCheck())
        {
            stateMachine.ChangeState(player.wallClimbState);
        }
    }

    public override void Exit()
    {
        coyoteActive = false;
    }
}