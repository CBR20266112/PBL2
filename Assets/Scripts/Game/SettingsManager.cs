using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임의 사운드 테마 (BGM/SFX 테마) 열거형
/// </summary>
public enum SoundTheme
{
    Korea = 0,
    China = 1,
    Japan = 2,
    Vietnam = 3,
    Kyrgyzstan = 4,
    Random = 5
}

/// <summary>
/// 게임 설정 데이터를 직렬화하기 위한 세이브 컨테이너
/// </summary>
[System.Serializable]
public class SettingsSaveData
{
    public int languageIndex = 0; // LanguageType enum 인덱스
    public float bgmVolume = 1f;  // 0.0 ~ 1.0
    public float sfxVolume = 1f;  // 0.0 ~ 1.0
    public float masterVolume = 1f;
    public float ambienceVolume = 0.7f;
    public int soundThemeIndex = 0;
}

/// <summary>
/// 게임의 통합 사용자 설정(언어, 오디오 볼륨, 사운드 테마 등)을 관리하는 유일한 매니저 클래스입니다.
/// </summary>
public class SettingsManager : Singleton<SettingsManager>
{
    private const string SoundThemeKey = "Settings_SoundTheme";

    private static readonly SoundTheme[] ExplicitThemes =
    {
        SoundTheme.Korea,
        SoundTheme.China,
        SoundTheme.Japan,
        SoundTheme.Vietnam,
        SoundTheme.Kyrgyzstan
    };

    private LanguageType _currentLanguage = LanguageType.Korean;
    private float _bgmVolume = 1f;
    private float _sfxVolume = 1f;
    private float _masterVolume = 1f;
    private float _ambienceVolume = 0.7f;

    public SoundTheme SelectedTheme { get; private set; } = SoundTheme.Korea;

    public float MasterVolume => _masterVolume;
    public float MusicVolume => _bgmVolume;
    public float SfxVolume => _sfxVolume;
    public float AmbienceVolume => _ambienceVolume;

    // 언어 변경 이벤트
    public delegate void LanguageChangedHandler(LanguageType newLanguage);
    public event LanguageChangedHandler OnLanguageChanged;

    // 볼륨 변경 이벤트
    public delegate void VolumeChangedHandler(float newVolume);
    public event VolumeChangedHandler OnBgmVolumeChanged;
    public event VolumeChangedHandler OnSfxVolumeChanged;

    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 언어 설정을 변경합니다. 변경된 경우에만 이벤트를 발생시킵니다.
    /// </summary>
    public void SetLanguage(LanguageType language)
    {
        if (_currentLanguage == language) return;

        _currentLanguage = language;
        Debug.Log($"[SettingsManager] 언어 변경: {language}");
        
        OnLanguageChanged?.Invoke(_currentLanguage);
    }

    /// <summary>
    /// 현재 언어 설정을 반환합니다.
    /// </summary>
    public LanguageType GetLanguage()
    {
        return _currentLanguage;
    }

    /// <summary>
    /// 설정을 읽어오는 레거시/에디터 테스트용 호환 메서드입니다.
    /// </summary>
    public void LoadSettings()
    {
        ApplyCurrentSettings();
    }

    /// <summary>
    /// 설정을 저장하는 레거시/에디터 테스트용 호환 메서드입니다.
    /// </summary>
    public void SaveSettings()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame();
        }
    }

    /// <summary>
    /// 마스터 볼륨을 설정합니다 (0.0 ~ 1.0).
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        _masterVolume = Mathf.Clamp01(volume);
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMasterVolume(_masterVolume);
    }

    /// <summary>
    /// BGM 볼륨을 변경합니다 (0.0 ~ 1.0). 레거시 호환용 API입니다.
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        SetBgmVolume(volume);
    }

    /// <summary>
    /// BGM 볼륨을 변경합니다 (0.0 ~ 1.0). 변경된 경우에만 이벤트를 발생시킵니다.
    /// </summary>
    public void SetBgmVolume(float volume)
    {
        float clampedVolume = Mathf.Clamp01(volume);
        if (Mathf.Approximately(_bgmVolume, clampedVolume)) return;

        _bgmVolume = clampedVolume;
        Debug.Log($"[SettingsManager] BGM 볼륨 변경: {_bgmVolume}");

        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(_bgmVolume);

        OnBgmVolumeChanged?.Invoke(_bgmVolume);
    }

    /// <summary>
    /// 현재 BGM 볼륨을 반환합니다.
    /// </summary>
    public float GetBgmVolume()
    {
        return _bgmVolume;
    }

    /// <summary>
    /// SFX 볼륨을 변경합니다 (0.0 ~ 1.0). 변경된 경우에만 이벤트를 발생시킵니다.
    /// </summary>
    public void SetSfxVolume(float volume)
    {
        float clampedVolume = Mathf.Clamp01(volume);
        if (Mathf.Approximately(_sfxVolume, clampedVolume)) return;

        _sfxVolume = clampedVolume;
        Debug.Log($"[SettingsManager] SFX 볼륨 변경: {_sfxVolume}");

        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSfxVolume(_sfxVolume);

        OnSfxVolumeChanged?.Invoke(_sfxVolume);
    }

    /// <summary>
    /// 현재 SFX 볼륨을 반환합니다.
    /// </summary>
    public float GetSfxVolume()
    {
        return _sfxVolume;
    }

    /// <summary>
    /// 엠비언스 볼륨을 설정합니다 (0.0 ~ 1.0).
    /// </summary>
    public void SetAmbienceVolume(float volume)
    {
        _ambienceVolume = Mathf.Clamp01(volume);
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetAmbienceVolume(_ambienceVolume);
    }

    /// <summary>
    /// 사운드 테마를 설정합니다.
    /// </summary>
    public void SetSoundTheme(SoundTheme soundTheme)
    {
        SelectedTheme = soundTheme;
        ApplyCurrentSceneMusic();
    }

    public string GetCurrentThemeDisplayName()
    {
        if (SelectedTheme == SoundTheme.Random)
            return "랜덤";

        if (AudioManager.Instance != null)
        {
            string displayName = AudioManager.Instance.GetThemeDisplayName(SelectedTheme.ToString());
            if (!string.IsNullOrEmpty(displayName)) return displayName;
        }

        return SelectedTheme.ToString();
    }

    public SoundTheme GetPlaybackTheme()
    {
        if (SelectedTheme == SoundTheme.Random)
        {
            return GetRandomTheme();
        }

        return SelectedTheme;
    }

    public string GetMusicTrackForScene(string sceneName)
    {
        if (sceneName == GameConstants.SCENE_TITLE || sceneName == GameConstants.SCENE_MAIN)
        {
            return GetThemeMusicTrackName(GetPlaybackTheme());
        }

        return string.Empty;
    }

    public string GetThemeMusicTrackName(SoundTheme theme)
    {
        if (theme == SoundTheme.Random)
        {
            theme = GetRandomTheme();
        }

        if (AudioManager.Instance != null)
        {
            AudioClip clip = AudioManager.Instance.GetThemeBgmClip(theme.ToString());
            if (clip != null)
            {
                return clip.name;
            }
        }

        return "01_Korea_TeaCafe";
    }

    public string GetThemeSpecificSfxClipName(string baseClipName)
    {
        SoundTheme theme = GetPlaybackTheme();
        return $"{baseClipName}_{theme}";
    }

    private SoundTheme GetRandomTheme()
    {
        return ExplicitThemes[UnityEngine.Random.Range(0, ExplicitThemes.Length)];
    }

    public void ApplyAudioVolumes()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterVolume(_masterVolume);
            AudioManager.Instance.SetMusicVolume(_bgmVolume);
            AudioManager.Instance.SetSfxVolume(_sfxVolume);
            AudioManager.Instance.SetAmbienceVolume(_ambienceVolume);
        }
    }

    public void ApplyCurrentSceneMusic()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == GameConstants.SCENE_TITLE || sceneName == GameConstants.SCENE_MAIN)
        {
            string themeStr = GetPlaybackTheme().ToString();
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayThemeBGM(themeStr);
                AudioManager.Instance.PlayThemeAmbience(themeStr);
            }
        }
    }

    /// <summary>
    /// 현재 설정된 값들을 바탕으로 이벤트를 일제히 발생시켜 연관된 매니저들에 적용시킵니다.
    /// </summary>
    public void ApplyCurrentSettings()
    {
        OnLanguageChanged?.Invoke(_currentLanguage);
        OnBgmVolumeChanged?.Invoke(_bgmVolume);
        OnSfxVolumeChanged?.Invoke(_sfxVolume);
        ApplyAudioVolumes();
        Debug.Log("[SettingsManager] 현재 설정 적용 완료 (이벤트 발송).");
    }

    // ──────────────────────────────────────────────────────────────────────
    // SaveManager 연동 (GetSaveData / LoadSaveData)
    // ──────────────────────────────────────────────────────────────────────

    public SettingsSaveData GetSaveData()
    {
        return new SettingsSaveData
        {
            languageIndex = (int)_currentLanguage,
            bgmVolume = _bgmVolume,
            sfxVolume = _sfxVolume,
            masterVolume = _masterVolume,
            ambienceVolume = _ambienceVolume,
            soundThemeIndex = (int)SelectedTheme
        };
    }

    public void LoadSaveData(SettingsSaveData saveData)
    {
        if (saveData == null) return;

        _bgmVolume = Mathf.Clamp01(saveData.bgmVolume);
        _sfxVolume = Mathf.Clamp01(saveData.sfxVolume);
        _masterVolume = Mathf.Clamp01(saveData.masterVolume);
        _ambienceVolume = Mathf.Clamp01(saveData.ambienceVolume);

        if (Enum.IsDefined(typeof(SoundTheme), saveData.soundThemeIndex))
        {
            SelectedTheme = (SoundTheme)saveData.soundThemeIndex;
        }

        if (Enum.IsDefined(typeof(LanguageType), saveData.languageIndex))
        {
            _currentLanguage = (LanguageType)saveData.languageIndex;
        }
        else
        {
            Debug.LogWarning($"[SettingsManager] 유효하지 않은 언어 인덱스({saveData.languageIndex}). Korean으로 복구합니다.");
            _currentLanguage = LanguageType.Korean;
        }

        Debug.Log("[SettingsManager] 설정 데이터 복원 완료.");
        ApplyCurrentSettings();
    }
}
