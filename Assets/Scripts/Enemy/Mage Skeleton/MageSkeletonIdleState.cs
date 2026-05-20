using UnityEngine;
using Random = UnityEngine.Random;
public class MageSkeletonIdleState : EnemyState<MageSkeleton>
{
    private float idleTime;
    private float startIdleTime = 1f;
    private float endIdleTime = 3f;
    private float timer;
    public MageSkeletonIdleState(MageSkeleton enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void Enter(EnemyState prevState)
    {
        idleTime = Random.Range(startIdleTime, endIdleTime);
    }

    public override void Exit()
    {
    }

    public override void PhysicsUpdate()
    {
    }

    public override void Update()
    {
        if (enemy.Target != null)
        {
            stateMachine.ChangeState(enemy.moveState);
            return;
        }
        if (enemy.move)
        {
            timer += Time.deltaTime;
            if (timer >= idleTime)
            {
                stateMachine.ChangeState(enemy.moveState);
                timer = 0;
            }
        }
    }
}
