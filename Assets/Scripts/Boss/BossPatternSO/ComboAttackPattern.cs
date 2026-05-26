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

    public override IEnumerator Execute(BossContext ctx)
    {
        int hitCount = ctx.currentPhase == 1 ? phase1HitCount : phase2HitCount;

        // 플레이어 방향으로 회전
        for (int i = 0; i < hitCount; i++)
        {
            int index = Mathf.Min(i, hitAnimStates.Length - 1);
            string clip = hitAnimStates[index];
            ctx.animator.Play(clip);
            yield return ctx.WaitForAnimEvent("HitboxOn");

            ctx.hitbox.Enable(damage, hitboxSizes[index], hitboxOffsets[index], 0f, 1f);

            yield return ctx.WaitForAnimEvent("HitboxOff");
            ctx.hitbox.Disable();

            if (i < hitCount - 1)
                yield return new WaitForSeconds(interHitDelay);
        }
        yield return new WaitForSeconds(recoveryTime);
    }
}
