using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
public class ElectricGolemAttackState : EnemyState<ElectricGolem>
{
    private CancellationTokenSource attackCts;
    private bool finished;
    public ElectricGolemAttackState(ElectricGolem enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

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
    public override void Exit()
    {
        enemy.Combat.Context.SuperArmor = false;
        attackCts?.Cancel();
        attackCts?.Dispose();
        attackCts = null;
    }

    public override void PhysicsUpdate()
    {
    }

    public override void Update()
    {
        if (finished)
            stateMachine.ChangeState(enemy.idleState);
    }
}
