using UnityEngine;

public abstract class EnemyState
{
    protected EnemyController enemy;
    protected EnemyStateMachine stateMachine;
    protected EnemyState prevState;

    protected EnemyState(EnemyController enemy, EnemyStateMachine stateMachine)
    {
        this.enemy = enemy;
        this.stateMachine = stateMachine;
    }
    public abstract void Enter(EnemyState prevState);
    public abstract void Update();
    public abstract void Exit();
    public abstract void HandleInput();
    public abstract void PhysicsUpdate();
}
