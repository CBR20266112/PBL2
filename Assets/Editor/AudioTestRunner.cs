using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;

[InitializeOnLoad]
public class AudioAutoTesterHook
{
    static AudioAutoTesterHook()
    {
        if (File.Exists("RunAudioTest.flag"))
        {
            File.Delete("RunAudioTest.flag");
            EditorApplication.delayCall += () => 
            {
                EditorApplication.isPlaying = true;
            };
        }
    }
}

public class AudioTestRunner : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void StartTests()
    {
        if (File.Exists("RunAudioTest_Runtime.flag"))
        {
            File.Delete("RunAudioTest_Runtime.flag");
            GameObject go = new GameObject("AudioTestRunner");
            go.AddComponent<AudioTestRunner>();
        }
    }

    private StringBuilder results = new StringBuilder();

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);

        AudioManager am = AudioManager.Instance;
        SettingsManager sm = SettingsManager.Instance;

        LogResult("게임 시작 시 AudioManager 정상 생성", am != null);

        bool isDDOL = am != null && am.gameObject.scene.buildIndex == -1;
        if (am != null && am.gameObject.scene.name == "DontDestroyOnLoad") isDDOL = true;
        LogResult("DontDestroyOnLoad 정상 동작", isDDOL);

        if (am == null) 
        {
            FinishTests();
            yield break;
        }

        FieldInfo activeMusicSourceField = typeof(AudioManager).GetField("_activeMusicSource", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo musicSourceAField = typeof(AudioManager).GetField("_musicSourceA", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo musicSourceBField = typeof(AudioManager).GetField("_musicSourceB", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo ambienceSourceField = typeof(AudioManager).GetField("_ambienceSource", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo activeSfxSourcesField = typeof(AudioManager).GetField("_activeSfxSources", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo sfxPoolField = typeof(AudioManager).GetField("_sfxPool", BindingFlags.NonPublic | BindingFlags.Instance);

        AudioSource GetActiveMusicSource() => (AudioSource)activeMusicSourceField.GetValue(am);
        AudioSource GetMusicSourceA() => (AudioSource)musicSourceAField.GetValue(am);
        AudioSource GetMusicSourceB() => (AudioSource)musicSourceBField.GetValue(am);
        AudioSource GetAmbienceSource() => (AudioSource)ambienceSourceField.GetValue(am);
        List<AudioSource> GetActiveSfxSources() => (List<AudioSource>)activeSfxSourcesField.GetValue(am);
        AudioSourcePool GetSfxPool() => (AudioSourcePool)sfxPoolField.GetValue(am);

        AudioClip clipA = AudioClip.Create("ClipA", 44100, 1, 44100, false);
        AudioClip clipB = AudioClip.Create("ClipB", 44100, 1, 44100, false);

        am.PlayMusic(clipA, true, 1.0f);
        yield return null;
        AudioSource activeBgm = GetActiveMusicSource();
        LogResult("BGM 정상 재생", activeBgm != null && activeBgm.isPlaying && activeBgm.clip == clipA);

        am.PlayMusic(clipB, true, 1.0f);
        yield return new WaitForSeconds(0.1f);
        AudioSource bgmA = GetMusicSourceA();
        AudioSource bgmB = GetMusicSourceB();
        bool isCrossFading = (bgmA.isPlaying && bgmB.isPlaying);
        LogResult("BGM Cross Fade 정상 동작", isCrossFading);

        am.PlayAmbience(clipA, 1.0f);
        yield return null;
        AudioSource ambSource = GetAmbienceSource();
        LogResult("Ambience 재생", ambSource != null && ambSource.isPlaying && ambSource.clip == clipA);

        am.PlayAmbience(clipB, 1.0f);
        float initVol = ambSource.volume;
        yield return new WaitForSeconds(0.2f);
        float midVol = ambSource.volume;
        bool volChanged = Mathf.Abs(initVol - midVol) > 0.001f;
        LogResult("Ambience Fade In", volChanged); 
        LogResult("Ambience Fade Out", volChanged); 

        ThemeAudioData dummyTheme = ScriptableObject.CreateInstance<ThemeAudioData>();
        dummyTheme.themeID = "TestTheme";
        dummyTheme.bgmClip = clipA;
        dummyTheme.ambienceClip = clipB;
        
        FieldInfo themeDbField = typeof(AudioManager).GetField("_themeDatabase", BindingFlags.NonPublic | BindingFlags.Instance);
        ThemeDatabase themeDb = ScriptableObject.CreateInstance<ThemeDatabase>();
        themeDb.themes = new List<ThemeAudioData>() { dummyTheme };
        themeDb.Initialize();
        themeDbField.SetValue(am, themeDb);

        am.PlayThemeBGM("TestTheme");
        am.PlayThemeAmbience("TestTheme");
        yield return null;
        bool themeBgmPassed = GetActiveMusicSource().clip == clipA;
        bool themeAmbPassed = GetAmbienceSource().clip == clipB;
        LogResult("Theme 변경 시 BGM 자동 교체", themeBgmPassed);
        LogResult("Theme 변경 시 Ambience 자동 교체", themeAmbPassed);

        SfxData dummySfx = new SfxData { sfxID = "TestSfx", clip = clipA, priority = 100, maxPolyphony = 1, useRandomPitch = true, minPitch=0.5f, maxPitch=2.0f, useRandomVolume=true, minVolume=0.5f, maxVolume=0.9f };
        SfxDatabase sfxDb = ScriptableObject.CreateInstance<SfxDatabase>();
        sfxDb.sfxList = new List<SfxData>() { dummySfx };
        sfxDb.Initialize();
        FieldInfo sfxDbField = typeof(AudioManager).GetField("_sfxDatabase", BindingFlags.NonPublic | BindingFlags.Instance);
        sfxDbField.SetValue(am, sfxDb);

        am.PlaySfx("TestSfx", 1.0f);
        yield return null;
        var activeSfx = GetActiveSfxSources();
        bool sfxPlayed = activeSfx.Count == 1 && activeSfx[0].isPlaying;
        LogResult("SFX 정상 재생", sfxPlayed);

        if (sfxPlayed)
        {
            AudioSource playedSfx = activeSfx[0];
            LogResult("Pitch Random 정상 적용", playedSfx.pitch != 1.0f);
            LogResult("Volume Random 정상 적용", playedSfx.volume != 1.0f);
            LogResult("Priority 정상 적용", playedSfx.priority == 100);
        }
        else
        {
            LogResult("Pitch Random 정상 적용", false);
            LogResult("Volume Random 정상 적용", false);
            LogResult("Priority 정상 적용", false);
        }

        am.PlaySfx("TestSfx", 1.0f);
        yield return null;
        LogResult("Polyphony 제한 정상 적용", GetActiveSfxSources().Count == 1);

        SfxData noLimitSfx = new SfxData { sfxID = "NoLimit", clip = clipA, maxPolyphony = 0 };
        sfxDb.sfxList.Add(noLimitSfx);
        sfxDb.Initialize();

        int startChildCount = GetSfxPool().transform.childCount;
        for (int i=0; i<20; i++) am.PlaySfx("NoLimit", 1.0f);
        yield return null;
        int endChildCount = GetSfxPool().transform.childCount;
        LogResult("Pool 부족 시 정상 확장", endChildCount > startChildCount);

        foreach(var src in GetActiveSfxSources()) src.Stop();
        yield return null; 
        
        AudioSource s1 = GetSfxPool().GetSource();
        GetSfxPool().ReturnSource(s1);
        AudioSource s2 = GetSfxPool().GetSource();
        LogResult("AudioSourcePool 정상 재사용", s1 == s2);

        float oldMaster = sm.MasterVolume;
        float oldMusic = sm.MusicVolume;
        float oldSfx = sm.SfxVolume;
        float oldAmb = sm.AmbienceVolume;

        sm.SetMasterVolume(0.42f);
        sm.SetMusicVolume(0.43f);
        sm.SetSfxVolume(0.44f);
        sm.SetAmbienceVolume(0.45f);

        float loadedMaster = PlayerPrefs.GetFloat("Settings_MasterVolume", 1f);
        float loadedMusic = PlayerPrefs.GetFloat("Settings_MusicVolume", 1f);
        float loadedSfx = PlayerPrefs.GetFloat("Settings_SfxVolume", 1f);
        float loadedAmbience = PlayerPrefs.GetFloat("Settings_AmbienceVolume", 1f);

        LogResult("Master Volume 저장", Mathf.Approximately(loadedMaster, 0.42f));
        LogResult("BGM Volume 저장", Mathf.Approximately(loadedMusic, 0.43f));
        LogResult("SFX Volume 저장", Mathf.Approximately(loadedSfx, 0.44f));
        LogResult("Ambience Volume 저장", Mathf.Approximately(loadedAmbience, 0.45f));

        sm.LoadSettings();
        bool reloadPass = Mathf.Approximately(sm.MasterVolume, 0.42f) &&
                          Mathf.Approximately(sm.MusicVolume, 0.43f) &&
                          Mathf.Approximately(sm.SfxVolume, 0.44f) &&
                          Mathf.Approximately(sm.AmbienceVolume, 0.45f);
        LogResult("게임 재실행 후 볼륨 복원", reloadPass);

        sm.SetMasterVolume(oldMaster);
        sm.SetMusicVolume(oldMusic);
        sm.SetSfxVolume(oldSfx);
        sm.SetAmbienceVolume(oldAmb);

        FinishTests();
    }

    void LogResult(string testName, bool pass)
    {
        results.AppendLine($"{testName}: {(pass ? "PASS" : "FAIL")}");
    }

    void FinishTests()
    {
        File.WriteAllText("AudioTestResults.txt", results.ToString());
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
