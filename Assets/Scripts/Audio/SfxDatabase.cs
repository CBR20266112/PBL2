using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 각 효과음(SFX)의 속성을 정의하는 데이터 클래스.
/// Pitch/Volume Randomization, Priority, Polyphony(최대 동시 재생 수) 등을 설정할 수 있습니다.
/// </summary>
[Serializable]
public class SfxData
{
    [Tooltip("SFX 고유 ID (예: UI_Click, Tea_Pour)")]
    public string sfxID;

    [Tooltip("재생할 오디오 클립. 할당되지 않으면 Resources 폴더에서 동적 로드 시도.")]
    public AudioClip clip;

    [Range(0, 256)]
    [Tooltip("0 = 최고 우선순위, 256 = 최하 우선순위. (중요한 사운드는 낮게 설정)")]
    public int priority = 128;

    [Header("Pitch Randomization")]
    public bool useRandomPitch = false;
    [Range(0.1f, 3f)] public float minPitch = 0.9f;
    [Range(0.1f, 3f)] public float maxPitch = 1.1f;

    [Header("Volume Randomization")]
    public bool useRandomVolume = false;
    [Range(0f, 1f)] public float minVolume = 0.9f;
    [Range(0f, 1f)] public float maxVolume = 1.0f;

    [Header("Polyphony (동시 재생 제한)")]
    [Tooltip("동일한 사운드가 동시에 재생될 수 있는 최대 개수 (0 = 무제한)")]
    public int maxPolyphony = 0;
}

/// <summary>
/// 게임 내 효과음 데이터를 중앙에서 관리하는 ScriptableObject 데이터베이스.
/// 하드코딩이나 switch 문을 피하고 데이터 기반(Data-Driven) 설계를 위해 사용됩니다.
/// </summary>
[CreateAssetMenu(fileName = "SfxDatabase", menuName = "Audio/SfxDatabase")]
public class SfxDatabase : ScriptableObject
{
    public List<SfxData> sfxList = new List<SfxData>();

    private Dictionary<string, SfxData> _sfxDict;

    /// <summary>
    /// ID를 기반으로 빠르게 검색할 수 있도록 딕셔너리를 초기화합니다.
    /// </summary>
    public void Initialize()
    {
        _sfxDict = new Dictionary<string, SfxData>();
        foreach (var sfx in sfxList)
        {
            if (!string.IsNullOrEmpty(sfx.sfxID) && !_sfxDict.ContainsKey(sfx.sfxID))
            {
                _sfxDict.Add(sfx.sfxID, sfx);
            }
        }
    }

    /// <summary>
    /// 지정된 ID의 SfxData를 반환합니다. 없으면 null을 반환합니다.
    /// </summary>
    public SfxData GetSfxData(string id)
    {
        if (_sfxDict == null)
            Initialize();

        if (_sfxDict.TryGetValue(id, out SfxData data))
            return data;

        return null;
    }
}
