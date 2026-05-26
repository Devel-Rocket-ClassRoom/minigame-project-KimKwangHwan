using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "CometDivePattern", menuName = "BossPatterns/CometDivePattern")]
public class CometDivePattern : BossPattern
{
    [SerializeField] private float diveSpeed = 10f;
    [SerializeField] private float diveHeight = 5f;

    [SerializeField] private string[] sequenceAnimStates;
    [SerializeField] private Vector2[] hitboxSizes;
    [SerializeField] private Vector2[] hitboxOffsets;
    [SerializeField] private float damage = 10f;

    [SerializeField] private int hitCount = 3;
    [SerializeField] private float interHitDelay = 0.2f;

    public override IEnumerator Execute(BossContext ctx)
    {
        for (int i = 0; i < hitCount; i++)
        {
            var rb = ctx.bossTransform.GetComponent<Rigidbody2D>();
            ctx.bossTransform.position = new Vector2(ctx.PlayerPos.x, diveHeight);
            ctx.animator.Play(sequenceAnimStates[0]);
            rb.gravityScale = 0f;
            yield return ctx.WaitForAnimEvent("TelegraphEnd");
            ctx.animator.Play(sequenceAnimStates[1]);
            rb.gravityScale = 1f;
            rb.linearVelocity = Vector2.down * diveSpeed;
            yield return ctx.WaitForAnimEvent("HitboxOn");
            ctx.hitbox.Enable(damage, hitboxOffsets[0], hitboxSizes[0], 0f, 1f);
            yield return new WaitUntil(() => ctx.bossTransform.GetComponent<Witch>().IsGrounded);
            yield return ctx.WaitForAnimEvent("HitboxOff");
            ctx.hitbox.Disable();

            ctx.animator.Play(sequenceAnimStates[2]);
            rb.linearVelocity = Vector2.zero;
            yield return ctx.WaitForAnimEvent("HitboxOn");
            ctx.hitbox.Enable(damage, hitboxOffsets[1], hitboxSizes[1], 0f, 1f);
            yield return ctx.WaitForAnimEvent("HitboxOff");
            ctx.hitbox.Disable();
            yield return ctx.WaitForAnimEvent("RecoveryEnd");
            yield return new WaitForSeconds(interHitDelay);
        }
        yield return new WaitForSeconds(recoveryTime);
    }
}
