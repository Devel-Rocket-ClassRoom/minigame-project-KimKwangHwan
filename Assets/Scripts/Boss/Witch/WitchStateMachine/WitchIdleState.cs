using UnityEngine;

public class WitchIdleState : EnemyState<Witch>
{
    public WitchIdleState(Witch enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void Enter(EnemyState prevState)
    {
        this.prevState = prevState;
        //enemy.Motor.MoveStop();
    }

    public override void Exit()
    {
    }

    public override void PhysicsUpdate()
    {
    }

    public override void Update()
    {
        enemy.PendingPattern = enemy.Selector.SelectNext(enemy.Ctx);
        stateMachine.ChangeState(enemy.moveState);
    }
}
