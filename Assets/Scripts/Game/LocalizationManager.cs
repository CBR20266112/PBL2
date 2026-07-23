using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 프로젝트 전체의 다국어 문자열을 관리하는 매니저 클래스입니다.
/// Key를 받아 현재 언어에 해당하는 문자열만 반환합니다.
/// UI 갱신, TextMeshPro 직접 접근, 폰트/이미지 교체는 담당하지 않습니다.
/// </summary>
public class LocalizationManager : Singleton<LocalizationManager>
{
    [Header("데이터베이스 연결")]
    [Tooltip("Inspector에서 LocalizationDatabase 에셋을 연결합니다. 카테고리별 여러 DB를 연결할 수 있습니다.")]
    [SerializeField] private List<LocalizationDatabase> _localizationDatabases = new List<LocalizationDatabase>();

    private LanguageType _currentLanguage = LanguageType.Korean;
    private readonly Dictionary<string, LocalizationEntry> _entryMap = new Dictionary<string, LocalizationEntry>();

    // 언어 변경 이벤트
    public delegate void LanguageChangedHandler(LanguageType newLanguage);
    public event LanguageChangedHandler OnLanguageChanged;

    protected override void Awake()
    {
        base.Awake();
        if (Instance == this)
        {
            BuildEntryMap();
        }
    }

    private void Start()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.OnLanguageChanged += HandleLanguageChanged;
        }
    }

    private void OnDestroy()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.OnLanguageChanged -= HandleLanguageChanged;
        }
    }

    private void HandleLanguageChanged(LanguageType newLanguage)
    {
        SetLanguage(newLanguage);
    }

    /// <summary>
    /// 모든 연결된 LocalizationDatabase에서 내부 딕셔너리 맵을 빌드합니다.
    /// </summary>
    private void BuildEntryMap()
    {
        _entryMap.Clear();
        if (_localizationDatabases == null) return;

        foreach (var db in _localizationDatabases)
        {
            if (db == null || db.entries == null) continue;

            foreach (var entry in db.entries)
            {
                if (entry != null && !string.IsNullOrEmpty(entry.key))
                {
                    _entryMap[entry.key] = entry;
                }
            }
        }

        Debug.Log($"[LocalizationManager] Localization 엔트리 {_entryMap.Count}개 로드 완료.");
    }

    /// <summary>
    /// 현재 활성 언어를 변경하고 OnLanguageChanged 이벤트를 발생시킵니다.
    /// </summary>
    public void SetLanguage(LanguageType language)
    {
        if (_currentLanguage == language) return;

        _currentLanguage = language;
        Debug.Log($"[LocalizationManager] 언어 변경: {language}");
        OnLanguageChanged?.Invoke(_currentLanguage);
    }

    /// <summary>
    /// 현재 활성 언어를 반환합니다.
    /// </summary>
    public LanguageType GetCurrentLanguage()
    {
        return _currentLanguage;
    }

    /// <summary>
    /// Key에 대응하는 현재 언어의 문자열을 반환합니다.
    /// Key가 없으면 Debug.LogWarning을 출력하고 Key 자체를 반환합니다.
    /// </summary>
    public string GetText(string key)
    {
        if (string.IsNullOrEmpty(key)) return string.Empty;

        if (!_entryMap.TryGetValue(key, out LocalizationEntry entry))
        {
            Debug.LogWarning($"[LocalizationManager] 미등록 Localization Key: \"{key}\"");
            return key;
        }

        switch (_currentLanguage)
        {
            case LanguageType.Korean:
                return !string.IsNullOrEmpty(entry.korean) ? entry.korean : key;
            case LanguageType.English:
                return !string.IsNullOrEmpty(entry.english) ? entry.english : key;
            default:
                return key;
        }
    }

    /// <summary>
    /// 지정한 Key가 등록되어 있는지 확인합니다.
    /// </summary>
    public bool HasKey(string key)
    {
        if (string.IsNullOrEmpty(key)) return false;
        return _entryMap.ContainsKey(key);
    }


}
