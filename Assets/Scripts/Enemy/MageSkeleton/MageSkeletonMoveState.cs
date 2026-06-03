using UnityEngine;
using Random = UnityEngine.Random;
public class MageSkeletonMoveState : EnemyState<MageSkeleton>
{
    private float moveDir;
    private float moveTimer;
    private float attackMoveInterval = 0.5f;
    private float moveInterval = 2f;
    public MageSkeletonMoveState(MageSkeleton enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void Enter(EnemyState prevState)
    {
        moveDir = 0f;
        moveTimer = moveInterval;
        enemy.Animator.SetBool("Walk", true);
        enemy.Motor.ResumeControl();
    }

    public override void Exit()
    {
        enemy.Animator.SetBool("Walk", false);
    }

    public override void PhysicsUpdate()
    {
        if (moveDir != 0f && enemy.Motor.IsBlocked(moveDir))
        {
            moveDir = -moveDir;
            enemy.AllFlip(moveDir);
        }
        enemy.Motor.MoveHorizontal(moveDir);
    }

    public override void Update()
    {
        moveTimer += Time.deltaTime;
        if (enemy.Target != null)
        {
            float rawDist = enemy.Target.position.x - enemy.transform.position.x;
            float absDist = Mathf.Abs(rawDist);
            float dirToTarget = absDist > 0.05f ? Mathf.Sign(rawDist) : 0f;

            if (enemy.Combat.SelectPattern() != null)
            {
                moveDir = dirToTarget;
                enemy.AllFlip(moveDir);
                stateMachine.ChangeState(enemy.attackState);
                return;
            }

            // 쿨타임 중: 공격 사정거리 밖이면 접근, 사정거리 안이면 거리 유지
            if (absDist > enemy.AttackDistance)
            {
                if (moveTimer >= attackMoveInterval)
                {
                    moveDir = dirToTarget;
                    moveTimer = 0f;
                }
            }
            else if (absDist < enemy.AttackDistance * 0.6f)
            {
                // 너무 가까우면 뒤로 물러남
                moveDir = -dirToTarget;
            }
            else
            {
                // 적정 거리 - 제자리 유지
                moveDir = 0f;
            }

            enemy.Animator.SetBool("Walk", moveDir != 0f);
            if (moveDir != 0f)
                enemy.AllFlip(moveDir);
            else if (dirToTarget != 0f)
                enemy.AllFlip(dirToTarget);
        }
        else
        {
            enemy.Animator.SetBool("Walk", true);
            if (moveTimer >= moveInterval)
            {
                moveDir = Random.value >= 0.5f ? 1f : -1f;
                moveTimer = 0f;
                if (Random.value > 0.8f)
                {
                    stateMachine.ChangeState(enemy.idleState);
                    return;
                }
            }
            enemy.AllFlip(moveDir);
        }
    }
}
