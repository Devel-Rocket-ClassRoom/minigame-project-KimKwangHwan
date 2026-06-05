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
        if (enemy.Ctx.playerTransform == null) return;
        //enemy.PendingPattern = enemy.Patterns[0];
        enemy.PendingPattern = enemy.Selector.SelectNext(enemy.Ctx);
        stateMachine.ChangeState(enemy.moveState);
    }
}
