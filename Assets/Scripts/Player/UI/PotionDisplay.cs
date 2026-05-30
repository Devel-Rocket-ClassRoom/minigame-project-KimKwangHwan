using UnityEngine;
using TMPro;
public class PotionDisplay : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private TextMeshProUGUI remainText;

    private void OnEnable()
    {
        inventory.OnHealItemChanged += ChangeText;
        ChangeText(inventory.MaxHealItems, inventory.CurrentHealItems);
    }
    private void OnDisable()
    {
        inventory.OnHealItemChanged -= ChangeText;
    }
    private void Start()
    {
        ChangeText(inventory.MaxHealItems, inventory.CurrentHealItems);
    }
    private void ChangeText(int max, int current)
    {
        remainText.text = $"{current} / {max}";
    }
}
