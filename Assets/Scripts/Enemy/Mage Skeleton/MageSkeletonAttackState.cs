using System.Collections;
using UnityEngine;

public class MageSkeletonAttackState : EnemyState<MageSkeleton>
{
    private Coroutine routine;
    private bool finished;

    public MageSkeletonAttackState(MageSkeleton enemy, EnemyStateMachine stateMachine)
        : base(enemy, stateMachine) { }

    public override void Enter(EnemyState prevState)
    {
        finished = false;

        var pattern = enemy.Combat.SelectPattern();
        if (pattern == null)
        {
            // 사거리 안에 있어도 쿨다운 중이면 패턴이 없을 수 있음 → 다시 이동
            stateMachine.ChangeState(enemy.moveState);
            return;
        }

        // 공격 중엔 제자리. 타겟 방향으로 한 번 정렬.
        enemy.Motor.MoveHorizontal(0f);
        enemy.Combat.MarkPatternUsed(pattern);
        routine = enemy.StartCoroutine(RunPattern(pattern));
    }

    private IEnumerator RunPattern(EnemyAttackPattern pattern)
    {
        float dir = Mathf.Sign(enemy.Target.position.x - enemy.transform.position.x);
        enemy.AllFlip(dir);
        yield return pattern.Execute(enemy.Combat.Context);
        finished = true;
    }

    public override void Update()
    {
        if (finished)
            stateMachine.ChangeState(enemy.moveState);
    }

    public override void PhysicsUpdate() { }

    public override void Exit()
    {
        if (routine != null)
        {
            enemy.StopCoroutine(routine);
            routine = null;
        }
    }
}