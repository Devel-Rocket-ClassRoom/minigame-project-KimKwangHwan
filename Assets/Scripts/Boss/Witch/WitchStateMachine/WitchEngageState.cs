public class WitchEngageState : EnemyState<Witch>
{
    public WitchEngageState(Witch enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine) { }

    public override void Enter(EnemyState prevState) { }
    public override void Exit() { }
    public override void Update() { }
    public override void PhysicsUpdate() { }
}
