using System;
using UnityEngine;

public class MageSkeleton : EnemyController
{
    public MageSkeletonIdleState idleState;
    public MageSkeletonMoveState moveState;
    public MageSkeletonAttackState attackState;
    public MageSkeletonHurtState hurtState;
    public MageSkeletonDeathState deathState;
    [SerializeField] private EnemyPerception perception;
    [SerializeField] private float attackDistance;
    [SerializeField] private float hurtDuration;
    
    
    public MageSkeletonCombat Combat { get { return (MageSkeletonCombat)enemyCombat; } }
    
    public bool move = true;
    public Transform Target => perception.Target;
    public float HurtDuration => hurtDuration;
    public float AttackDistance => attackDistance;
    protected override void Awake()
    {
        base.Awake();
        idleState = new MageSkeletonIdleState(this, stateMachine);
        moveState = new MageSkeletonMoveState(this, stateMachine);
        attackState = new MageSkeletonAttackState(this, stateMachine);
        hurtState = new MageSkeletonHurtState(this, stateMachine);
        deathState = new MageSkeletonDeathState(this, stateMachine);
        Health.OnDead += Dead;
        stateMachine.Initialize(idleState);
    }
    protected override void GetHurt(float damage)
    {
        base.GetHurt(damage);
        perception.SetTarget();
        if (Combat.Context.SuperArmor)
        {
            if (damage >= 20f)
                stateMachine.ChangeState(hurtState);
            return;
        }
        stateMachine.ChangeState(hurtState);
    }

    private void Dead()
    {
        stateMachine.ChangeState(deathState);
    }
}
