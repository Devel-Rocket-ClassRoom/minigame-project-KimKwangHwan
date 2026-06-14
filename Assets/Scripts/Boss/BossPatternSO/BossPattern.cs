using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "BossPattern", menuName = "Boss/BossPattern")]
public abstract class BossPattern : ScriptableObject
{
    [Header("Identification")]
    public string patternName;

    [Header("Selection")]
    public int unlockPhase = 1;
    public float cooldown = 3f;
    [Range(0f, 10f)] public float weight = 1f;
    public float minDistance = 0f;
    public float maxDistance = 999f;

    [Header("Animation")]
    public string animStateName;

    [Header("Timing (seconds)")]
    public float telegraphTime = 0.5f;
    public float recoveryTime = 0.8f;

    public PatternType patternType;

    // 런타임 상태
    [System.NonSerialized] public float lastUsedTime = -999f;

    public bool IsReady(float now) => now - lastUsedTime >= cooldown;

    public abstract UniTask Execute(BossContext ctx, CancellationToken ct = default);
}
public enum PatternType
{
    None,
    Random,
    Ranged,
    Melee,
    Dash,
}
