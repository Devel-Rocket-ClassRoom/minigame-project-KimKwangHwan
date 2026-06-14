using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class WitchAttackState : EnemyState<Witch>
{
    private CancellationTokenSource _attackCts;

    public WitchAttackState(Witch enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void Enter(EnemyState prevState)
    {
        _attackCts = new CancellationTokenSource();
        AttackRoutine(_attackCts.Token).Forget();
    }

    public override void Exit()
    {
        _attackCts?.Cancel();
        _attackCts?.Dispose();
        _attackCts = null;
    }

    public override void PhysicsUpdate()
    {
    }

    public override void Update()
    {
    }

    private async UniTask AttackRoutine(CancellationToken ct)
    {
        var pattern = enemy.PendingPattern;

        if (pattern != null)
        {
            pattern.lastUsedTime = Time.time;
            await pattern.Execute(enemy.Ctx, ct);
        }

        // 다 끝났으니 슬롯 비우고 Idle로
        enemy.PendingPattern = null;
        stateMachine.ChangeState(enemy.idleState);
    }
}
