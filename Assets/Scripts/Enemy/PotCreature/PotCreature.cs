using UnityEngine;

public class PotCreature : EnemyController
{
    public PotCreatureIdleState idleState;
    public PotCreatureMoveState moveState;
    public PotCreatureAttackState attackState;
    public PotCreatureHurtState hurtState;
    public PotCreatureDeathState deathState;
    [SerializeField] private EnemyPerception perception;
    [SerializeField] private float hurtDuration;
    public EnemyCombat Combat { get { return enemyCombat; } }
    public bool move = true;
    public Transform Target => perception.Target;
    public float HurtDuration => hurtDuration;

    protected override void Awake()
    {
        base.Awake();
        idleState = new PotCreatureIdleState(this, stateMachine);
        moveState = new PotCreatureMoveState(this, stateMachine);
        attackState = new PotCreatureAttackState(this, stateMachine);
        hurtState = new PotCreatureHurtState(this, stateMachine);
        deathState = new PotCreatureDeathState(this, stateMachine);
        Health.OnDead += Dead;
        stateMachine.Initialize(idleState);
    }
    protected override void GetHurt(float damage)
    {
        base.GetHurt(damage);
        perception.SetTarget();
        if (stateMachine.CurrentState is PotCreatureHurtState)
            return;
        if (Combat.Context.SuperArmor)
        {
            if (damage >= 25f)
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
