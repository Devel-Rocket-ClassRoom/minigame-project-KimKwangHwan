using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] protected MoveSet startingMoveSet;
    [SerializeField] protected Animator animator;
    [SerializeField] protected float bufferDuration = 0.15f;
    protected EnemyMotor motor;
    public MoveSet MoveSet { get; protected set; }
    public float Facing => transform.localScale.x >= 0f ? 1f : -1f;
    public bool IsAttacking { get; set; }
    public AttackRuntime Runtime { get; protected set; }
    public bool AttackBuffered { get; protected set; }
    public AttackType BufferedType { get; protected set; }
    protected float bufferedAt;
    protected MoveSet pendingMoveSet;
    protected float lastAttackEndTime;
    protected AttackType chainType;

    protected virtual void Awake()
    {
        MoveSet = startingMoveSet;
        motor = GetComponent<EnemyMotor>();
    }
    protected virtual void Buffer(AttackType type)
    {
        AttackBuffered = true;
        BufferedType = type;
        bufferedAt = Time.time;
    }
    public virtual void ConsumeBuffer() => AttackBuffered = false;
    public virtual void BeginChain(AttackType type)
    {
        if (type != chainType)
        {
            chainType = type;
        }
    }
    public virtual void NotifyAttackEnded()
    {
        lastAttackEndTime = Time.time;
    }
}
