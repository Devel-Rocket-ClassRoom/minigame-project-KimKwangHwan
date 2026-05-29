using UnityEngine;

public class MageSkeletonHurtState : EnemyState<MageSkeleton>
{
    private float hurtTimer;
    public MageSkeletonHurtState(MageSkeleton enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void Enter(EnemyState prevState)
    {
        enemy.Animator.SetTrigger("Hurt");
        enemy.Motor.MoveStop();
        hurtTimer = 0f;
    }

    public override void Exit()
    {
        enemy.Animator.ResetTrigger("Hurt");
    }

    public override void PhysicsUpdate()
    {
    }

    public override void Update()
    {
        hurtTimer += Time.deltaTime;
        if (hurtTimer >= enemy.HurtDuration)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
