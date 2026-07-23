using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 튜토리얼 강조 대상 UI(RectTransform)의 ID 매핑을 등록하고 조회하는 레지스트리 매니저입니다.
/// UI 요소들이 Awake/OnEnable 시점에 자신을 등록하면 튜토리얼 시스템이 ID 기반으로 접근할 수 있습니다.
/// </summary>
public class TutorialHighlightRegistry : Singleton<TutorialHighlightRegistry>
{
    private readonly Dictionary<string, RectTransform> _registry = new Dictionary<string, RectTransform>();

    /// <summary>
    /// 강조 대상 UI 항목을 등록합니다.
    /// </summary>
    public void RegisterTarget(string targetId, RectTransform rectTransform)
    {
        if (string.IsNullOrEmpty(targetId) || rectTransform == null) return;
        _registry[targetId] = rectTransform;
    }

    /// <summary>
    /// 등록된 강조 대상 UI 항목을 해제합니다.
    /// </summary>
    public void UnregisterTarget(string targetId)
    {
        if (string.IsNullOrEmpty(targetId)) return;
        _registry.Remove(targetId);
    }

    /// <summary>
    /// ID에 해당하는 RectTransform을 가져옵니다. 등록되어 있지 않으면 null 반환.
    /// </summary>
    public RectTransform GetTarget(string targetId)
    {
        if (string.IsNullOrEmpty(targetId)) return null;

        if (_registry.TryGetValue(targetId, out RectTransform rect))
        {
            return rect;
        }

        return null;
    }
}
