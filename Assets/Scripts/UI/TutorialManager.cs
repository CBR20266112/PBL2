using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 튜토리얼 관리
/// 게임 시작 후 첫 플레이를 안내하는 인게임 튜토리얼
/// </summary>
public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject _tutorialPanel;
    [SerializeField] private Text _tutorialText;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _skipButton;
    [SerializeField] private Image _highlightImage;

    private int _currentStep = 0;
    private string[] _tutorialSteps = new string[]
    {
        "안녕하세요! xx다방에 오신 것을 환영합니다.",
        "여기서 당신은 할머니로부터 물려받은 다방을 운영합니다.",
        "손님들이 차를 주문하면, 정확하게 만들어주세요.",
        "손님 대기 버튼을 눌러 첫 번째 손님을 불러보세요!",
        "준비되셨나요? 시작하겠습니다!"
    };

    private void Start()
    {
        RegisterListeners();

        // 새 게임일 경우만 튜토리얼 표시
        if (!PlayerPrefs.HasKey("TutorialCompleted"))
        {
            ShowTutorial();
        }
        else
        {
            _tutorialPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        RegisterListeners();
    }

    private void OnDisable()
    {
        UnregisterListeners();
    }

    private void RegisterListeners()
    {
        if (_nextButton != null)
        {
            _nextButton.onClick.RemoveListener(OnNextClicked);
            _nextButton.onClick.AddListener(OnNextClicked);
        }
        if (_skipButton != null)
        {
            _skipButton.onClick.RemoveListener(OnSkipClicked);
            _skipButton.onClick.AddListener(OnSkipClicked);
        }
    }

    private void UnregisterListeners()
    {
        if (_nextButton != null)
            _nextButton.onClick.RemoveListener(OnNextClicked);
        if (_skipButton != null)
            _skipButton.onClick.RemoveListener(OnSkipClicked);
    }

    private void ShowTutorial()
    {
        _tutorialPanel.SetActive(true);
        _currentStep = 0;
        UpdateTutorialStep();
    }

    private void OnNextClicked()
    {
        _currentStep++;
        if (_currentStep < _tutorialSteps.Length)
        {
            UpdateTutorialStep();
        }
        else
        {
            CompleteTutorial();
        }
    }

    private void OnSkipClicked()
    {
        CompleteTutorial();
    }

    private void UpdateTutorialStep()
    {
        if (_tutorialText != null)
        {
            _tutorialText.text = _tutorialSteps[_currentStep];
        }

        Debug.Log($"Tutorial Step {_currentStep}: {_tutorialSteps[_currentStep]}");
    }

    private void CompleteTutorial()
    {
        _tutorialPanel.SetActive(false);
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();
        GameManager.Instance.CurrentState = GameManager.GameState.Playing;
        Debug.Log("Tutorial completed");
    }
}
