using UnityEngine;
using System.Collections;
public class BossContext
{
    public Transform bossTransform;
    public Transform playerTransform;
    public Animator animator;
    public BossAnimEvents animEvents;
    public Hitbox hitbox;
    public Transform muzzle;
    public int currentPhase;

    public Vector2 PlayerPos => playerTransform.position;
    public Vector2 BossPos => bossTransform.position;
    public Vector2 DirToPlayer => (PlayerPos - BossPos).normalized;
    public float DistToPlayer => Vector2.Distance(BossPos, PlayerPos);
    public bool PlayerIsRight => PlayerPos.x > BossPos.x;
    
    public Vector2 AllFlip()
    {
        Vector2 dir;
        if (PlayerIsRight)
        {
            dir = Vector2.right;
            bossTransform.localScale = new Vector3(1f, bossTransform.localScale.y, bossTransform.localScale.z);
        }
        else
        {
            dir = Vector2.left;
            bossTransform.localScale = new Vector3(-1f, bossTransform.localScale.y, bossTransform.localScale.z);
        }

        return dir;
    }

    public IEnumerator WaitForAnimEvent(string eventName)
    {
        bool fired = false;
        System.Action handler = () => fired = true;
        animEvents.Subscribe(eventName, handler);
        yield return new WaitUntil(() => fired);
        animEvents.Unsubscribe(eventName, handler);
    }
}
