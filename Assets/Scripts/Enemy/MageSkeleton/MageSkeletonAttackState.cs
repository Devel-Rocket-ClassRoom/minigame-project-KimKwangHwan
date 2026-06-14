using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class MageSkeletonAttackState : EnemyState<MageSkeleton>
{
    private CancellationTokenSource attackCts;
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
        attackCts = new CancellationTokenSource();
        RunPattern(pattern).Forget();
    }

    private async UniTask RunPattern(EnemyAttackPattern pattern)
    {
        float dir = Mathf.Sign(enemy.Target.position.x - enemy.transform.position.x);
        
        enemy.AllFlip(dir);
        await pattern.Execute(enemy.Combat.Context, attackCts.Token);
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
        attackCts?.Cancel();
        attackCts?.Dispose();
        attackCts = null;
    }
}