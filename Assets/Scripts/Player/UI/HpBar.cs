using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputManagerEntry;

public class HpBar : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private Slider hpBar;
    private void Awake()
    {
        hpBar = GetComponent<Slider>();
    }
    private void Start()
    {
        UpdateHpBar(playerHealth.CurrentHp, playerHealth.MaxHp);
    }
    private void OnEnable()
    {
        if (PlayerManager.Instance != null && PlayerManager.Instance.HasPlayer)
            Bind(PlayerManager.Instance.Current);
        PlayerManager.OnPlayerSet += Bind;
        PlayerManager.OnPlayerCleared += Unbind;
        UpdateHpBar(playerHealth.CurrentHp, playerHealth.MaxHp);
        playerHealth.OnHealthChanged += UpdateHpBar;
    }
    private void OnDisable()
    {
        playerHealth.OnHealthChanged -= UpdateHpBar;
        PlayerManager.OnPlayerSet -= Bind;
        PlayerManager.OnPlayerCleared -= Unbind;
    }
    private void UpdateHpBar(float currentHp, float maxHp)
    {
        hpBar.value = maxHp > 0f ? currentHp / maxHp : 0f;
    }
    private void Bind(PlayerController p) 
    {
        playerHealth = p.GetComponent<PlayerHealth>();
    }
    private void Unbind() { /* 정리 */ }
}
