using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SaveDataV = SaveDataV1;
/// <summary>
/// 3슬롯 선택 UI. 저장 모드와 로드 모드를 같은 패널로 처리한다.
///   - 저장 모드: 슬롯 선택 → SavePoint.ExecuteSave
///   - 로드 모드: 슬롯 선택 → GameInitializer.ApplyLoad
/// 패널이 열리면 Time.timeScale = 0 으로 게임을 정지한다.
/// </summary>
public class SaveSlotUI : MonoBehaviour
{
    public static SaveSlotUI Instance { get; private set; }

    private enum Mode { None, Save, Load }

    [Header("Panels")]
    [SerializeField] private GameObject slotPanel;     // 기본 SetActive false
    [SerializeField] private GameObject promptUI;      // 기본 SetActive false

    [Header("Slots")]
    [SerializeField] private Button[] slotButtons = new Button[SaveManager.SlotCount];
    [SerializeField] private TextMeshProUGUI[] slotTexts = new TextMeshProUGUI[SaveManager.SlotCount];

    [Header("Etc")]
    [SerializeField] private Button cancelButton;

    private Mode _mode = Mode.None;
    private PlayerController _currentPlayer;
    private SavePoint _currentSavePoint;

    public bool IsOpen => slotPanel != null && slotPanel.activeSelf;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 슬롯 버튼 리스너 연결 (루프 변수 캡처 주의 → 지역 복사)
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int index = i;
            if (slotButtons[i] != null)
                slotButtons[i].onClick.AddListener(() => OnSlotSelected(index));
        }

        if (cancelButton != null)
            cancelButton.onClick.AddListener(ClosePanel);

        if (slotPanel != null) slotPanel.SetActive(false);
        if (promptUI != null) promptUI.SetActive(false);
    }

    // ── 패널 열기 ───────────────────────────────────────────────
    public void OpenForSave(PlayerController player, SavePoint savePoint)
    {
        _mode = Mode.Save;
        _currentPlayer = player;
        _currentSavePoint = savePoint;
        OpenPanel();
    }

    public void OpenForLoad()
    {
        _mode = Mode.Load;
        _currentPlayer = null;
        _currentSavePoint = null;
        OpenPanel();
    }

    private void OpenPanel()
    {
        ShowPrompt(false);
        if (slotPanel != null) slotPanel.SetActive(true);
        RefreshSlotTexts();
        Time.timeScale = 0f;
    }

    // ── 프롬프트 ────────────────────────────────────────────────
    public void ShowPrompt(bool show)
    {
        if (promptUI != null) promptUI.SetActive(show);
    }

    // ── 슬롯 텍스트 갱신 ────────────────────────────────────────
    private void RefreshSlotTexts()
    {
        for (int slot = 0; slot < SaveManager.SlotCount; slot++)
        {
            if (slotTexts[slot] == null) continue;

            string header = $"슬롯 {slot + 1}";

            if (!SaveManager.Instance.HasSave(slot))
            {
                slotTexts[slot].text = $"{header}\n비어 있음";
                continue;
            }

            SaveDataV data = SaveManager.Instance.Load(slot);
            if (data == null)
            {
                // 파일은 있으나 복호화/역직렬화 실패 → 손상
                slotTexts[slot].text = $"{header}\n손상됨";
                continue;
            }

            string when = data.savedAt.ToString("yyyy-MM-dd HH:mm");
            slotTexts[slot].text = $"{header}\n{when}";
        }
    }

    // ── 슬롯 선택 ───────────────────────────────────────────────
    private void OnSlotSelected(int slot)
    {
        switch (_mode)
        {
            case Mode.Save:
                if (_currentSavePoint != null && _currentPlayer != null)
                    _currentSavePoint.ExecuteSave(slot, _currentPlayer);
                break;

            case Mode.Load:
                //if (GameInitializer.Instance != null)
                //    GameInitializer.Instance.ApplyLoad(slot);
                break;
        }
        ClosePanel();
    }

    // ── 패널 닫기 ───────────────────────────────────────────────
    public void ClosePanel()
    {
        if (slotPanel != null) slotPanel.SetActive(false);
        _mode = Mode.None;
        _currentPlayer = null;
        _currentSavePoint = null;
        Time.timeScale = 1f;
    }
}