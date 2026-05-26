using System.Collections.Generic;
using UnityEngine;
public class BossAnimEvents : MonoBehaviour
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

    // 애니메이션 클립의 이벤트가 여기로 호출됨
    public void AnimEvent_HitboxOn() => handlers.GetValueOrDefault("HitboxOn")?.Invoke();
    public void AnimEvent_HitboxOff() => handlers.GetValueOrDefault("HitboxOff")?.Invoke();
    public void AnimEvent_Telegraph() => handlers.GetValueOrDefault("TelegraphEnd")?.Invoke();
    public void AnimEvent_ProjectileFire() => handlers.GetValueOrDefault("ProjectileFire")?.Invoke();
    public void AnimEvent_Recovery() => handlers.GetValueOrDefault("RecoveryEnd")?.Invoke();
}
