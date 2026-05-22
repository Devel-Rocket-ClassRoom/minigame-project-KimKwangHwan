using System;
using UnityEngine;

public class MageSkeleton : EnemyController
{
    public MageSkeletonIdleState idleState;
    public MageSkeletonMoveState moveState;
    public MageSkeletonAttackState attackState;
    [SerializeField] private EnemyPerception perception;
    [SerializeField] private float attackDistance;
    public EnemyMotor Motor { get { return enemyMotor; } }
    public MageSkeletonCombat Combat { get { return (MageSkeletonCombat)enemyCombat; } }
    public EnemyHealth Health { get { return enemyHealth; } }
    public bool move = true;
    public Transform Target => perception.Target;
    public float AttackDistance => attackDistance;
    protected override void Awake()
    {
        base.Awake();
        idleState = new MageSkeletonIdleState(this, stateMachine);
        moveState = new MageSkeletonMoveState(this, stateMachine);
        attackState = new MageSkeletonAttackState(this, stateMachine);
        stateMachine.Initialize(idleState);
    }
    protected override void GetHurt(float damage)
    {
        base.GetHurt(damage);
        perception.SetTarget();
    }
}
