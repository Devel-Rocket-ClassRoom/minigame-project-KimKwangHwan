using UnityEngine;

public abstract class EnemyState
{
    protected EnemyStateMachine stateMachine;
    protected EnemyState prevState;
    public abstract void Enter(EnemyState prevState);
    public abstract void Update();
    public abstract void Exit();
    public abstract void PhysicsUpdate();
}
public abstract class EnemyState<T> : EnemyState where T : EnemyController
{
    protected T enemy;

    protected EnemyState(T enemy, EnemyStateMachine stateMachine)
    {
        this.enemy = enemy;
        this.stateMachine = stateMachine;
    }
}