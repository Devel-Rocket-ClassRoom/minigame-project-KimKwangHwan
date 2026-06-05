using TMPro;
using UnityEngine;

public class PotionDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI remainText;
    private Inventory inventory;
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
        if (inventory == null) return;
        ChangeText(inventory.MaxHealItems, inventory.CurrentHealItems);
    }
    private void ChangeText(int max, int current)
    {
        remainText.text = $"{current} / {max}";
    }
    private void Bind(PlayerController p)
    {
        if (inventory != null)
            inventory.OnHealItemChanged -= ChangeText;
        inventory = p.GetComponent<Inventory>();
        inventory.OnHealItemChanged += ChangeText;
        ChangeText(inventory.MaxHealItems, inventory.CurrentHealItems);
    }
    private void Unbind()
    {
        if (inventory != null)
            inventory.OnHealItemChanged -= ChangeText;
        inventory = null;
    }
}
