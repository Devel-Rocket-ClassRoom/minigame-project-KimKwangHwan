using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ComboAttackPattern", menuName = "BossPatterns/ComboAttackPattern")]
public class ComboAttackPattern : BossPattern
{
    [Header("Combo")]
    [SerializeField] private int phase1HitCount = 2;
    [SerializeField] private int phase2HitCount = 3;
    [SerializeField] private float interHitDelay = 0.2f;

    [Header("Hits")]
    [SerializeField] private string[] hitAnimStates;
    [SerializeField] private Vector2[] hitboxSizes;
    [SerializeField] private Vector2[] hitboxOffsets;
    [SerializeField] private float damage = 10f;

    public AudioClip[] attackClip;
    public AudioClip[] hitClip;
    public override IEnumerator Execute(BossContext ctx)
    {
        int hitCount = ctx.currentPhase == 1 ? phase1HitCount : phase2HitCount;
        Rigidbody2D rb = ctx.bossTransform.GetComponent<Rigidbody2D>();
        for (int i = 0; i < hitCount; i++)
        {
            int index = Mathf.Min(i, hitAnimStates.Length - 1);
            string clip = hitAnimStates[index];
            ctx.animator.Play(clip);
            SFXManager.Instance.PlaySFX(attackClip[index]);
            yield return ctx.WaitForAnimEvent("HitboxOn");
            Vector2 dir = ctx.AllFlip();
            ctx.hitbox.Enable(damage, hitboxOffsets[index], hitboxSizes[index], 0f, 1f, hitClip[index]);
            rb.linearVelocityX = dir.x * 5f;
            yield return ctx.WaitForAnimEvent("HitboxOff");
            ctx.hitbox.Disable();
            rb.linearVelocityX = 0f;
            if (i < hitCount - 1)
                yield return new WaitForSeconds(interHitDelay);
        }
        yield return new WaitForSeconds(recoveryTime);
    }
}
