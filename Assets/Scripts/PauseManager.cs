using System;
using UnityEngine;

public class PauseManager : Singleton<PauseManager>
{
    public static event Action<bool> OnPauseChanged;

    public bool IsPaused { get; private set; }

    public void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        OnPauseChanged?.Invoke(true);
    }

    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        OnPauseChanged?.Invoke(false);
    }

    public void Toggle()
    {
        if (IsPaused) Resume();
        else Pause();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        // 씬 전환 시 timeScale 초기화
        Time.timeScale = 1f;
    }
}
