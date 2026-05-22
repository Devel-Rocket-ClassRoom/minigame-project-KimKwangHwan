using UnityEngine;

public class EnemyStateMachine
{
    public EnemyState CurrentState { get; private set; }

    public void Initialize(EnemyState startState)
    {
        CurrentState = startState;
        CurrentState.Enter(null);
    }

    public void ChangeState(EnemyState newState)
    {
        CurrentState.Exit();
        var prev = CurrentState;
        CurrentState = newState;
        CurrentState.Enter(prev);
    }
}
