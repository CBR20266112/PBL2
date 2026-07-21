using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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
/// 사운드 설정을 저장하고 적용하는 매니저
/// </summary>
public class SettingsManager : Singleton<SettingsManager>
{
    private const string MasterVolumeKey = "Settings_MasterVolume";
    private const string MusicVolumeKey = "Settings_MusicVolume";
    private const string SfxVolumeKey = "Settings_SfxVolume";
    private const string AmbienceVolumeKey = "Settings_AmbienceVolume";
    private const string SoundThemeKey = "Settings_SoundTheme";

    private static readonly SoundTheme[] ExplicitThemes =
    {
        SoundTheme.Korea,
        SoundTheme.China,
        SoundTheme.Japan,
        SoundTheme.Vietnam,
        SoundTheme.Kyrgyzstan
    };

    public float MasterVolume { get; private set; } = 1.0f;
    public float MusicVolume { get; private set; } = 0.5f;
    public float SfxVolume { get; private set; } = 0.8f;
    public float AmbienceVolume { get; private set; } = 0.7f;
    public SoundTheme SelectedTheme { get; private set; } = SoundTheme.Korea;

    protected override void Awake()
    {
        base.Awake();
        LoadSettings();
    }

    public void LoadSettings()
    {
        MasterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 1.0f);
        MusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 0.5f);
        SfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 0.8f);
        AmbienceVolume = PlayerPrefs.GetFloat(AmbienceVolumeKey, 0.7f);

        int savedTheme = PlayerPrefs.GetInt(SoundThemeKey, (int)SoundTheme.Korea);
        if (Enum.IsDefined(typeof(SoundTheme), savedTheme))
        {
            SelectedTheme = (SoundTheme)savedTheme;
        }
        else
        {
            SelectedTheme = SoundTheme.Korea;
        }

        ApplyAudioVolumes();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(MasterVolumeKey, MasterVolume);
        PlayerPrefs.SetFloat(MusicVolumeKey, MusicVolume);
        PlayerPrefs.SetFloat(SfxVolumeKey, SfxVolume);
        PlayerPrefs.SetFloat(AmbienceVolumeKey, AmbienceVolume);
        PlayerPrefs.SetInt(SoundThemeKey, (int)SelectedTheme);
        PlayerPrefs.Save();
    }

    public void SetMasterVolume(float volume)
    {
        MasterVolume = Mathf.Clamp01(volume);
        SaveSettings();
        AudioManager.Instance.SetMasterVolume(MasterVolume);
    }

    public void SetMusicVolume(float volume)
    {
        MusicVolume = Mathf.Clamp01(volume);
        SaveSettings();
        AudioManager.Instance.SetMusicVolume(MusicVolume);
    }

    public void SetSfxVolume(float volume)
    {
        SfxVolume = Mathf.Clamp01(volume);
        SaveSettings();
        AudioManager.Instance.SetSfxVolume(SfxVolume);
    }

    public void SetAmbienceVolume(float volume)
    {
        AmbienceVolume = Mathf.Clamp01(volume);
        SaveSettings();
        AudioManager.Instance.SetAmbienceVolume(AmbienceVolume);
    }

    public void SetSoundTheme(SoundTheme soundTheme)
    {
        SelectedTheme = soundTheme;
        SaveSettings();
        ApplyCurrentSceneMusic();
    }

    public string GetCurrentThemeDisplayName()
    {
        if (SelectedTheme == SoundTheme.Random)
            return "랜덤";

        string displayName = AudioManager.Instance.GetThemeDisplayName(SelectedTheme.ToString());
        return !string.IsNullOrEmpty(displayName) ? displayName : SelectedTheme.ToString();
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

        AudioClip clip = AudioManager.Instance.GetThemeBgmClip(theme.ToString());
        if (clip != null)
        {
            return clip.name; // 기존 API 호환을 위해 clip 이름 반환
        }

        // DB에 없을 경우의 최소한의 기본값 (하드코딩 최소화)
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
        AudioManager.Instance.SetMasterVolume(MasterVolume);
        AudioManager.Instance.SetMusicVolume(MusicVolume);
        AudioManager.Instance.SetSfxVolume(SfxVolume);
        AudioManager.Instance.SetAmbienceVolume(AmbienceVolume);
    }

    public void ApplyCurrentSceneMusic()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == GameConstants.SCENE_TITLE || sceneName == GameConstants.SCENE_MAIN)
        {
            string themeStr = GetPlaybackTheme().ToString();
            AudioManager.Instance.PlayThemeBGM(themeStr);
            AudioManager.Instance.PlayThemeAmbience(themeStr);
        }
    }
}
