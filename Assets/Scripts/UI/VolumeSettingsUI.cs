using UnityEngine;
using UnityEngine.UI;

public class VolumeSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        if (SFXManager.Instance == null) return;
        bgmSlider.SetValueWithoutNotify(SFXManager.Instance.BgmVolume);
        sfxSlider.SetValueWithoutNotify(SFXManager.Instance.SfxVolume);

        bgmSlider.onValueChanged.AddListener(v => SFXManager.Instance.BgmVolume = v);
        sfxSlider.onValueChanged.AddListener(v => SFXManager.Instance.SfxVolume = v);
    }
}
