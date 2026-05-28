using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
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
        UpdateHpBar(playerHealth.CurrentHp, playerHealth.MaxHp);
        playerHealth.OnHealthChanged += UpdateHpBar;
    }
    private void OnDisable()
    {
        playerHealth.OnHealthChanged -= UpdateHpBar;
    }
    private void UpdateHpBar(float currentHp, float maxHp)
    {
        hpBar.value = maxHp > 0f ? currentHp / maxHp : 0f;
    }
}
