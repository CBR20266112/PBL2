using UnityEngine;
using TMPro;

/// <summary>
/// UI의 TMP_Text에 다국어 문자열을 자동으로 갱신해 주는 컴포넌트입니다.
/// </summary>
public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string localizationKey;
    [SerializeField] private TMP_Text targetText;

    private void Awake()
    {
        // 타겟 텍스트가 할당되지 않은 경우 자동 캐싱 시도
        if (targetText == null)
        {
            targetText = GetComponent<TMP_Text>();
        }
    }

    private void OnEnable()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.OnLanguageChanged += OnLanguageChanged;
        }

        // 활성화 시점에 즉시 갱신
        Refresh();
    }

    private void OnDisable()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.OnLanguageChanged -= OnLanguageChanged;
        }
    }

    private void OnLanguageChanged(LanguageType newLanguage)
    {
        Refresh();
    }

    /// <summary>
    /// 런타임에 다국어 키를 변경하고 즉시 텍스트를 갱신합니다.
    /// </summary>
    public void SetLocalizationKey(string key)
    {
        localizationKey = key;
        Refresh();
    }

    /// <summary>
    /// 현재 설정된 다국어 키를 반환합니다.
    /// </summary>
    public string GetLocalizationKey()
    {
        return localizationKey;
    }

    /// <summary>
    /// 설정된 다국어 키를 기반으로 텍스트를 최신 언어로 갱신합니다.
    /// </summary>
    public void Refresh()
    {
        if (targetText == null)
        {
            Debug.LogWarning($"[LocalizedText] 대상 TMP_Text가 없습니다. GameObject: {gameObject.name}");
            return;
        }

        if (string.IsNullOrEmpty(localizationKey))
        {
            // 키가 없으면 무시합니다 (빈 문자열이 의도일 수 있으나 보통은 세팅 오류 방지)
            return;
        }

        if (LocalizationManager.Instance != null)
        {
            targetText.text = LocalizationManager.Instance.GetText(localizationKey);
        }
        else
        {
            // 매니저 인스턴스가 없을 경우 fallback으로 키값을 그대로 출력할 수도 있으나
            // 경고를 띄워두는 것이 좋습니다.
            Debug.LogWarning("[LocalizedText] LocalizationManager.Instance가 존재하지 않습니다.");
        }
    }
}
