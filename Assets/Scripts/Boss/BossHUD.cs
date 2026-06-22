using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
public class BossHUD : MonoBehaviour
{
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider delayedHpBar;
    [SerializeField] private TMP_Text bossNameText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] float delaySpeed = 2f;

    private EnemyHealth health;

    private void Update()
    {
        if (health == null) return;
        float target = health.CurrentHp / health.MaxHp;
        delayedHpBar.value = Mathf.MoveTowards(delayedHpBar.value, target, delaySpeed * Time.deltaTime);
    }

    public void Bind(EnemyHealth health)
    {
        if (health != null) health.OnDamaged -= OnDamaged;
        this.health = health;
        health.OnDamaged += OnDamaged;
        hpBar.value = 1f;
        delayedHpBar.value = 1f;
    }

    public void Unbind()
    {
        if (health != null) health.OnDamaged -= OnDamaged;
        health = null;
    }

    private void OnDamaged(float _)
    {
        if (health == null) return;
        hpBar.value = health.CurrentHp / health.MaxHp;
    }

    public async UniTask Show(float duration = 0.4f)
    {
        canvasGroup.alpha = 0f;
        gameObject.SetActive(true);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
            await UniTask.Yield();
        }
        canvasGroup.alpha = 1f;
    }

    public async UniTask Hide(float duration = 0.4f)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / duration);
            await UniTask.Yield();
        }
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
        Unbind();
    }
}
