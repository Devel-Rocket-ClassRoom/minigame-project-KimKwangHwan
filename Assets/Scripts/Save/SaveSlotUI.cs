using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SaveDataV = SaveDataV1;

public class SaveSlotUI : MonoBehaviour
{
    public enum Mode { NewGame, Continue }

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button[] slotButtons;
    [SerializeField] private TextMeshProUGUI[] slotTexts;
    [SerializeField] private Button backButton;

    private Mode _mode;
    private System.Action<int> _onSlotSelected;

    private void Awake()
    {
        if (backButton != null)
            backButton.onClick.AddListener(() => gameObject.SetActive(false));

        for (int i = 0; i < slotButtons.Length; i++)
        {
            int slot = i;
            if (slotButtons[i] != null)
                slotButtons[i].onClick.AddListener(() => OnSlotClicked(slot));
        }

        gameObject.SetActive(false);
    }

    public void Open(Mode mode, System.Action<int> onSlotSelected)
    {
        _mode = mode;
        _onSlotSelected = onSlotSelected;
        gameObject.SetActive(true);
        Refresh();
    }

    private void Refresh()
    {
        if (titleText != null)
            titleText.text = _mode == Mode.NewGame ? "New Game" : "Continue";

        for (int i = 0; i < slotButtons.Length; i++)
        {
            if (i >= SaveManager.SlotCount) break;

            bool hasSave = SaveManager.Instance.HasSave(i);

            if (slotTexts != null && i < slotTexts.Length && slotTexts[i] != null)
            {
                if (hasSave)
                {
                    SaveDataV data = SaveManager.Instance.Load(i);
                    string mapLabel = data?.mapId ?? "Unknown";
                    string dateLabel = data?.savedAt.ToString("yyyy-MM-dd HH:mm") ?? "";
                    slotTexts[i].text = $"Slot {i + 1}\n{mapLabel}\n{dateLabel}";
                }
                else
                {
                    slotTexts[i].text = $"Slot {i + 1}\n Empty";
                }
            }

            if (slotButtons[i] != null)
                slotButtons[i].interactable = _mode == Mode.NewGame || hasSave;
        }
    }

    private void OnSlotClicked(int slot)
    {
        if (_mode == Mode.NewGame)
            SaveManager.Instance.DeleteSave(slot);

        _onSlotSelected?.Invoke(slot);
        gameObject.SetActive(false);
    }
}
