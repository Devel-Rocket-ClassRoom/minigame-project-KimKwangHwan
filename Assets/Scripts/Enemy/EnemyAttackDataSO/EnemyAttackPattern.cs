using UnityEngine;
using System.Collections;
[CreateAssetMenu(fileName = "EnemyAttackPattern", menuName = "Enemy/EnemyAttackPattern")]
public abstract class EnemyAttackPattern : ScriptableObject
{
    [Header("공통")]
    public float cooldown = 1f;
    public float range = 3f;
    public int priority = 0;

    public virtual bool CanExecute(EnemyContext ctx)
    {
        float dist = Vector2.Distance(ctx.self.position, ctx.target.position);
        return dist <= range;
    }

    public abstract IEnumerator Execute(EnemyContext ctx);
}
