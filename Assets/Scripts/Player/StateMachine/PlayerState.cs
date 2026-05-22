public abstract class PlayerState
{
    protected PlayerController player;
    protected PlayerStateMachine stateMachine;
    protected PlayerState prevState;
    protected PlayerState(PlayerController player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public abstract void Enter(PlayerState prevState);
    public abstract void Update();
    public abstract void Exit();
    public abstract void HandleInput();
    public abstract void PhysicsUpdate();
}
