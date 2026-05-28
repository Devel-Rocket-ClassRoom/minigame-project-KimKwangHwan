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
        Debug.Log(eventName);
    }
    public AnimEventAwaiter ArmEvent(string eventName)
    {
        var awaiter = new AnimEventAwaiter();
        System.Action handler = () => awaiter.Fired = true;
        animEvents.Subscribe(eventName, handler);
        awaiter.OnComplete = () => animEvents.Unsubscribe(eventName, handler);
        return awaiter;
    }
}
public class AnimEventAwaiter
{
    public bool Fired { get; set; }
    public System.Action OnComplete { get; set; }

    public IEnumerator Wait()
    {
        yield return new WaitUntil(() => Fired);
        OnComplete?.Invoke();
    }
}