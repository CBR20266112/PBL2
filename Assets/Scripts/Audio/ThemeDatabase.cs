using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 여러 ThemeAudioData를 묶어서 중앙 관리하는 데이터베이스.
/// AudioManager나 SettingsManager에서 이 DB를 참조하여 하드코딩 없이 테마를 처리합니다.
/// </summary>
[CreateAssetMenu(fileName = "ThemeDatabase", menuName = "Audio/ThemeDatabase")]
public class ThemeDatabase : ScriptableObject
{
    public List<ThemeAudioData> themes = new List<ThemeAudioData>();

    private Dictionary<string, ThemeAudioData> _themeDict;

    /// <summary>
    /// 검색 최적화를 위해 딕셔너리를 빌드합니다.
    /// </summary>
    public void Initialize()
    {
        _themeDict = new Dictionary<string, ThemeAudioData>();
        foreach (var theme in themes)
        {
            if (theme != null && !string.IsNullOrEmpty(theme.themeID) && !_themeDict.ContainsKey(theme.themeID))
            {
                _themeDict.Add(theme.themeID, theme);
            }
        }
    }

    /// <summary>
    /// Theme ID (예: "Korea")를 기반으로 ThemeAudioData를 가져옵니다.
    /// </summary>
    public ThemeAudioData GetTheme(string themeID)
    {
        if (_themeDict == null)
        {
            Initialize();
        }

        if (_themeDict.TryGetValue(themeID, out ThemeAudioData data))
        {
            return data;
        }

        return null;
    }
}
