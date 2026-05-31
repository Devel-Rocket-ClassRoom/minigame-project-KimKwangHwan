using TMPro;
using UnityEngine;
using static UnityEngine.InputManagerEntry;
public class PotionDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI remainText;
    private Inventory inventory;

    private void OnEnable()
    {
        if (PlayerManager.Instance != null && PlayerManager.Instance.HasPlayer)
            Bind(PlayerManager.Instance.Current);
        PlayerManager.OnPlayerSet += Bind;
        PlayerManager.OnPlayerCleared += Unbind;
        inventory.OnHealItemChanged += ChangeText;
        ChangeText(inventory.MaxHealItems, inventory.CurrentHealItems);
    }
    private void OnDisable()
    {
        inventory.OnHealItemChanged -= ChangeText;
        PlayerManager.OnPlayerSet -= Bind;
        PlayerManager.OnPlayerCleared -= Unbind;
    }
    private void Start()
    {
        ChangeText(inventory.MaxHealItems, inventory.CurrentHealItems);
    }
    private void ChangeText(int max, int current)
    {
        remainText.text = $"{current} / {max}";
    }
    private void Bind(PlayerController p)
    {
        inventory = p.GetComponent<Inventory>();
    }
    private void Unbind() { /* 정리 */ }
}
