using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임 전체에서 배경음악, 효과음, 앰비언스를 관리하는 싱글톤
/// BGM CrossFade, Ambience Fade, AudioMixer 연동을 지원합니다.
/// </summary>
public class AudioManager : Singleton<AudioManager>
{
    private const string MusicResourcePath = "Audio/Music/";
    private const string SfxResourcePath = "Audio/SFX/";
    private const string AmbienceResourcePath = "Audio/Ambience/";

    // ──────────────────────────────
    // Inspector 설정
    // ──────────────────────────────

    [Header("Audio Mixer (선택 사항 — 할당하지 않아도 기본 볼륨 제어가 작동합니다)")]
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioMixerGroup _bgmMixerGroup;
    [SerializeField] private AudioMixerGroup _sfxMixerGroup;
    [SerializeField] private AudioMixerGroup _ambienceMixerGroup;

    [Header("SFX Data (선택 사항)")]
    [SerializeField, Tooltip("Polyphony, Pitch Random 등 상세 설정이 있는 SFX 데이터베이스")]
    private SfxDatabase _sfxDatabase;

    [Header("Theme Data")]
    [SerializeField, Tooltip("국가별 테마 데이터 리스트")]
    private ThemeDatabase _themeDatabase;

    [Header("BGM Cross Fade")]
    [SerializeField, Range(0.1f, 5f)] private float _crossFadeDuration = 2f;

    [Header("Ambience Fade")]
    [SerializeField, Range(0.1f, 5f)] private float _ambienceFadeDuration = 2f;

    // AudioMixer 파라미터 이름 (AudioMixer 내 Exposed Parameter 이름과 일치해야 함)
    private const string MixerParam_Master = "MasterVolume";
    private const string MixerParam_BGM = "BGMVolume";
    private const string MixerParam_SFX = "SFXVolume";
    private const string MixerParam_Ambience = "AmbienceVolume";

    // ──────────────────────────────
    // 내부 AudioSource 참조
    // ──────────────────────────────

    // BGM: 2개의 AudioSource로 CrossFade 지원
    private AudioSource _musicSourceA;
    private AudioSource _musicSourceB;
    private AudioSource _activeMusicSource;

    // SFX (Object Pool 기반)
    private AudioSourcePool _sfxPool;
    
    // SFX 동시 재생(Polyphony) 및 수거 관리를 위한 GC-Free 리스트
    private List<AudioSource> _activeSfxSources = new List<AudioSource>();
    private List<string> _activeSfxIDs = new List<string>();
    private Dictionary<string, int> _polyphonyCounts = new Dictionary<string, int>();

    // Ambience
    private AudioSource _ambienceSource;

    // Coroutine 참조 (중복 실행 방지)
    private Coroutine _crossFadeCoroutine;
    private Coroutine _ambienceFadeCoroutine;

    // 현재 볼륨 캐시 (AudioMixer 미사용 시 직접 제어에 필요)
    private float _currentMusicVolume = 0.5f;
    private float _currentAmbienceVolume = 0.7f;

    // ──────────────────────────────
    // 초기화
    // ──────────────────────────────

    protected override void Awake()
    {
        base.Awake();

        // 딕셔너리 사전 초기화 (Pre-warming)로 첫 재생 시의 지연(Hitch) 방지
        if (_themeDatabase != null) _themeDatabase.Initialize();
        if (_sfxDatabase != null) _sfxDatabase.Initialize();

        InitializeAudioSources();

        SceneManager.sceneLoaded += OnSceneLoaded;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);

        Debug.Log("AudioManager initialized");
    }

    private void InitializeAudioSources()
    {
        // BGM Source A
        _musicSourceA = gameObject.AddComponent<AudioSource>();
        ConfigureSource(_musicSourceA, loop: true, volume: 0.5f, _bgmMixerGroup);

        // BGM Source B (CrossFade용)
        _musicSourceB = gameObject.AddComponent<AudioSource>();
        ConfigureSource(_musicSourceB, loop: true, volume: 0f, _bgmMixerGroup);

        _activeMusicSource = _musicSourceA;

        // SFX Source Pool 초기화
        _sfxPool = gameObject.AddComponent<AudioSourcePool>();
        _sfxPool.Initialize(_sfxMixerGroup);

        // Ambience Source
        _ambienceSource = gameObject.AddComponent<AudioSource>();
        ConfigureSource(_ambienceSource, loop: true, volume: 0f, _ambienceMixerGroup);
    }

    private void ConfigureSource(AudioSource source, bool loop, float volume, AudioMixerGroup mixerGroup)
    {
        source.playOnAwake = false;
        source.loop = loop;
        source.volume = volume;
        if (mixerGroup != null)
            source.outputAudioMixerGroup = mixerGroup;
    }

    private void Start()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.OnBgmVolumeChanged += ApplyBgmVolume;
            SettingsManager.Instance.OnSfxVolumeChanged += ApplySfxVolume;
            
            // 초기 볼륨 동기화
            ApplyBgmVolume(SettingsManager.Instance.GetBgmVolume());
            ApplySfxVolume(SettingsManager.Instance.GetSfxVolume());
        }
    }

    protected override void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.OnBgmVolumeChanged -= ApplyBgmVolume;
            SettingsManager.Instance.OnSfxVolumeChanged -= ApplySfxVolume;
        }
        base.OnDestroy();
    }

    private void Update()
    {
        // 매 프레임 재생이 끝난 SFX 소스를 확인하여 풀로 반환 (GC 발생 없음)
        for (int i = _activeSfxSources.Count - 1; i >= 0; i--)
        {
            AudioSource source = _activeSfxSources[i];
            if (!source.isPlaying)
            {
                string sfxID = _activeSfxIDs[i];
                
                // 풀 반환
                _sfxPool.ReturnSource(source);

                // Polyphony 카운트 감소
                if (!string.IsNullOrEmpty(sfxID) && _polyphonyCounts.ContainsKey(sfxID))
                {
                    _polyphonyCounts[sfxID]--;
                }

                // 리스트에서 제거
                _activeSfxSources.RemoveAt(i);
                _activeSfxIDs.RemoveAt(i);
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode _)
    {
        if (SettingsManager.Instance == null) return;

        // SettingsManager에 GetMusicTrackForScene가 존재한다고 가정(기존 코드 호환)
        // 컴파일 에러 발생 시 SettingsManager쪽에 해당 메서드가 복구되어야 합니다.
        // string musicTrack = SettingsManager.Instance.GetMusicTrackForScene(scene.name);
        // if (!string.IsNullOrEmpty(musicTrack))
        // {
        //     PlayMusic(musicTrack, true, SettingsManager.Instance.GetBgmVolume());
        // }
    }

    // ──────────────────────────────
    // Volume Control
    // ──────────────────────────────

    /// <summary>
    /// 마스터 볼륨을 설정합니다. AudioMixer가 할당된 경우에만 동작합니다.
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        if (_audioMixer != null)
        {
            _audioMixer.SetFloat(MixerParam_Master, VolumeToDecibel(volume));
        }
    }

    /// <summary>
    /// BGM 볼륨을 AudioMixer 또는 Fallback에 적용합니다. (신규 API)
    /// </summary>
    public void ApplyBgmVolume(float volume)
    {
        _currentMusicVolume = Mathf.Clamp01(volume);

        if (_audioMixer != null)
        {
            _audioMixer.SetFloat(MixerParam_BGM, VolumeToDecibel(_currentMusicVolume));
        }
        else
        {
            if (_activeMusicSource != null && _crossFadeCoroutine == null)
                _activeMusicSource.volume = _currentMusicVolume;
        }
    }

    /// <summary>
    /// SFX 볼륨을 AudioMixer에 적용합니다. (신규 API)
    /// </summary>
    public void ApplySfxVolume(float volume)
    {
        float clampedVolume = Mathf.Clamp01(volume);

        if (_audioMixer != null)
        {
            _audioMixer.SetFloat(MixerParam_SFX, VolumeToDecibel(clampedVolume));
        }
    }

    /// <summary>
    /// 배경음악(BGM) 볼륨을 설정합니다. (기존 래퍼 API)
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        ApplyBgmVolume(volume);
    }

    /// <summary>
    /// 효과음(SFX) 볼륨을 설정합니다. (기존 래퍼 API)
    /// </summary>
    public void SetSfxVolume(float volume)
    {
        ApplySfxVolume(volume);
    }

    /// <summary>
    /// 앰비언스(Ambience) 볼륨을 설정합니다.
    /// </summary>
    public void SetAmbienceVolume(float volume)
    {
        _currentAmbienceVolume = Mathf.Clamp01(volume);

        if (_audioMixer != null)
        {
            _audioMixer.SetFloat(MixerParam_Ambience, VolumeToDecibel(_currentAmbienceVolume));
        }
        else
        {
            if (_ambienceSource != null && _ambienceFadeCoroutine == null)
                _ambienceSource.volume = _currentAmbienceVolume;
        }
    }

    /// <summary>
    /// 선형 볼륨 값(0~1)을 데시벨(-80~0 dB)로 변환합니다.
    /// </summary>
    private float VolumeToDecibel(float volume)
    {
        return volume > 0.0001f ? Mathf.Log10(volume) * 20f : -80f;
    }

    // ──────────────────────────────
    // BGM
    // ──────────────────────────────

    /// <summary>
    /// BGM을 재생합니다. (신규 API)
    /// </summary>
    public void PlayBgm(string id)
    {
        float volume = SettingsManager.Instance != null ? SettingsManager.Instance.GetBgmVolume() : 0.5f;
        PlayMusic(id, true, volume);
    }

    /// <summary>
    /// 배경음악을 재생합니다. (하위 호환용 래퍼 API)
    /// </summary>
    public void PlayMusic(string clipName, bool loop = true, float volume = 0.5f)
    {
        if (string.IsNullOrEmpty(clipName))
            return;

        AudioClip clip = LoadAudioClip(MusicResourcePath + clipName);
        if (clip != null)
        {
            PlayMusic(clip, loop, volume);
        }
    }

    /// <summary>
    /// 배경음악 클립을 직접 받아 재생합니다. (데이터 기반 연동)
    /// </summary>
    public void PlayMusic(AudioClip clip, bool loop = true, float volume = 0.5f)
    {
        if (clip == null)
            return;

        // 같은 클립이 이미 재생 중이면 무시
        if (_activeMusicSource.clip == clip && _activeMusicSource.isPlaying)
            return;

        _currentMusicVolume = volume;

        // 현재 음악이 재생 중이면 CrossFade, 아니면 바로 재생
        if (_activeMusicSource.isPlaying)
        {
            if (_crossFadeCoroutine != null)
                StopCoroutine(_crossFadeCoroutine);

            _crossFadeCoroutine = StartCoroutine(CrossFadeCoroutine(clip, loop, volume));
        }
        else
        {
            _activeMusicSource.clip = clip;
            _activeMusicSource.loop = loop;
            _activeMusicSource.volume = (_audioMixer != null) ? 1f : volume;
            _activeMusicSource.Play();
        }

        Debug.Log($"AudioManager: Playing music '{clip.name}'");
    }

    /// <summary>
    /// BGM을 정지합니다. (신규 API)
    /// </summary>
    public void StopBgm()
    {
        StopMusic();
    }

    /// <summary>
    /// 현재 재생 중인 배경음악을 즉시 정지합니다. (하위 호환용 래퍼 API)
    /// </summary>
    public void StopMusic()
    {
        if (_crossFadeCoroutine != null)
        {
            StopCoroutine(_crossFadeCoroutine);
            _crossFadeCoroutine = null;
        }

        _musicSourceA.Stop();
        _musicSourceB.Stop();
        Debug.Log("AudioManager: Stopped music");
    }

    /// <summary>
    /// 현재 재생 중인 배경음악을 페이드 아웃합니다.
    /// </summary>
    public void FadeOutMusic(float duration = -1f)
    {
        if (duration < 0f) duration = _crossFadeDuration;

        if (_crossFadeCoroutine != null)
            StopCoroutine(_crossFadeCoroutine);

        _crossFadeCoroutine = StartCoroutine(FadeOutCoroutine(_activeMusicSource, duration));
    }

    /// <summary>
    /// 현재 씬에 맞는 배경음악을 다시 로드하여 재생합니다.
    /// </summary>
    public void ReloadCurrentSceneMusic()
    {
        // string currentScene = SceneManager.GetActiveScene().name;
        // if (SettingsManager.Instance != null)
        // {
        //     string musicTrack = SettingsManager.Instance.GetMusicTrackForScene(currentScene);
        //     if (!string.IsNullOrEmpty(musicTrack))
        //     {
        //         PlayMusic(musicTrack, true, SettingsManager.Instance.GetBgmVolume());
        //     }
        // }
    }

    private IEnumerator CrossFadeCoroutine(AudioClip newClip, bool loop, float targetVolume)
    {
        AudioSource fadeOutSource = _activeMusicSource;
        AudioSource fadeInSource = (_activeMusicSource == _musicSourceA) ? _musicSourceB : _musicSourceA;

        // AudioMixer 사용 시 소스 볼륨은 1.0 기준으로 페이드
        float fadeTargetVolume = (_audioMixer != null) ? 1f : targetVolume;

        // 새 소스 설정 및 재생 시작
        fadeInSource.clip = newClip;
        fadeInSource.loop = loop;
        fadeInSource.volume = 0f;
        fadeInSource.Play();

        float timer = 0f;
        float startVolume = fadeOutSource.volume;

        while (timer < _crossFadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / _crossFadeDuration);

            fadeOutSource.volume = Mathf.Lerp(startVolume, 0f, t);
            fadeInSource.volume = Mathf.Lerp(0f, fadeTargetVolume, t);

            yield return null;
        }

        // 페이드 완료 후 정리
        fadeOutSource.Stop();
        fadeOutSource.clip = null;
        fadeOutSource.volume = 0f;

        fadeInSource.volume = fadeTargetVolume;
        _activeMusicSource = fadeInSource;

        _crossFadeCoroutine = null;
    }

    private IEnumerator FadeOutCoroutine(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / duration);
            source.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        source.Stop();
        source.volume = 0f;
        _crossFadeCoroutine = null;
    }

    // ──────────────────────────────
    // Ambience
    // ──────────────────────────────

    /// <summary>
    /// 앰비언스를 재생합니다. (하위 호환용 문자열 로드)
    /// </summary>
    public void PlayAmbience(string clipName, float volume = -1f)
    {
        if (string.IsNullOrEmpty(clipName))
            return;

        AudioClip clip = LoadAudioClip(AmbienceResourcePath + clipName);
        if (clip != null)
        {
            PlayAmbience(clip, volume);
        }
    }

    /// <summary>
    /// 앰비언스 클립을 직접 받아 재생합니다. (데이터 기반 연동)
    /// </summary>
    public void PlayAmbience(AudioClip clip, float volume = -1f)
    {
        if (clip == null)
            return;

        // 같은 클립이 이미 재생 중이면 무시
        if (_ambienceSource.clip == clip && _ambienceSource.isPlaying)
            return;

        if (volume >= 0f)
            _currentAmbienceVolume = Mathf.Clamp01(volume);

        if (_ambienceFadeCoroutine != null)
            StopCoroutine(_ambienceFadeCoroutine);

        _ambienceFadeCoroutine = StartCoroutine(AmbienceFadeCoroutine(clip));

        Debug.Log($"AudioManager: Playing ambience '{clip.name}'");
    }

    /// <summary>
    /// 현재 앰비언스를 페이드 아웃하여 정지합니다.
    /// </summary>
    public void StopAmbience()
    {
        if (_ambienceFadeCoroutine != null)
            StopCoroutine(_ambienceFadeCoroutine);

        _ambienceFadeCoroutine = StartCoroutine(FadeOutAmbienceCoroutine());
    }

    private IEnumerator AmbienceFadeCoroutine(AudioClip newClip)
    {
        float targetVolume = (_audioMixer != null) ? 1f : _currentAmbienceVolume;

        // 현재 재생 중이면 먼저 페이드 아웃
        if (_ambienceSource.isPlaying)
        {
            float startVolume = _ambienceSource.volume;
            float timer = 0f;

            while (timer < _ambienceFadeDuration)
            {
                timer += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(timer / _ambienceFadeDuration);
                _ambienceSource.volume = Mathf.Lerp(startVolume, 0f, t);
                yield return null;
            }

            _ambienceSource.Stop();
        }

        // 새 클립으로 페이드 인
        _ambienceSource.clip = newClip;
        _ambienceSource.volume = 0f;
        _ambienceSource.Play();

        float fadeTimer = 0f;
        while (fadeTimer < _ambienceFadeDuration)
        {
            fadeTimer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(fadeTimer / _ambienceFadeDuration);
            _ambienceSource.volume = Mathf.Lerp(0f, targetVolume, t);
            yield return null;
        }

        _ambienceSource.volume = targetVolume;
        _ambienceFadeCoroutine = null;
    }

    private IEnumerator FadeOutAmbienceCoroutine()
    {
        float startVolume = _ambienceSource.volume;
        float timer = 0f;

        while (timer < _ambienceFadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / _ambienceFadeDuration);
            _ambienceSource.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        _ambienceSource.Stop();
        _ambienceSource.clip = null;
        _ambienceSource.volume = 0f;
        _ambienceFadeCoroutine = null;
    }

    // ──────────────────────────────
    // SFX (기존 API 유지)
    // ──────────────────────────────

    /// <summary>
    /// 효과음을 재생합니다. 테마별 오버라이드가 있으면 우선 사용합니다.
    /// 기존 API 시그니처를 그대로 유지하되 내부적으로 Object Pool과 SfxDatabase를 사용합니다.
    /// </summary>
    public void PlaySfx(string clipName, float volume = 1f)
    {
        if (string.IsNullOrEmpty(clipName))
            return;

        string themedClipName = clipName;
        // string themedClipName = SettingsManager.Instance.GetThemeSpecificSfxClipName(clipName);
        SfxData data = null;
        
        if (_sfxDatabase != null)
        {
            // 테마별 클립명이 데이터베이스에 있으면 우선 적용, 없으면 기본 클립명
            data = _sfxDatabase.GetSfxData(themedClipName) ?? _sfxDatabase.GetSfxData(clipName);
        }

        AudioClip clipToPlay = null;
        if (data != null && data.clip != null)
        {
            clipToPlay = data.clip;
        }
        else
        {
            // 데이터베이스에 없거나 클립이 지정되지 않았으면 Resources에서 동적 로드 시도
            clipToPlay = LoadAudioClip(SfxResourcePath + themedClipName) ?? LoadAudioClip(SfxResourcePath + clipName);
        }

        if (clipToPlay == null)
            return;

        // Polyphony (동시 재생 수) 확인
        string trackID = (data != null && !string.IsNullOrEmpty(data.sfxID)) ? data.sfxID : clipName;
        if (data != null && data.maxPolyphony > 0)
        {
            _polyphonyCounts.TryGetValue(trackID, out int currentCount);
            if (currentCount >= data.maxPolyphony)
            {
                // 제한에 도달했으면 재생 무시
                return;
            }
        }

        // 풀에서 AudioSource 획득
        AudioSource source = _sfxPool.GetSource();
        source.clip = clipToPlay;
        
        // 속성 설정
        source.priority = (data != null) ? data.priority : 128;
        
        // Pitch Randomization
        if (data != null && data.useRandomPitch)
        {
            source.pitch = UnityEngine.Random.Range(data.minPitch, data.maxPitch);
        }
        else
        {
            source.pitch = 1f;
        }

        // Volume Randomization & AudioMixer Fallback
        float finalVolume = volume;
        if (data != null && data.useRandomVolume)
        {
            finalVolume *= UnityEngine.Random.Range(data.minVolume, data.maxVolume);
        }

        // AudioMixer가 할당되지 않은 경우 전역 SFX 볼륨을 직접 곱해줍니다.
        if (_audioMixer == null)
        {
            float sfxVol = SettingsManager.Instance != null ? SettingsManager.Instance.GetSfxVolume() : 1f;
            finalVolume *= sfxVol;
        }
        
        source.volume = Mathf.Clamp01(finalVolume);
        source.Play();

        // 관리 리스트에 추가 (GC Free)
        _activeSfxSources.Add(source);
        _activeSfxIDs.Add(trackID);

        // Polyphony 카운트 증가
        if (data != null && data.maxPolyphony > 0)
        {
            if (!_polyphonyCounts.ContainsKey(trackID))
                _polyphonyCounts[trackID] = 0;
                
            _polyphonyCounts[trackID]++;
        }

        Debug.Log($"AudioManager: Played SFX '{clipName}' (Priority: {source.priority}, TrackID: {trackID})");
    }

    // ──────────────────────────────
    // Audio Clip Loading
    // ──────────────────────────────

    private AudioClip LoadAudioClip(string path)
    {
        AudioClip clip = Resources.Load<AudioClip>(path);
        if (clip == null)
        {
            Debug.LogWarning($"AudioManager: AudioClip not found at Resources/{path}");
        }
        return clip;
    }

    // ──────────────────────────────
    // Theme Database Integration
    // ──────────────────────────────

    /// <summary>
    /// 특정 테마 ID에 해당하는 테마 데이터를 반환합니다.
    /// </summary>
    public ThemeAudioData GetThemeData(string themeID)
    {
        return _themeDatabase?.GetTheme(themeID);
    }

    /// <summary>
    /// 특정 테마의 배경음악(BGM) 클립을 가져옵니다.
    /// </summary>
    public AudioClip GetThemeBgmClip(string themeID)
    {
        return GetThemeData(themeID)?.bgmClip;
    }

    /// <summary>
    /// 테마 데이터베이스에서 특정 테마의 표시 이름(Display Name)을 가져옵니다.
    /// </summary>
    public string GetThemeDisplayName(string themeID)
    {
        return GetThemeData(themeID)?.displayName;
    }

    /// <summary>
    /// 특정 테마의 BGM을 재생합니다. (데이터베이스 직접 참조)
    /// </summary>
    public void PlayThemeBGM(string themeID, bool loop = true, float volume = -1f)
    {
        if (volume < 0f) volume = SettingsManager.Instance != null ? SettingsManager.Instance.GetBgmVolume() : 0.5f;

        ThemeAudioData data = GetThemeData(themeID);
        if (data != null && data.bgmClip != null)
        {
            PlayMusic(data.bgmClip, loop, volume);
        }
        else
        {
            // Fallback 로직은 보존하되 SettingsManager에 대한 직접 호출 방어
            // PlayMusic(SettingsManager.Instance.GetThemeMusicTrackName(SettingsManager.Instance.GetPlaybackTheme()), loop, volume);
        }
    }

    /// <summary>
    /// 특정 테마의 앰비언스를 재생합니다. (데이터베이스 직접 참조)
    /// </summary>
    public void PlayThemeAmbience(string themeID, float volume = -1f)
    {
        // 임시로 기본값 0.7f 지정 (SettingsManager에서 Ambience 제거됨)
        if (volume < 0f) volume = 0.7f; 

        ThemeAudioData data = GetThemeData(themeID);
        if (data != null && data.ambienceClip != null)
        {
            PlayAmbience(data.ambienceClip, volume);
        }
    }
}
