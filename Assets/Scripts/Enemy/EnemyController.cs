using UnityEngine;

public class MageSkeletonEnemyController : MonoBehaviour
{
    protected Animator animator;
    public Animator Animator { get { return animator; } }
    public float Facing { get { return Mathf.Sign(transform.localScale.x); } }
    [SerializeField]
    protected EnemyMotor enemyMotor;
    [SerializeField]
    protected EnemyCombat enemyCombat;
    public EnemyMotor Motor { get { return enemyMotor; } }
    public EnemyCombat Combat { get { return enemyCombat; } }
    protected float moveDirection;
    protected EnemyStateMachine stateMachine;
    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();
        moveDirection = 1f;
    }

    public virtual void AllFlip(float x)
    {
        if (x != 0f)
        {
            if (moveDirection * x < 0f)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            moveDirection = x > 0f ? 1f : -1f;
        }
    }
    protected virtual void Update()
    {
        stateMachine.CurrentState.HandleInput();
        stateMachine.CurrentState.Update();
    }
    protected virtual void FixedUpdate()
    {
        stateMachine.CurrentState.PhysicsUpdate();
    }
}
