using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private Slider hpBar;

    private void Awake()
    {
        hpBar = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        PlayerManager.OnPlayerSet += Bind;
        PlayerManager.OnPlayerCleared += Unbind;
        if (PlayerManager.Instance != null && PlayerManager.Instance.HasPlayer)
            Bind(PlayerManager.Instance.Current);
    }

    private void OnDisable()
    {
        PlayerManager.OnPlayerSet -= Bind;
        PlayerManager.OnPlayerCleared -= Unbind;
        Unbind();
    }

    private void Start()
    {
        if (playerHealth == null) return;
        UpdateHpBar(playerHealth.CurrentHp, playerHealth.MaxHp);
    }

    private void Bind(PlayerController p)
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateHpBar;
        playerHealth = p.GetComponent<PlayerHealth>();
        playerHealth.OnHealthChanged += UpdateHpBar;
        UpdateHpBar(playerHealth.CurrentHp, playerHealth.MaxHp);
    }

    private void Unbind()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateHpBar;
        playerHealth = null;
    }

    private void UpdateHpBar(float currentHp, float maxHp)
    {
        hpBar.value = maxHp > 0f ? currentHp / maxHp : 0f;
    }
}
