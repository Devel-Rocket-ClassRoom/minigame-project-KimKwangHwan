using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private MoveSet startingMoveSet;
    [SerializeField] private Hitbox hitbox;
    [SerializeField] private Transform muzzleOrigin;
    private PlayerStamina playerStamina;
    private PlayerMotor motor;
    [SerializeField] private Animator animator;
    [SerializeField] private Vector2 hitboxSize;
    [SerializeField] private Vector2 hitboxOffset;
    public MoveSet MoveSet { get; private set; }
    public int ComboIndex { get; private set; }
    public float Facing => transform.localScale.x >= 0f ? 1f : -1f;

    public bool IsAttacking { get; set; }

    public AttackRuntime Runtime { get; private set; }

    public bool AttackBuffered { get; private set; }
    public AttackType BufferedType { get; private set; }
    [SerializeField] float bufferDuration = 0.15f;
    float _bufferedAt;
    [SerializeField] float comboResetTime = 0.6f;
    float _lastAttackEndTime;
    AttackType _chainType;

    MoveSet _pendingMoveSet;
    public event System.Action<MoveSet> OnMoveSetChanged;

    private void Awake()
    {
        MoveSet = startingMoveSet;
        motor = GetComponent<PlayerMotor>();
        playerStamina = GetComponent<PlayerStamina>();
        Runtime = new AttackRuntime
        {
            Motor = motor,
            Hitbox = hitbox,
            MuzzleOrigin = muzzleOrigin,
            Facing = 1f
        };
    }

    private void Update()
    {
        if (AttackBuffered && Time.time - _bufferedAt > bufferDuration)
            AttackBuffered = false;
        if (!IsAttacking && Time.time - _lastAttackEndTime > comboResetTime)
            ComboIndex = 0;
    }
    public void OnMeleeInput(InputAction.CallbackContext c)
    {
        if (c.started) Buffer(AttackType.Melee);
    }
    public void OnRangedInput(InputAction.CallbackContext c)
    {
        if (c.started) Buffer(AttackType.Ranged);
    }

    void Buffer(AttackType type)
    {
        AttackBuffered = true;
        BufferedType = type;
        _bufferedAt = Time.time;
    }

    public void ConsumeBuffer() => AttackBuffered = false;
    public void BeginChain(AttackType type)
    {
        if (type != _chainType)
        {
            ComboIndex = 0;
            _chainType = type;
        }
        if (!MoveSet.HasComboAt(type, AttackContext.Grounded, ComboIndex) && !MoveSet.HasComboAt(type, AttackContext.Airborne, ComboIndex))
        {
            ComboIndex = 0;
        }
    }

    public void AdvanceCombo() => ComboIndex++;
    // 콤보 마지막 또는 다른 타입 공격으로 끊을 때 — 즉시 0으로 리셋
    public void NotifyAttackEnded()
    {
        _lastAttackEndTime = Time.time;
        ComboIndex = 0;
    }
    // 콤보가 남아있는데 자연 종료된 경우 — ComboIndex 보존, comboResetTime 타이머만 시작
    public void NotifyAttackPaused()
    {
        _lastAttackEndTime = Time.time;
    }

    public void RequestEquip(MoveSet next)
    {
        if (IsAttacking)
        {
            _pendingMoveSet = next;
            return;
        }
        ApplyEquip(next);
    }
    public void FlushPendingEquip()
    {
        if (_pendingMoveSet == null) return;
        ApplyEquip(_pendingMoveSet);
        _pendingMoveSet = null;
    }
    private void ApplyEquip(MoveSet next)
    {
        MoveSet = next;
        ComboIndex = 0;
        OnMoveSetChanged?.Invoke(next);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube((Vector2)hitbox.transform.position + hitboxOffset, hitboxSize);
    }
}
