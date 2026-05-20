using UnityEngine;

public class MageSkeleton : EnemyController
{
    public MageSkeletonIdleState idleState;
    public MageSkeletonMoveState moveState;
    public MageSkeletonAttackState attackState;

    protected override void Awake()
    {
        base.Awake();
        idleState = new MageSkeletonIdleState(this, stateMachine);
        moveState = new MageSkeletonMoveState(this, stateMachine);
        attackState = new MageSkeletonAttackState(this, stateMachine);
    }
}
