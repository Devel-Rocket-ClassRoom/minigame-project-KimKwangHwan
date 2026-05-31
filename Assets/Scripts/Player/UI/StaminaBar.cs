using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputManagerEntry;

public class StaminaBar : MonoBehaviour
{
    private PlayerStamina playerStamina;
    private Slider staminaBar;
    private void Awake()
    {
        staminaBar = GetComponent<Slider>();
    }
    private void Start()
    {
        UpdateStaminaBar(playerStamina.CurrentStamina, playerStamina.MaxStamina);
    }
    private void OnEnable()
    {
        if (PlayerManager.Instance != null && PlayerManager.Instance.HasPlayer)
            Bind(PlayerManager.Instance.Current);
        PlayerManager.OnPlayerSet += Bind;
        PlayerManager.OnPlayerCleared += Unbind;
        UpdateStaminaBar(playerStamina.CurrentStamina, playerStamina.MaxStamina);
        playerStamina.OnStaminaChanged += UpdateStaminaBar;
    }
    private void OnDisable()
    {
        playerStamina.OnStaminaChanged -= UpdateStaminaBar;
        PlayerManager.OnPlayerSet -= Bind;
        PlayerManager.OnPlayerCleared -= Unbind;
    }
    private void UpdateStaminaBar(float currentStamina, float maxStamina)
    {
        staminaBar.value = maxStamina > 0f ? currentStamina / maxStamina : 0f;
    }
    private void Bind(PlayerController p)
    {
        playerStamina = p.GetComponent<PlayerStamina>();
    }
    private void Unbind() { /* 정리 */ }
}
