using UnityEngine;

public enum AttackContext { Grounded, Airborne, Running }

[CreateAssetMenu(menuName = "Combat/AttackData")]
public class AttackData : ScriptableObject
{
    public string animTrigger;        // 또는 AnimationClip 직접 참조
    public float duration;            // 상태 유지 시간 (애니 이벤트로 대체 가능)
    public float damage;

    [Header("Hitbox")]
    public Vector2 hitboxOffset;
    public Vector2 hitboxSize;
    public float activeStart;         // 히트박스 켜지는 시점 (정규화 0~1 or 초)
    public float activeEnd;

    [Header("Movement")]
    public bool lockHorizontal;       // 지상 약공격: 멈춤
    public Vector2 lungeVelocity;     // 대시 공격: 앞으로 밀림
    public bool overrideGravity;      // 공중 공격: 부유 등
    public float gravityScale;

    [Header("Cancel")]
    public float cancelWindowStart;   // 다음 콤보/행동으로 캔슬 가능 구간
}