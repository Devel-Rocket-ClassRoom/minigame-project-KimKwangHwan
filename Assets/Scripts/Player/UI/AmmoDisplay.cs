using TMPro;
using UnityEngine;

public class AmmoDisplay : MonoBehaviour
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
        ChangeText(inventory.MaxAmmo, inventory.CurrentAmmo);
    }
    private void ChangeText(int max, int current)
    {
        remainText.text = $"{current} / {max}";
    }
    private void Bind(PlayerController p)
    {
        if (inventory != null)
            inventory.OnAmmoChanged -= ChangeText;
        inventory = p.GetComponent<Inventory>();
        inventory.OnAmmoChanged += ChangeText;
        ChangeText(inventory.MaxAmmo, inventory.CurrentAmmo);
    }
    private void Unbind() 
    {
        if (inventory != null)
            inventory.OnAmmoChanged -= ChangeText;
        inventory = null;
    }
}
