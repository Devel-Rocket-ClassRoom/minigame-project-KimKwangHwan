using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class WitchMoveState : EnemyState<Witch>
{
    private CancellationTokenSource moveCts;
    public WitchMoveState(Witch enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void Enter(EnemyState prevState)
    {
        moveCts = new CancellationTokenSource();

        MoveRoutine(moveCts.Token).Forget();
    }

    public override void Exit()
    {
        moveCts?.Cancel();
        moveCts?.Dispose();
        moveCts = null;
    }

    public override void PhysicsUpdate()
    {
    }

    public override void Update()
    {
    }
    private async UniTask MoveRoutine(CancellationToken ct)
    {
        var pattern = enemy.PendingPattern;

        if (pattern != null && pattern.patternType != PatternType.None)
            await enemy.teleportMove.Execute(enemy.Ctx, pattern.patternType, pattern.minDistance, pattern.maxDistance, ct);

        stateMachine.ChangeState(enemy.attackState);
    }
}
