using UnityEngine;
using System.Collections;

public class WitchAttackState : EnemyState<Witch>
{
    private Coroutine routine;
    public WitchAttackState(Witch enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void Enter(EnemyState prevState)
    {
        routine = enemy.StartCoroutine(AttackRoutine());
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
    private IEnumerator AttackRoutine()
    {
        var pattern = enemy.PendingPattern;

        if (pattern != null)
        {
            pattern.lastUsedTime = Time.time;
            yield return pattern.Execute(enemy.Ctx);
        }

        // 다 끝났으니 슬롯 비우고 Idle로
        enemy.PendingPattern = null;
        stateMachine.ChangeState(enemy.idleState);
    }
}
