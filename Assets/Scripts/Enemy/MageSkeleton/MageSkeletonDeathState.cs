using UnityEngine;

public class MageSkeletonDeathState : EnemyState<MageSkeleton>
{
    public MageSkeletonDeathState(MageSkeleton enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void Enter(EnemyState prevState)
    {
        enemy.Animator.SetBool("Death", true);
        enemy.StartFadeAndDestroy();
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
