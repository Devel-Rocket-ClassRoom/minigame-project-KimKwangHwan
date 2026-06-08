public class PlayerJumpState : PlayerAirState
{
    public PlayerJumpState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override void Enter(PlayerState prevState)
    {
        this.prevState = prevState;
        player.Animator.SetBool("IsGrounded", false);

        if (pendingWallJump)
        {
            player.Motor.WallJump(pendingWallSide);
            pendingWallJump = false;
        }
        else
        {
            player.Motor.JumpVertical();
        }
        jumpUsed++;

        if (jumpUsed == 1)
        {
            player.Animator.SetTrigger("Jump");
            SFXManager.Instance.PlaySFX(player.jumpClip);
        }
        else
        {
            player.Animator.SetTrigger("DoubleJump");
            SFXManager.Instance.PlaySFX(player.doubleJumpClip);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        if (player.Motor.GetYVelocity() < 0f)
            stateMachine.ChangeState(player.fallState);
    }
    public override void Update()
    {
        base.Update();
        //if (player.Motor.ClimbCheck())
        //{
        //    stateMachine.ChangeState(player.wallClimbState);
        //}
    }
}