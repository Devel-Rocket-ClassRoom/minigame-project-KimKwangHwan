using UnityEngine;

public class WitchDeathState : EnemyState<Witch>
{
    public WitchDeathState(Witch enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void Enter(EnemyState prevState)
    {
        //this.prevState = prevState;
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
