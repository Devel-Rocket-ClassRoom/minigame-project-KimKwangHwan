using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : Singleton<FadeController>
{
    [SerializeField] private float defaultDuration = 0.5f;

    private CanvasGroup _canvasGroup;

    protected override void Awake()
    {
        base.Awake();
        BuildOverlay();
    }

    private void BuildOverlay()
    {
        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;
        gameObject.AddComponent<GraphicRaycaster>();

        _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = false;

        var overlayObj = new GameObject("FadeOverlay");
        overlayObj.transform.SetParent(transform, false);

        var image = overlayObj.AddComponent<Image>();
        image.color = Color.black;
        image.raycastTarget = false;

        var rect = overlayObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
    }

    // 검은 화면 → 투명 (씬 드러내기)
    public async UniTask FadeIn(float duration = -1f)
    {
        if (duration < 0f) duration = defaultDuration;
        _canvasGroup.blocksRaycasts = true;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            _canvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / duration);
            await UniTask.Yield();
        }
        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
    }

    // 투명 → 검은 화면 (씬 가리기)
    public async UniTask FadeOut(float duration = -1f)
    {
        if (duration < 0f) duration = defaultDuration;
        _canvasGroup.blocksRaycasts = false;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
            await UniTask.Yield();
        }
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
    }
}
