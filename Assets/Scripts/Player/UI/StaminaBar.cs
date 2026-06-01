using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    private PlayerStamina playerStamina;
    private Slider staminaBar;

    private void Awake()
    {
        staminaBar = GetComponent<Slider>();
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
        UpdateStaminaBar(playerStamina.CurrentStamina, playerStamina.MaxStamina);
    }
    private void Bind(PlayerController p)
    {
        if (playerStamina != null)
            playerStamina.OnStaminaChanged -= UpdateStaminaBar;
        playerStamina = p.GetComponent<PlayerStamina>();
        playerStamina.OnStaminaChanged += UpdateStaminaBar;
        UpdateStaminaBar(playerStamina.CurrentStamina, playerStamina.MaxStamina);
    }

    private void Unbind()
    {
        if (playerStamina != null)
            playerStamina.OnStaminaChanged -= UpdateStaminaBar;
        playerStamina = null;
    }

    private void UpdateStaminaBar(float currentStamina, float maxStamina)
    {
        staminaBar.value = maxStamina > 0f ? currentStamina / maxStamina : 0f;
    }
}
