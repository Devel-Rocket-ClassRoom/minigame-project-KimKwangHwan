using TMPro;
using UnityEngine;

public class AmmoDisplay : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private TextMeshProUGUI remainText;

    private void OnEnable()
    {
        inventory.OnAmmoChanged += ChangeText;
        ChangeText(inventory.MaxAmmo, inventory.CurrentAmmo);
    }
    private void OnDisable()
    {
        inventory.OnAmmoChanged -= ChangeText;
    }
    private void Start()
    {
        ChangeText(inventory.MaxAmmo, inventory.CurrentAmmo);
    }
    private void ChangeText(int max, int current)
    {
        remainText.text = $"{current} / {max}";
    }
}
