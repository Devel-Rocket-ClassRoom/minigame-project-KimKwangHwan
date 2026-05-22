using UnityEngine;

public abstract class AttackData : ScriptableObject
{
    [Header("Animation / Timing")]
    public string animState;
    [Min(0.01f)] public float duration = 0.4f;

    [Header("Telegraph (normalized 0~1) — 전조")]
    [Range(0f, 1f)] public float telegraphStart = 0f;
    [Range(0f, 1f)] public float telegraphEnd = 0f;  // start==end면 전조 없음

    [Header("Active Window (normalized 0~1)")]
    [Range(0f, 1f)] public float activeStart = 0.2f;
    [Range(0f, 1f)] public float activeEnd = 0.5f;

    [Header("Cancel")]
    [Range(0f, 1f)] public float cancelWindowStart = 0.6f;

    [Header("Movement")]
    public bool lockHorizontal;
    public Vector2 lungeVelocity;       // x는 Facing으로 부호 결정
    public bool overrideGravity;
    public float gravityScale = 1f;

    // ── 실행 훅. 기본은 no-op, 필요한 것만 override ──
    public abstract void OnEnter(AttackRuntime rt);
    public abstract void OnActiveStart(AttackRuntime rt);
    public abstract void OnActiveEnd(AttackRuntime rt);
    public abstract void OnExit(AttackRuntime rt);
}