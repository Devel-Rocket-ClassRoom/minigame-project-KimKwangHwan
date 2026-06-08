using System.Collections;
using UnityEngine;

public class SFXManager : Singleton<SFXManager>
{
    [Header("BGM")]
    [SerializeField] private float _crossfadeDuration = 1.5f;
    [SerializeField, Range(0f, 1f)] private float _bgmVolume = 0.8f;

    [Header("SFX")]
    [SerializeField] private int _sfxPoolSize = 16;
    [SerializeField, Range(0f, 1f)] private float _sfxVolume = 1f;
    [SerializeField] private int _maxSameSound = 3;

    private AudioSource _bgmA;
    private AudioSource _bgmB;
    private AudioSource _activeBgm;
    private Coroutine   _fadeRoutine;

    private AudioSource[] _sfxPool;
    private AudioClip[]   _sfxClips;   // 각 풀 슬롯의 현재 클립
    private int           _sfxIndex;   // 라운드로빈 포인터

    // ── 볼륨 프로퍼티 ────────────────────────────────────────────────

    public float BgmVolume
    {
        get => _bgmVolume;
        set
        {
            _bgmVolume = Mathf.Clamp01(value);
            if (_activeBgm != null && _activeBgm.isPlaying)
                _activeBgm.volume = _bgmVolume;
        }
    }

    public float SfxVolume
    {
        get => _sfxVolume;
        set => _sfxVolume = Mathf.Clamp01(value);
    }

    // ── 초기화 ───────────────────────────────────────────────────────

    protected override void Awake()
    {
        base.Awake();

        _bgmA = CreateBgmSource();
        _bgmB = CreateBgmSource();
        _activeBgm = _bgmA;

        _sfxPool  = new AudioSource[_sfxPoolSize];
        _sfxClips = new AudioClip[_sfxPoolSize];
        for (int i = 0; i < _sfxPoolSize; i++)
        {
            _sfxPool[i] = gameObject.AddComponent<AudioSource>();
            _sfxPool[i].loop = false;
            _sfxPool[i].playOnAwake = false;
        }
    }

    private AudioSource CreateBgmSource()
    {
        var src = gameObject.AddComponent<AudioSource>();
        src.loop = true;
        src.playOnAwake = false;
        src.volume = 0f;
        return src;
    }

    // ── BGM API ──────────────────────────────────────────────────────

    /// <summary>
    /// BGM을 재생합니다. 같은 클립이 이미 재생 중이면 아무것도 하지 않습니다.
    /// </summary>
    public void PlayBGM(AudioClip clip, bool crossfade = true)
    {
        if (clip == null)
        {
            StopBGM();
            return;
        }

        if (_activeBgm.clip == clip && _activeBgm.isPlaying)
            return;

        if (_fadeRoutine != null)
            StopCoroutine(_fadeRoutine);

        _fadeRoutine = crossfade
            ? StartCoroutine(CrossFade(clip))
            : null;

        if (!crossfade)
            SwitchImmediate(clip);
    }

    /// <summary>
    /// BGM을 정지합니다.
    /// </summary>
    public void StopBGM(bool fade = true)
    {
        if (_fadeRoutine != null)
            StopCoroutine(_fadeRoutine);

        _fadeRoutine = fade
            ? StartCoroutine(FadeOut(_activeBgm))
            : null;

        if (!fade)
        {
            _activeBgm.Stop();
            _activeBgm.volume = 0f;
        }
    }

    private void SwitchImmediate(AudioClip clip)
    {
        _bgmA.Stop();
        _bgmB.Stop();
        _activeBgm = _bgmA;
        _activeBgm.clip = clip;
        _activeBgm.volume = _bgmVolume;
        _activeBgm.Play();
    }

    private IEnumerator CrossFade(AudioClip nextClip)
    {
        AudioSource next = (_activeBgm == _bgmA) ? _bgmB : _bgmA;
        AudioSource prev = _activeBgm;

        next.clip = nextClip;
        next.volume = 0f;
        next.Play();

        float elapsed = 0f;
        float startVol = prev.volume;

        while (elapsed < _crossfadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / _crossfadeDuration;
            prev.volume = Mathf.Lerp(startVol, 0f, t);
            next.volume = Mathf.Lerp(0f, _bgmVolume, t);
            yield return null;
        }

        prev.Stop();
        prev.volume = 0f;
        next.volume = _bgmVolume;
        _activeBgm = next;
        _fadeRoutine = null;
    }

    private IEnumerator FadeOut(AudioSource source)
    {
        float startVol = source.volume;
        float elapsed = 0f;

        while (elapsed < _crossfadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(startVol, 0f, elapsed / _crossfadeDuration);
            yield return null;
        }

        source.Stop();
        source.volume = 0f;
        _fadeRoutine = null;
    }

    // ── SFX API ──────────────────────────────────────────────────────

    /// <summary>
    /// 효과음을 재생합니다. clip이 null이면 무시합니다.
    /// </summary>
    public void PlaySFX(AudioClip clip, float volumeScale = 1f, float pitch = 1f)
    {
        if (clip == null) return;
        if (CountPlaying(clip) >= _maxSameSound) return;

        int idx = PickSource();
        _sfxClips[idx] = clip;

        _sfxPool[idx].clip = clip;
        _sfxPool[idx].pitch = pitch;
        _sfxPool[idx].volume = _sfxVolume * volumeScale;
        _sfxPool[idx].Play();
    }

    // 현재 재생 중인 동일 클립 수를 카운트
    private int CountPlaying(AudioClip clip)
    {
        int count = 0;
        for (int i = 0; i < _sfxPool.Length; i++)
            if (_sfxPool[i].isPlaying && _sfxClips[i] == clip)
                count++;
        return count;
    }

    // !isPlaying 슬롯 우선 탐색, 없으면 라운드로빈 강제 탈취
    private int PickSource()
    {
        for (int i = 0; i < _sfxPool.Length; i++)
        {
            int j = (_sfxIndex + i) % _sfxPool.Length;
            if (!_sfxPool[j].isPlaying)
            {
                _sfxIndex = (j + 1) % _sfxPool.Length;
                return j;
            }
        }

        int steal = _sfxIndex;
        _sfxPool[steal].Stop();
        _sfxIndex = (steal + 1) % _sfxPool.Length;
        return steal;
    }
}
