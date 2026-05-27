using UnityEngine;
using System.Collections;

public class WitchAttackState : EnemyState<Witch>
{
    private Coroutine loopRoutine;
    private Coroutine currentPatternRoutine;
    public WitchAttackState(Witch enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void Enter(EnemyState prevState)
    {
        loopRoutine = enemy.StartCoroutine(BehaviorLoop());
    }

    public override void Exit()
    {
        if (currentPatternRoutine != null) enemy.StopCoroutine(currentPatternRoutine);
        if (loopRoutine != null) enemy.StopCoroutine(loopRoutine);
    }

    public override void PhysicsUpdate()
    {
    }

    public override void Update()
    {
    }

    private IEnumerator BehaviorLoop()
    {
        while (true)
        {
            var pattern = enemy.Selector.SelectNext(enemy.Ctx);
            pattern.lastUsedTime = Time.time;

            currentPatternRoutine = enemy.StartCoroutine(pattern.Execute(enemy.Ctx));
            yield return currentPatternRoutine;

            // 패턴 사이에 살짝 쉬고 싶으면
            // yield return new WaitForSeconds(enemy.AttackInterval);
        }
    }
}
