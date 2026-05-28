using UnityEngine;
using System.Collections.Generic;

public class EnemyAnimEvent : MonoBehaviour
{
    private Dictionary<string, System.Action> handlers = new();

    public void Subscribe(string name, System.Action h)
    {
        if (handlers.ContainsKey(name)) handlers[name] += h;
        else handlers[name] = h;
    }
    public void Unsubscribe(string name, System.Action h)
    {
        if (handlers.ContainsKey(name)) handlers[name] -= h;
    }

    public void AnimEvent_HitboxOn() => handlers.GetValueOrDefault("HitboxOn")?.Invoke();
    public void AnimEvent_HitboxOff() => handlers.GetValueOrDefault("HitboxOff")?.Invoke();
    public void AnimEvent_Telegraph() => handlers.GetValueOrDefault("TelegraphEnd")?.Invoke();
    public void AnimEvent_ProjectileFire() => handlers.GetValueOrDefault("ProjectileFire")?.Invoke();
    public void AnimEvent_Recovery() => handlers.GetValueOrDefault("RecoveryEnd")?.Invoke();
}
