using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 타이틀 화면의 버튼 기능 및 씬 전환을 관리하는 매니저 스크립트
/// </summary>
public class TitleScreenManager : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _settingsButton;

    private void OnEnable()
    {
        // 버튼 이벤트 리스너 등록
        if (_startButton != null)
        {
            _startButton.onClick.RemoveListener(OnStartClicked);
            _startButton.onClick.AddListener(OnStartClicked);
        }

        if (_settingsButton != null)
        {
            _settingsButton.onClick.RemoveListener(OnSettingsClicked);
            _settingsButton.onClick.AddListener(OnSettingsClicked);
        }
    }

    private void OnDisable()
    {
        // 메모리 누수 방지를 위한 이벤트 해제
        if (_startButton != null)
        {
            _startButton.onClick.RemoveListener(OnStartClicked);
        }

        if (_settingsButton != null)
        {
            _settingsButton.onClick.RemoveListener(OnSettingsClicked);
        }
    }

    /// <summary>
    /// 시작 버튼 클릭 시 Main 씬으로 전환
    /// </summary>
    public void OnStartClicked()
    {
        Debug.Log("시작 버튼 클릭: Main 씬으로 이동합니다.");
        SceneManager.LoadScene("Main");
    }

    /// <summary>
    /// 설정 버튼 클릭 시 로그 출력 (추후 팝업 연동 가능)
    /// </summary>
    public void OnSettingsClicked()
    {
        Debug.Log("설정 버튼 클릭: 설정 창을 엽니다.");
    }
}

