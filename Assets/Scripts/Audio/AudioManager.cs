using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임 전체에서 배경음악과 사운드를 관리하는 싱글톤
/// </summary>
public class AudioManager : Singleton<AudioManager>
{
    private const string MusicResourcePath = "Audio/Music/";
    private const string SfxResourcePath = "Audio/SFX/";

    private AudioSource _musicSource;
    private AudioSource _sfxSource;

    protected override void Awake()
    {
        base.Awake();

        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.playOnAwake = false;
        _musicSource.loop = true;
        _musicSource.volume = 0.5f;

        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.playOnAwake = false;
        _sfxSource.loop = false;
        _sfxSource.volume = 1f;

        SceneManager.sceneLoaded += OnSceneLoaded;
        SettingsManager.Instance.ApplyAudioVolumes();
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);

        Debug.Log("AudioManager initialized");
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode _)
    {
        string musicTrack = SettingsManager.Instance.GetMusicTrackForScene(scene.name);
        if (!string.IsNullOrEmpty(musicTrack))
        {
            PlayMusic(musicTrack, true, SettingsManager.Instance.MusicVolume);
        }
    }

    public void SetMusicVolume(float volume)
    {
        _musicSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSfxVolume(float volume)
    {
        _sfxSource.volume = Mathf.Clamp01(volume);
    }

    public void ReloadCurrentSceneMusic()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        string musicTrack = SettingsManager.Instance.GetMusicTrackForScene(currentScene);
        if (!string.IsNullOrEmpty(musicTrack))
        {
            PlayMusic(musicTrack, true, SettingsManager.Instance.MusicVolume);
        }
    }

    public void PlayMusic(string clipName, bool loop = true, float volume = 0.5f)
    {
        if (string.IsNullOrEmpty(clipName))
            return;

        AudioClip clip = LoadAudioClip(MusicResourcePath + clipName);
        if (clip == null)
            return;

        if (_musicSource.clip == clip && _musicSource.isPlaying)
            return;

        _musicSource.clip = clip;
        _musicSource.loop = loop;
        _musicSource.volume = volume;
        _musicSource.Play();

        Debug.Log($"AudioManager: Playing music '{clipName}'");
    }

    public void StopMusic()
    {
        if (_musicSource.isPlaying)
        {
            _musicSource.Stop();
            Debug.Log("AudioManager: Stopped music");
        }
    }

    public void PlaySfx(string clipName, float volume = 1f)
    {
        if (string.IsNullOrEmpty(clipName))
            return;

        string themedClipName = SettingsManager.Instance.GetThemeSpecificSfxClipName(clipName);
        AudioClip clip = LoadAudioClip(SfxResourcePath + themedClipName) ?? LoadAudioClip(SfxResourcePath + clipName);
        if (clip == null)
            return;

        _sfxSource.PlayOneShot(clip, volume);
        Debug.Log($"AudioManager: Played SFX '{clipName}' (theme: {themedClipName})");
    }

    private AudioClip LoadAudioClip(string path)
    {
        AudioClip clip = Resources.Load<AudioClip>(path);
        if (clip == null)
        {
            Debug.LogWarning($"AudioManager: AudioClip not found at Resources/{path}");
        }

        return clip;
    }
}
