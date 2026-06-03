using UnityEngine;

public class AssassinDeathState : EnemyState<Assassin>
{
    public AssassinDeathState(Assassin enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
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
