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
    private const string MusicVolumeKey = "Settings_MusicVolume";
    private const string SfxVolumeKey = "Settings_SfxVolume";
    private const string SoundThemeKey = "Settings_SoundTheme";

    private static readonly SoundTheme[] ExplicitThemes =
    {
        SoundTheme.Korea,
        SoundTheme.China,
        SoundTheme.Japan,
        SoundTheme.Vietnam,
        SoundTheme.Kyrgyzstan
    };

    public float MusicVolume { get; private set; } = 0.5f;
    public float SfxVolume { get; private set; } = 0.8f;
    public SoundTheme SelectedTheme { get; private set; } = SoundTheme.Korea;

    protected override void Awake()
    {
        base.Awake();
        LoadSettings();
    }

    public void LoadSettings()
    {
        MusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 0.5f);
        SfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 0.8f);

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
        PlayerPrefs.SetFloat(MusicVolumeKey, MusicVolume);
        PlayerPrefs.SetFloat(SfxVolumeKey, SfxVolume);
        PlayerPrefs.SetInt(SoundThemeKey, (int)SelectedTheme);
        PlayerPrefs.Save();
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

    public void SetSoundTheme(SoundTheme soundTheme)
    {
        SelectedTheme = soundTheme;
        SaveSettings();
        ApplyCurrentSceneMusic();
    }

    public string GetCurrentThemeDisplayName()
    {
        switch (SelectedTheme)
        {
            case SoundTheme.Korea:
                return "한국";
            case SoundTheme.China:
                return "중국";
            case SoundTheme.Japan:
                return "일본";
            case SoundTheme.Vietnam:
                return "베트남";
            case SoundTheme.Kyrgyzstan:
                return "키르기스스탄";
            case SoundTheme.Random:
                return "랜덤";
            default:
                return "한국";
        }
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
        switch (theme)
        {
            case SoundTheme.Korea:
                return "01_Korea_TeaCafe";
            case SoundTheme.China:
                return "02_China_TeaCafe";
            case SoundTheme.Japan:
                return "03_Japan_TeaCafe";
            case SoundTheme.Vietnam:
                return "04_Vietnam_TeaCafe";
            case SoundTheme.Kyrgyzstan:
                return "05_Kyrgyzstan_TeaCafe";
            case SoundTheme.Random:
                return GetThemeMusicTrackName(GetRandomTheme());
            default:
                return "01_Korea_TeaCafe";
        }
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
        AudioManager.Instance.SetMusicVolume(MusicVolume);
        AudioManager.Instance.SetSfxVolume(SfxVolume);
    }

    public void ApplyCurrentSceneMusic()
    {
        string track = GetMusicTrackForScene(SceneManager.GetActiveScene().name);
        if (!string.IsNullOrEmpty(track))
        {
            AudioManager.Instance.PlayMusic(track, true, MusicVolume);
        }
    }
}
