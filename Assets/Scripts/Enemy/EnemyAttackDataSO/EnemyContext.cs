using System.Collections;
using UnityEngine;

public class EnemyContext
{
    public Transform self;
    public Transform target;
    public Rigidbody2D rb;
    public Animator anim;
    public MonoBehaviour runner;
    public Hitbox hitbox;
    public Transform muzzle;
    public EnemyAnimEvent animEvents;
    public bool SuperArmor;
    public float Facing => self.localScale.x;

    public IEnumerator WaitForAnimEvent(string eventName)
    {
        bool fired = false;
        System.Action handler = () => fired = true;
        animEvents.Subscribe(eventName, handler);
        yield return new WaitUntil(() => fired);
        animEvents.Unsubscribe(eventName, handler);
    }
}