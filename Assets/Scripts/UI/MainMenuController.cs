using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 메인 메뉴의 비즈니스 로직(세이브 확인, 씬 전환, 종료 등)을 처리하는 Controller 클래스입니다.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [SerializeField] private MainMenuView view;
    
    [Tooltip("게임 시작 시 전환될 씬의 이름입니다.")]
    [SerializeField] private string nextSceneName = "Main";

    private void OnEnable()
    {
        if (view != null)
        {
            // View의 이벤트 구독
            view.OnNewGameClicked += HandleNewGameClicked;
            view.OnContinueClicked += HandleContinueClicked;
            view.OnSettingsClicked += HandleSettingsClicked;
            view.OnQuitClicked += HandleQuitClicked;
        }
    }

    private void OnDisable()
    {
        if (view != null)
        {
            // View의 이벤트 구독 해제
            view.OnNewGameClicked -= HandleNewGameClicked;
            view.OnContinueClicked -= HandleContinueClicked;
            view.OnSettingsClicked -= HandleSettingsClicked;
            view.OnQuitClicked -= HandleQuitClicked;
        }
    }

    private void Start()
    {
        InitializeMenuState();
    }

    /// <summary>
    /// 메뉴 진입 시 초기 상태를 설정합니다. (세이브 존재 여부에 따른 버튼 활성화 등)
    /// </summary>
    private void InitializeMenuState()
    {
        bool hasSave = false;

        if (SaveManager.Instance != null)
        {
            hasSave = SaveManager.Instance.HasSave();
        }
        else
        {
            Debug.LogWarning("[MainMenuController] SaveManager.Instance가 존재하지 않습니다. 이어하기 버튼이 비활성화됩니다.");
        }

        if (view != null)
        {
            view.SetContinueButtonInteractable(hasSave);
        }
    }

    private void HandleNewGameClicked()
    {
        Debug.Log("[MainMenuController] 새 게임을 시작합니다.");
        PlayClickSound();
        
        if (SaveManager.Instance != null && SaveManager.Instance.HasSave())
        {
            SaveManager.Instance.DeleteSave();
        }

        TransitionToGameScene();
    }

    private void HandleContinueClicked()
    {
        Debug.Log("[MainMenuController] 이어하기를 선택했습니다.");
        PlayClickSound();

        if (SaveManager.Instance != null)
        {
            bool loadSuccess = SaveManager.Instance.LoadGame();
            if (!loadSuccess)
            {
                Debug.LogWarning("[MainMenuController] 이어하기에 실패했습니다.");
                return;
            }
        }

        TransitionToGameScene();
    }

    private void HandleSettingsClicked()
    {
        Debug.Log("[MainMenuController] 설정 열기 요청 발생.");
        PlayClickSound();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowPanel("SettingsPanel");
        }

        var settingsView = Object.FindFirstObjectByType<SettingsView>(FindObjectsInactive.Include);
        if (settingsView != null)
        {
            settingsView.gameObject.SetActive(true);
        }
    }

    private void HandleQuitClicked()
    {
        Debug.Log("[MainMenuController] 게임 종료 선택.");
        PlayClickSound();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void PlayClickSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySfx("ui_click");
        }
    }

    private void TransitionToGameScene()
    {
        string targetScene = string.IsNullOrEmpty(nextSceneName) || nextSceneName == "Main" ? GameConstants.SCENE_MAIN : nextSceneName;
        
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.RequestTransition(new SceneTransitionRequest(targetScene, 0.3f, false));
        }
        else
        {
            SceneManager.LoadScene(targetScene);
        }
    }
}
