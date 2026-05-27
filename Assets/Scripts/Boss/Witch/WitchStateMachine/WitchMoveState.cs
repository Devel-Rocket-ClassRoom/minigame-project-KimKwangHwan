using System.Collections;
using UnityEngine;

public class WitchMoveState : EnemyState<Witch>
{
    private Coroutine routine;
    public WitchMoveState(Witch enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void Enter(EnemyState prevState)
    {
        routine = enemy.StartCoroutine(MoveRoutine());
    }

    public override void Exit()
    {
        if (routine != null) enemy.StopCoroutine(routine);
    }

    public override void PhysicsUpdate()
    {
    }

    public override void Update()
    {
    }
    private IEnumerator MoveRoutine()
    {
        var pattern = enemy.PendingPattern;

        if (pattern != null && pattern.patternType != PatternType.None)
            yield return enemy.teleportMove.Execute(enemy.Ctx, pattern.patternType, pattern.minDistance, pattern.maxDistance);

        stateMachine.ChangeState(enemy.attackState);
    }
}
