using UnityEngine;

public class ElectricGolem : EnemyController
{
    public ElectricGolemIdleState idleState;
    public ElectricGolemMoveState moveState;
    public ElectricGolemAttackState attackState;
    public ElectricGolemHurtState hurtState;
    public ElectricGolemDeathState deathState;
    [SerializeField] private EnemyPerception perception;
    [SerializeField] private float hurtDuration;
    public EnemyCombat Combat { get { return enemyCombat; } }
    public bool move = true;
    public Transform Target => perception.Target;
    public float HurtDuration => hurtDuration;
    protected override void Awake()
    {
        base.Awake();
        idleState = new ElectricGolemIdleState(this, stateMachine);
        moveState = new ElectricGolemMoveState(this, stateMachine);
        attackState = new ElectricGolemAttackState(this, stateMachine);
        hurtState = new ElectricGolemHurtState(this, stateMachine);
        deathState = new ElectricGolemDeathState(this, stateMachine);
        Health.OnDead += Dead;
        stateMachine.Initialize(idleState);
    }
    protected override void GetHurt(float damage)
    {
        base.GetHurt(damage);
        perception.SetTarget();
        if (Combat.Context.SuperArmor)
        {
            if (damage >= 40f)
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
