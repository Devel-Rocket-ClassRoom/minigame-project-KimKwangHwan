using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
public class PlayerDashState : PlayerState
{
    private float elapsed;
    private float originalGravity;
    private float dashDir;
    private float needStamina = 10f;
    private bool isCancel = false;
    private CancellationTokenSource afterImageCts;
    public PlayerDashState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter(PlayerState prevState)
    {
        this.prevState = prevState;
        if (!player.Stamina.TryUseStamina(needStamina))
        {
            isCancel = true;
            stateMachine.ChangeState(prevState);
            return;
        }
        player.HurtBox.DoInvincible();
        elapsed = 0f;
        dashDir = player.Facing;
        player.Motor.SuppressHorizontalControl = true;
        originalGravity = player.Motor.RB.gravityScale;
        player.Motor.RB.gravityScale = 0f;
        player.Motor.RB.linearVelocity = new Vector2(player.dashSpeed * dashDir, 0f);
        //if (!player.Motor.IsGrounded()) player.airDashLeft--;

        afterImageCts = new CancellationTokenSource();
        SpawnAfterimages(afterImageCts.Token).Forget();

        player.Animator.SetBool("Dash", true);
        SFXManager.Instance.PlaySFX(player.dashClip);
    }

    public override void Exit()
    {
        if (isCancel)
        {
            isCancel = false;
            return;
        }
        player.Motor.RB.gravityScale = originalGravity;
        player.Motor.SuppressHorizontalControl = false;

        afterImageCts?.Cancel();
        afterImageCts?.Dispose();
        afterImageCts = null;

        player.Motor.SetHorizontalVelocity(player.Motor.RB.linearVelocityX * 0.4f);
        player.lastDashTime = Time.time;
        player.Animator.SetBool("Dash", false);
        player.HurtBox.CancelInvincible();
    }

    public override void HandleInput()
    {
    }

    public override void PhysicsUpdate()
    {
    }

    public override void Update()
    {
        elapsed += Time.deltaTime;

        if (elapsed >= player.dashDuration)
        {
            if (player.Motor.IsGrounded())
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.fallState);
        }
    }

    private async UniTask SpawnAfterimages(CancellationToken ct)
    {
        int wait = (int)(player.afterImageInterval * 1000);

        while (true)
        {
            ct.ThrowIfCancellationRequested();
            SpawnOne();
            await UniTask.Delay(wait, cancellationToken: ct);
        }
    }

    private void SpawnOne()
    {
        var go = Object.Instantiate(player.afterImagePrefab);
        var img = go.GetComponent<AfterImage>();
        img.Init(
            sprite: player.spriteRenderer.sprite,
            position: player.transform.position,
            scale: player.transform.lossyScale,
            color: player.afterImageColor,
            life: player.afterImageLifetime
        );
    }
}
