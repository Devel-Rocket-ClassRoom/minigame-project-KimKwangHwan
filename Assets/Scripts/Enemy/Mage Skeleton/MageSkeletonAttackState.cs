using UnityEngine;

public class MageSkeletonAttackState : EnemyState<MageSkeleton>
{
    public MageSkeletonAttackState(MageSkeleton enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void Enter(EnemyState prevState)
    {
        enemy.Motor.MoveStop();
    }

    public override void Exit()
    {
    }

    public override void PhysicsUpdate()
    {
    }

    public override void Update()
    {
    }
}
