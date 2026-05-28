using UnityEngine;

public class Assassin : EnemyController
{
    public AssassinIdleState idleState;
    public AssassinMoveState moveState;
    public AssassinAttackState attackState;
    public AssassinHurtState hurtState;
    public AssassinDeathState deathState;
    [SerializeField] private EnemyPerception perception;
    [SerializeField] private float attackDistance;
    [SerializeField] private float hurtDuration;
    public AssassinCombat Combat { get { return (AssassinCombat)enemyCombat; } }

    public bool move = true;
    public Transform Target => perception.Target;
    public float HurtDuration => hurtDuration;
    public float AttackDistance => attackDistance;

    protected override void Awake()
    {
        base.Awake();
        idleState = new AssassinIdleState(this, stateMachine);
        moveState = new AssassinMoveState(this, stateMachine);
        attackState = new AssassinAttackState(this, stateMachine);
        hurtState = new AssassinHurtState(this, stateMachine);
        deathState = new AssassinDeathState(this, stateMachine);
        Health.OnDead += Dead;
        stateMachine.Initialize(idleState);
    }
    protected override void GetHurt(float damage)
    {
        base.GetHurt(damage);
        perception.SetTarget();
        if (stateMachine.CurrentState is AssassinAttackState _attackState)
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
