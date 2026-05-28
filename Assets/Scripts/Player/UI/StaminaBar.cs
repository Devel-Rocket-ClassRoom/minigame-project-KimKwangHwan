using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private PlayerStamina playerStamina;
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
        UpdateStaminaBar(playerStamina.CurrentStamina, playerStamina.MaxStamina);
        playerStamina.OnStaminaChanged += UpdateStaminaBar;
    }
    private void OnDisable()
    {
        playerStamina.OnStaminaChanged -= UpdateStaminaBar;
    }
    private void UpdateStaminaBar(float currentStamina, float maxStamina)
    {
        staminaBar.value = maxStamina > 0f ? currentStamina / maxStamina : 0f;
    }
}
