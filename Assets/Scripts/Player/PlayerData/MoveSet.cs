using UnityEngine;

[CreateAssetMenu(menuName = "Combat/MoveSet")]
public class MoveSet : ScriptableObject
{
    [SerializeField] AttackData[] groundedCombo;  // 1타→2타→3타
    [SerializeField] AttackData airborne;
    [SerializeField] AttackData running;

    public AttackData Resolve(AttackContext ctx, int comboIndex)
    {
        switch (ctx)
        {
            case AttackContext.Grounded:
                return groundedCombo[Mathf.Min(comboIndex, groundedCombo.Length - 1)];
            case AttackContext.Airborne: return airborne;
            case AttackContext.Running: return running;
        }
        return null;
    }
}