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
            stateMachine.ChangeState(enemy.idleState);
            return;
        }
        
        enemy.Combat.MarkPatternUsed(pattern);
        enemy.Motor.MoveStop();
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
            stateMachine.ChangeState(enemy.idleState);
    }

    public override void PhysicsUpdate() { }

    public override void Exit()
    {
        enemy.Combat.Context.SuperArmor = false;
        if (routine != null)
        {
            enemy.StopCoroutine(routine);
            routine = null;
        }
    }
}