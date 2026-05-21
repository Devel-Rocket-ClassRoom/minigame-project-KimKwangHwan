using UnityEngine;

public class MageSkeletonAttackState : EnemyState<MageSkeleton>
{
    private AttackRuntime rt;
    private AttackData data;
    private AttackType typeAtEntry;
    private int comboIndexAtEntry;

    private float timer;
    private float t;
    private bool activeOn;
    private bool comboQueued;
    private bool aborted;
    private bool lungeApplied;
    public MageSkeletonAttackState(MageSkeleton enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void Enter(EnemyState prevState)
    {
        this.prevState = prevState;
        enemy.Motor.MoveStop();
        enemy.Animator.ResetTrigger("Attack1");
        rt = enemy.Combat.Runtime;
        typeAtEntry = enemy.Combat.BufferedType;
        enemy.Combat.BeginChain(typeAtEntry);
        //
        data = enemy.Combat.MoveSet.Resolve(typeAtEntry, )
    }

    public override void Exit()
    {
    }

    public override void PhysicsUpdate()
    {
    }

    public override void Update()
    {
    }


}
