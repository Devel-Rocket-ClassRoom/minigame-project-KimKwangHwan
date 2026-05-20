using System;
using UnityEngine;

public class MageSkeleton : EnemyController
{
    public MageSkeletonIdleState idleState;
    public MageSkeletonMoveState moveState;
    public MageSkeletonAttackState attackState;
    [SerializeField] private EnemyPerception perception;
    [SerializeField] private float attackDistance;
    public bool move = true;
    public Transform Target => perception.Target;
    protected override void Awake()
    {
        base.Awake();
        idleState = new MageSkeletonIdleState(this, stateMachine);
        moveState = new MageSkeletonMoveState(this, stateMachine);
        attackState = new MageSkeletonAttackState(this, stateMachine);
        stateMachine.Initialize(idleState);
    }
}
