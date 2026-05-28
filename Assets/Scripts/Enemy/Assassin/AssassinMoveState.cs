using UnityEngine;

public class AssassinMoveState : EnemyState<Assassin>
{
    private float moveDir;
    private float moveTimer;
    private float attackMoveInterval = 0.5f;
    private float moveInterval = 2f;
    public AssassinMoveState(Assassin enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void Enter(EnemyState prevState)
    {
        moveDir = 0f;
        moveTimer = moveInterval;
        enemy.Animator.SetBool("Walk", true);
    }

    public override void Exit()
    {
        enemy.Animator.SetBool("Walk", false);
    }

    public override void PhysicsUpdate()
    {
        enemy.Motor.MoveHorizontal(moveDir);
    }

    public override void Update()
    {
        moveTimer += Time.deltaTime;
        if (enemy.Target != null)
        {
            if (enemy.Combat.SelectPattern() != null)
            {
                moveDir = enemy.Target.position.x - enemy.transform.position.x;
                if (Mathf.Abs(moveDir) > 0.05f)
                    moveDir = Mathf.Sign(moveDir);
                enemy.AllFlip(moveDir);
                stateMachine.ChangeState(enemy.attackState);
                return;
            }
            enemy.Animator.SetBool("Walk", true);
            if (moveTimer >= attackMoveInterval)
            {
                moveDir = enemy.Target.position.x - enemy.transform.position.x;
                if (Mathf.Abs(moveDir) > 0.05f)
                    moveDir = Mathf.Sign(moveDir);
                moveTimer = 0f;
            }
        }
        else
        {
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
        }
        enemy.AllFlip(moveDir);
    }
}
