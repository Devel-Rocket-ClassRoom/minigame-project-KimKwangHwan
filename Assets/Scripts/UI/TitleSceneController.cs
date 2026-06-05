using System.Collections;
using UnityEngine;

public class TitleSceneController : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "MainScene";
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    private bool _isTransitioning;

    private void Start()
    {
        StartCoroutine(FadeController.Instance.FadeIn(fadeInDuration));
    }

    // 시작 버튼 OnClick()에 연결
    public void OnStartGameClick()
    {
        if (_isTransitioning) return;
        _isTransitioning = true;
        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        yield return FadeController.Instance.FadeOut(fadeOutDuration);
        GameInitializer.Instance.LoadGameScene(gameSceneName);
    }
}
