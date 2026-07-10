using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI 시스템 중앙 관리
/// UI 패널 표시/숨김, 버튼 매니저 역할
/// </summary>
public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Canvas _mainCanvas;

    private void OnEnable()
    {
        if (_mainCanvas == null)
        {
            Debug.LogWarning("UIManager: MainCanvas is not assigned!");
        }
    }

    /// <summary>
    /// 패널을 활성화
    /// </summary>
    public void ShowPanel(string panelName)
    {
        Transform panel = _mainCanvas.transform.Find(panelName);
        if (panel != null)
        {
            panel.gameObject.SetActive(true);
            Debug.Log($"Panel shown: {panelName}");
        }
        else
        {
            Debug.LogWarning($"Panel not found: {panelName}");
        }
    }

    /// <summary>
    /// 패널을 비활성화
    /// </summary>
    public void HidePanel(string panelName)
    {
        Transform panel = _mainCanvas.transform.Find(panelName);
        if (panel != null)
        {
            panel.gameObject.SetActive(false);
            Debug.Log($"Panel hidden: {panelName}");
        }
        else
        {
            Debug.LogWarning($"Panel not found: {panelName}");
        }
    }

    /// <summary>
    /// 모든 패널을 숨김
    /// </summary>
    public void HideAllPanels()
    {
        foreach (Transform child in _mainCanvas.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 패널 토글
    /// </summary>
    public void TogglePanel(string panelName)
    {
        Transform panel = _mainCanvas.transform.Find(panelName);
        if (panel != null)
        {
            panel.gameObject.SetActive(!panel.gameObject.activeSelf);
        }
    }
}
