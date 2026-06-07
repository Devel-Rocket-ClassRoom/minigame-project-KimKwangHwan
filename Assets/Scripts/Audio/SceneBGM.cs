using UnityEngine;

public class SceneBGM : MonoBehaviour
{
    [SerializeField] private AudioClip _bgmClip;
    [SerializeField] private bool _crossfade = false;  // 타이틀은 즉시 시작이 자연스러움

    private void Start()
    {
        SFXManager.Instance?.PlayBGM(_bgmClip, false);
    }

    private void OnDestroy()
    {
        SFXManager.Instance?.StopBGM(false);
    }
}