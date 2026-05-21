using UnityEngine;

[CreateAssetMenu(fileName = "AttackSelector", menuName = "Scriptable Objects/AttackSelector")]
public abstract class EnemyAttackSelector : ScriptableObject
{
    public abstract AttackData Pick(EnemyAttackContext ctx);
}
public struct EnemyAttackContext
{
    public float distanceToPlayer;
    public float healthRatio;
    public int phase;
    public AttackData lastAttack;
    public float timeSinceLastAttack;
}