using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Settings UI의 비즈니스 로직(Manager 연동)을 처리하는 Controller 클래스입니다.
/// </summary>
public class SettingsController : MonoBehaviour
{
    [SerializeField] private SettingsView view;

    private void Awake()
    {
        if (view != null)
        {
            // 드롭다운 언어 목록을 Enum 기반으로 동적 할당
            SetupLanguageOptions();
        }
    }

    private void OnEnable()
    {
        if (view != null)
        {
            view.OnLanguageChanged += HandleLanguageChanged;
            view.OnBgmVolumeChanged += HandleBgmVolumeChanged;
            view.OnSfxVolumeChanged += HandleSfxVolumeChanged;
            view.OnCloseClicked += HandleCloseClicked;

            SyncViewWithSettings();
        }
    }

    private void OnDisable()
    {
        if (view != null)
        {
            view.OnLanguageChanged -= HandleLanguageChanged;
            view.OnBgmVolumeChanged -= HandleBgmVolumeChanged;
            view.OnSfxVolumeChanged -= HandleSfxVolumeChanged;
            view.OnCloseClicked -= HandleCloseClicked;
        }
    }

    /// <summary>
    /// LanguageType enum을 기반으로 언어 옵션 리스트를 만들어 View에 전달합니다.
    /// </summary>
    private void SetupLanguageOptions()
    {
        string[] names = Enum.GetNames(typeof(LanguageType));
        List<string> options = new List<string>(names);
        
        view.SetLanguageOptions(options);
    }

    /// <summary>
    /// SettingsManager의 현재 값을 읽어와 View UI 컨트롤 값과 동기화합니다.
    /// </summary>
    private void SyncViewWithSettings()
    {
        if (SettingsManager.Instance == null || view == null) return;

        int currentLangIndex = (int)SettingsManager.Instance.GetLanguage();
        float currentBgmVol = SettingsManager.Instance.GetBgmVolume();
        float currentSfxVol = SettingsManager.Instance.GetSfxVolume();

        view.InitializeValues(currentLangIndex, currentBgmVol, currentSfxVol);
    }

    private void HandleLanguageChanged(int index)
    {
        if (Enum.IsDefined(typeof(LanguageType), index))
        {
            LanguageType newLanguage = (LanguageType)index;
            if (SettingsManager.Instance != null)
            {
                SettingsManager.Instance.SetLanguage(newLanguage);
                Debug.Log($"[SettingsController] 언어 설정 변경: {newLanguage}");
            }
        }
        else
        {
            Debug.LogWarning($"[SettingsController] 유효하지 않은 LanguageType 인덱스입니다: {index}");
        }
    }

    private void HandleBgmVolumeChanged(float volume)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetBgmVolume(volume);
        }
    }

    private void HandleSfxVolumeChanged(float volume)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetSfxVolume(volume);
        }
    }

    private void HandleCloseClicked()
    {
        Debug.Log("[SettingsController] 설정 창 닫기 버튼 클릭됨.");
        if (view != null)
        {
            view.Hide();
        }
    }
}
