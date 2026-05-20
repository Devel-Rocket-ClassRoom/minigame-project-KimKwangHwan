using UnityEngine;
using System.Collections;
public class PlayerDashState : PlayerState
{
    private float elapsed;
    private float originalGravity;
    private float dashDir;
    private Coroutine afterImageRoutine;

    public PlayerDashState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter(PlayerState prevState)
    {
        this.prevState = prevState;
        elapsed = 0f;
        dashDir = player.Facing;
        player.Motor.SuppressHorizontalControl = true;
        originalGravity = player.Motor.RB.gravityScale;
        player.Motor.RB.gravityScale = 0f;
        player.Motor.RB.linearVelocity = new Vector2(player.dashSpeed * dashDir, 0f);
        //if (!player.Motor.IsGrounded()) player.airDashLeft--;
        afterImageRoutine = player.StartCoroutine(SpawnAfterimages());
        player.Animator.SetBool("Dash", true);
    }

    public override void Exit()
    {
        player.Motor.RB.gravityScale = originalGravity;
        player.Motor.SuppressHorizontalControl = false;
        player.Motor.SetHorizontalVelocity(player.Motor.RB.linearVelocityX * 0.4f);
        player.lastDashTime = Time.time;
        player.Animator.SetBool("Dash", false);

        if (afterImageRoutine != null)
        {
            player.StopCoroutine(afterImageRoutine);
            afterImageRoutine = null;
        }
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

    private IEnumerator SpawnAfterimages()
    {
        var wait = new WaitForSeconds(player.afterImageInterval);
        while (true)
        {
            SpawnOne();
            yield return wait;
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
