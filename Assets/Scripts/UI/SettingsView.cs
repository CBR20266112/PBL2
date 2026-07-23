using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Settings UI의 시각적 컴포넌트를 제어하고 이벤트를 방출하는 View 클래스입니다.
/// </summary>
public class SettingsView : MonoBehaviour
{
    [SerializeField] private GameObject contentPanel;
    [SerializeField] private TMP_Dropdown languageDropdown;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Button btnClose;

    public event Action<int> OnLanguageChanged;
    public event Action<float> OnBgmVolumeChanged;
    public event Action<float> OnSfxVolumeChanged;
    public event Action OnCloseClicked;

    private void Awake()
    {
        if (languageDropdown != null)
        {
            languageDropdown.onValueChanged.AddListener(index => OnLanguageChanged?.Invoke(index));
        }

        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.onValueChanged.AddListener(volume => OnBgmVolumeChanged?.Invoke(volume));
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.onValueChanged.AddListener(volume => OnSfxVolumeChanged?.Invoke(volume));
        }

        if (btnClose != null)
        {
            btnClose.onClick.AddListener(() => OnCloseClicked?.Invoke());
        }
    }

    /// <summary>
    /// 언어 드롭다운의 옵션 목록을 동적으로 설정합니다.
    /// </summary>
    public void SetLanguageOptions(List<string> options)
    {
        if (languageDropdown != null && options != null)
        {
            languageDropdown.ClearOptions();
            languageDropdown.AddOptions(options);
        }
    }

    /// <summary>
    /// UI 컨트롤들의 값을 초기화합니다. 이벤트가 불필요하게 연쇄 호출되는 것을 방지합니다.
    /// </summary>
    public void InitializeValues(int languageIndex, float bgmVolume, float sfxVolume)
    {
        if (languageDropdown != null)
        {
            languageDropdown.SetValueWithoutNotify(languageIndex);
        }

        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.SetValueWithoutNotify(bgmVolume);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.SetValueWithoutNotify(sfxVolume);
        }
    }

    /// <summary>
    /// 설정 패널을 표시합니다.
    /// </summary>
    public void Show()
    {
        if (contentPanel != null)
        {
            contentPanel.SetActive(true);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 설정 패널을 숨깁니다.
    /// </summary>
    public void Hide()
    {
        if (contentPanel != null)
        {
            contentPanel.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
