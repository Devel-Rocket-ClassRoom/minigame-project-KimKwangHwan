using UnityEngine;

public class PotCreatureDeathState : EnemyState<PotCreature>
{
    public PotCreatureDeathState(PotCreature enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void Enter(EnemyState prevState)
    {
        enemy.Animator.SetBool("Death", true);
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
