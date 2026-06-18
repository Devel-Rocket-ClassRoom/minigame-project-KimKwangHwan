using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading;
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

    public async UniTask WaitForAnimEvent(string eventName, CancellationToken ct)
    {
        bool fired = false;
        System.Action handler = () => fired = true;
        animEvents.Subscribe(eventName, handler);
        await UniTask.WaitUntil(() => fired, cancellationToken: ct);
        animEvents.Unsubscribe(eventName, handler);
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

    public async UniTask Wait()
    {
        await UniTask.WaitUntil(() => Fired);
        OnComplete?.Invoke();
    }
}