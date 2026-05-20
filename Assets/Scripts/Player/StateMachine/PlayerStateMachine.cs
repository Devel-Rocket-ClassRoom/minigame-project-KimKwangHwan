using UnityEngine;

public class PlayerStateMachine
{
    public PlayerState CurrentState { get; private set; }

    public void Initialize(PlayerState startState)
    {
        CurrentState = startState;
        CurrentState.Enter(null);
    }

    public void ChangeState(PlayerState newState)
    {
        CurrentState.Exit();
        var prev = CurrentState;
        CurrentState = newState;
        CurrentState.Enter(prev);
    }
}