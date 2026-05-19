
using System.Runtime.CompilerServices;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class PlayerAttackState : PlayerState
{
    private AttackRuntime _rt;
    private AttackData _data;
    private AttackType _typeAtEntry;
    private AttackContext _ctxAtEntry;
    private int _comboIndexAtEntry;

    private float _timer;
    private float _t;
    private bool _activeOn;
    private bool _comboQueued;
    private bool _aborted;
    private bool _lungeApplied;

    public PlayerAttackState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        Debug.Log("Enter AttackState");
        player.Animator.ResetTrigger("Attack");
        _rt = player.Combat.Runtime;
        _typeAtEntry = player.Combat.BufferedType;
        _ctxAtEntry = ResolveContext();
        player.Combat.BeginChain(_typeAtEntry);
        _comboIndexAtEntry = player.Combat.ComboIndex;

        _data = player.Combat.MoveSet.Resolve(_typeAtEntry, _ctxAtEntry, _comboIndexAtEntry);

        if (_data == null)
        {
            _aborted = true;
            ExitToLocomotion();
            return;
        }

        player.Combat.IsAttacking = true;
        _timer = 0f;
        _t = 0f;
        _activeOn = false;
        _comboQueued = false;
        _aborted = false;
        _lungeApplied = false;

        _rt.Facing = player.Combat.Facing;

        player.Combat.ConsumeBuffer();
        //player.Animator.Play(_data.animState);

        var anim = player.Animator;
        Debug.Log($"AttackType: {(int)_typeAtEntry}");
        anim.SetInteger("AttackType", (int)_typeAtEntry);
        anim.SetInteger("ComboIndex", _comboIndexAtEntry);
        anim.SetBool("IsGrounded", player.Motor.IsGrounded());
        anim.SetTrigger("Attack");
        //if (_data.overrideGravity)
        //    player.Motor.SetGravityScale(_data.gravityScale);
        _data.OnEnter(_rt);
    }

    public override void HandleInput()
    {
        if (_aborted) return;
        if (!_comboQueued && _t >= _data.cancelWindowStart && player.Combat.AttackBuffered)
        {
            _comboQueued = true;
        }
    }

    public override void PhysicsUpdate()
    {
        if (_aborted || _data == null) return;

        //if (_data.lockHorizontal)
        //    player.Motor.SetVelocityX(0f);

        if (!_lungeApplied && _data.lungeVelocity != Vector2.zero)
        {
            //player.Motor.SetVelocity(new Vector2(
            //    _data.lungeVelocity.x * _rt.Facing,
            //    _data.lungeVelocity.y));
            _lungeApplied = true;
        }
    }

    public override void Update()
    {
        if (_aborted) return;

        _timer += Time.deltaTime;
        _t = _timer / _data.duration;

        UpdateActiveWindow(_t);

        if (_t < 1f) return;

        if (_comboQueued && HasNextCombo())
        {
            player.Combat.AdvanceCombo();
            player.Combat.ConsumeBuffer();
            stateMachine.ChangeState(player.attackState);
        }
        else
        {
            player.Combat.NotifyAttackEnded();
            ExitToLocomotion();
        }
    }
    public override void Exit()
    {
        if (_data != null)
        {
            _data.OnExit(_rt);
            //if (_data.overrideGravity)
            //    player.Motor.RestoreGravity();
        }

        player.Combat.IsAttacking = false;

        // 콤보 재진입이면 무브셋 교체 보류
        if (!_comboQueued)
            player.Combat.FlushPendingEquip();
    }
    private AttackContext ResolveContext()
    {
        if (!player.Motor.IsGrounded()) return AttackContext.Airborne;
        // 달리는 중일 때 공격 추가
        return AttackContext.Grounded;
    }
    private bool HasNextCombo() => player.Combat.MoveSet.HasComboAt(_typeAtEntry, _ctxAtEntry, _comboIndexAtEntry + 1);
    private void UpdateActiveWindow(float t)
    {
        bool shouldBeOn = t >= _data.activeStart && t <= _data.activeEnd;
        if (shouldBeOn == _activeOn) return;

        _activeOn = shouldBeOn;
        if (shouldBeOn) _data.OnActiveStart(_rt);
        else _data.OnActiveEnd(_rt);
    }
    void ExitToLocomotion()
    {
        stateMachine.ChangeState(player.Motor.IsGrounded() ? player.idleState : player.fallState);
    }
}
