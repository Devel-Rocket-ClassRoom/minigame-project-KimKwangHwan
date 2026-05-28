using UnityEngine;

public class ElectricGolemHurtState : EnemyState<ElectricGolem>
{
    private float hurtTimer;
    public ElectricGolemHurtState(ElectricGolem enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
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
