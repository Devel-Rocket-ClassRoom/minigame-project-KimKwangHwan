using System.Collections;
using UnityEngine;

public class GameOverUI : MonoBehaviour
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

    private void OnEnable()
    {
        PlayerManager.OnPlayerSet += OnPlayerSet;
    }

    private void OnDisable()
    {
        PlayerManager.OnPlayerSet -= OnPlayerSet;
    }

    private void OnPlayerSet(PlayerController player)
    {
        player.Health.OnDead += Show;
    }

    private void Show()
    {
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        yield return new WaitForSecondsRealtime(delayBeforeFade);


        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    public void OnRestartClick()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        GameInitializer.Instance.Restart();
    }
}
