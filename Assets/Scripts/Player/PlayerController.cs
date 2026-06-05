using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerInputReader)), RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerInputReader playerInput;
    [SerializeField] private PlayerMotor playerMotor;
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerStamina playerStamina;
    [SerializeField] private Animator animator;
    [SerializeField] private HurtBox hurtBox;
    [SerializeField] private Inventory inventory;
    private PlayerStateMachine stateMachine;
    public Inventory Inventory => inventory;
    public PlayerInputReader Input => playerInput;
    public PlayerMotor Motor => playerMotor;
    public PlayerCombat Combat => playerCombat;
    public PlayerHealth Health => playerHealth;
    public PlayerStamina Stamina => playerStamina;
    public HurtBox HurtBox => hurtBox;
    public PlayerState State => stateMachine.CurrentState;
    public Animator Animator => animator;
    public float Facing => Mathf.Sign(transform.localScale.x);

    public PlayerIdleState idleState;
    public PlayerRunState runState;
    public PlayerAirState airState;
    public PlayerJumpState jumpState;
    public PlayerFallState fallState;
    public PlayerWallClimbState wallClimbState;
    public PlayerAttackState attackState;
    public PlayerDashState dashState;
    public PlayerHurtState hurtState;
    public PlayerDeathState deathState;

    private float moveDirection; // +면 오른쪽, -면 왼쪽
    public float dashSpeed = 20f;
    public float dashDuration = 0.7f;
    public float dashCooldown = 0.5f;
    [HideInInspector] public float lastDashTime = -Mathf.Infinity;
    [HideInInspector] public int airDashLeft;
    public GameObject afterImagePrefab;
    public SpriteRenderer spriteRenderer; 
    public float afterImageInterval = 0.04f;
    public float afterImageLifetime = 0.3f;
    public Color afterImageColor = new Color(0.5f, 0.8f, 1f, 0.6f);

    public float hurtDuration = 0.8f;
    public float hurtEscapeTime = 0.4f;
    private Coroutine _blinkCoroutine;
    private Color _originalColor;
    private MaterialPropertyBlock _mpb;
    private static readonly int FlashID = Shader.PropertyToID("_FlashAmount");

    private void Awake()
    {
        stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(this, stateMachine);
        runState = new PlayerRunState(this, stateMachine);
        airState = new PlayerAirState(this, stateMachine);
        jumpState = new PlayerJumpState(this, stateMachine);
        fallState = new PlayerFallState(this, stateMachine);
        wallClimbState = new PlayerWallClimbState(this, stateMachine);
        attackState = new PlayerAttackState(this, stateMachine);
        dashState = new PlayerDashState(this, stateMachine);
        hurtState = new PlayerHurtState(this, stateMachine);
        deathState = new PlayerDeathState(this, stateMachine);

        stateMachine.Initialize(idleState);
        moveDirection = 1f;
        playerHealth.OnDamaged += GetHurt;
        playerHealth.OnDead += Dead;

        _originalColor = new Color();
        _originalColor = spriteRenderer.color;
        _mpb = new MaterialPropertyBlock();
    }
    private void SetFlash(float amount)
    {
        spriteRenderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat(FlashID, amount);
        spriteRenderer.SetPropertyBlock(_mpb);
    }
    private void Update()
    {
        stateMachine.CurrentState.HandleInput();
        stateMachine.CurrentState.Update();
        if (Input.UseItemPressed)
        {
            inventory.TryUseHealItem(Health);
        }
    }
    private void FixedUpdate()
    {
        stateMachine.CurrentState.PhysicsUpdate();
    }
    void OnEnable() => PlayerManager.Instance?.Set(this);
    void OnDisable() => PlayerManager.Instance?.Clear(this);
    public void AllFlip(float x)
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
    public void WallFlip(float wallDirection) // wallDirection: 벽이 있는 방향 (1 or -1)
    {
        AllFlip(-wallDirection); // 벽 반대 방향을 바라봄
    }
    public bool CanDash()
    {
        if (stateMachine.CurrentState is PlayerDashState) return false;
        if (Time.time < lastDashTime + dashCooldown) return false;
        return true;
    }
    protected virtual void GetHurt(float damage)
    {
        stateMachine.ChangeState(hurtState);
        StartBlink();
    }

    public void StartBlink(float interval = 0.1f, int count = 2)
    {
        if (_blinkCoroutine == null)
            _originalColor = spriteRenderer.color;
        else
        {
            StopCoroutine(_blinkCoroutine);
            spriteRenderer.color = _originalColor;
        }
        _blinkCoroutine = StartCoroutine(BlinkRoutine(interval, count));
    }

    private IEnumerator BlinkRoutine(float interval, int count)
    {
        var wait = new WaitForSeconds(interval);
        for (int b = 0; b < count; b++)
        {
            SetFlash(1f);
            yield return wait;
            SetFlash(0f);
            yield return wait;
        }
        _blinkCoroutine = null;
    }
    private void Dead()
    {
        stateMachine.ChangeState(deathState);
    }
    public void AllRecovery()
    {
        Health.Heal(Health.MaxHp);
        Inventory.AddAmmo(Inventory.MaxAmmo);
        Inventory.AddHealItem(Inventory.MaxHealItems);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "DeadZone")
            playerHealth.TakeDamage(Health.MaxHp);
    }
}
