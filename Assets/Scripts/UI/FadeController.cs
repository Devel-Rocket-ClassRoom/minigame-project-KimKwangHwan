using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : Singleton<FadeController>
{
    [SerializeField] private float defaultDuration = 0.5f;

    private CanvasGroup _canvasGroup;
    private CanvasGroup _whiteGroup;

    protected override void Awake()
    {
        base.Awake();
        BuildOverlay();
        BuildWhiteOverlay();
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

    private void BuildWhiteOverlay()
    {
        var whiteObj = new GameObject("WhiteOverlay");
        whiteObj.transform.SetParent(transform, false);
        var image = whiteObj.AddComponent<Image>();
        image.color = Color.white;
        image.raycastTarget = false;
        var rect = whiteObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
        _whiteGroup = whiteObj.AddComponent<CanvasGroup>();
        _whiteGroup.alpha = 0f;
        _whiteGroup.blocksRaycasts = false;
        _whiteGroup.interactable = false;
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

    public async UniTask FlashWhite(float duration = 0.3f)
    {
        float half = duration * 0.5f;
        float elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.unscaledDeltaTime;
            _whiteGroup.alpha = Mathf.Clamp01(elapsed / half);
            await UniTask.Yield();
        }
        _whiteGroup.alpha = 1f;
        elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.unscaledDeltaTime;
            _whiteGroup.alpha = 1f - Mathf.Clamp01(elapsed / half);
            await UniTask.Yield();
        }
        _whiteGroup.alpha = 0f;
    }
}
