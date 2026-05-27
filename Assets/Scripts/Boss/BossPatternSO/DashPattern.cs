using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "DashPattern", menuName = "BossPatterns/DashPattern")]
public class DashPattern : BossPattern
{
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 2f;
    [SerializeField] private string[] animStates;
    [SerializeField] private Vector2 hitboxSize;
    [SerializeField] private Vector2 hitboxOffset;
    [SerializeField] private float damage = 10f;
    public override IEnumerator Execute(BossContext ctx)
    {
        Vector2 dir = ctx.AllFlip();
        var rb = ctx.bossTransform.GetComponent<Rigidbody2D>();
        ctx.animator.Play(animStates[0]);

        yield return ctx.WaitForAnimEvent("HitboxOn");

        ctx.hitbox.Enable(damage, hitboxOffset, hitboxSize, 0f, 1f);
        ctx.animator.Play(animStates[1]);
        rb.linearVelocity = dir * dashSpeed;
        yield return new WaitForSeconds(dashDuration);

        ctx.animator.Play(animStates[2]);
        yield return ctx.WaitForAnimEvent("HitboxOff");
        ctx.hitbox.Disable();

        yield return ctx.WaitForAnimEvent("RecoveryEnd");
        yield return new WaitForSeconds(recoveryTime);
    }
}
