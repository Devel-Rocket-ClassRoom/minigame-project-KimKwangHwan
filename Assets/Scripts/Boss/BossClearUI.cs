using Cysharp.Threading.Tasks;
using UnityEngine;
using System;

public class BossClearUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float delayBeforeFade = 0.8f;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public void Show() => FadeInRoutine().Forget();

    private async UniTask FadeInRoutine()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delayBeforeFade), DelayType.UnscaledDeltaTime);
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            await UniTask.Yield();
        }

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    public void OnTitleClick()
    {
        GameInitializer.Instance.QuitToTitle();
    }
}
