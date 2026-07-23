using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 메인 메뉴의 시각적 요소(버튼 등)를 참조하고, UI 이벤트를 Controller에 전달하는 View 클래스입니다.
/// </summary>
public class MainMenuView : MonoBehaviour
{
    [SerializeField] private Button btnNewGame;
    [SerializeField] private Button btnContinue;
    [SerializeField] private Button btnSettings;
    [SerializeField] private Button btnQuit;

    // View가 발생하는 이벤트들
    public event Action OnNewGameClicked;
    public event Action OnContinueClicked;
    public event Action OnSettingsClicked;
    public event Action OnQuitClicked;

    private void Awake()
    {
        // 버튼 클릭 이벤트를 C# Action으로 릴레이
        if (btnNewGame != null)
            btnNewGame.onClick.AddListener(() => OnNewGameClicked?.Invoke());
        
        if (btnContinue != null)
            btnContinue.onClick.AddListener(() => OnContinueClicked?.Invoke());
        
        if (btnSettings != null)
            btnSettings.onClick.AddListener(() => OnSettingsClicked?.Invoke());
        
        if (btnQuit != null)
            btnQuit.onClick.AddListener(() => OnQuitClicked?.Invoke());
    }

    /// <summary>
    /// 이어하기 버튼의 상호작용 가능 여부를 설정합니다.
    /// </summary>
    public void SetContinueButtonInteractable(bool isInteractable)
    {
        if (btnContinue != null)
        {
            btnContinue.interactable = isInteractable;
        }
    }
}
