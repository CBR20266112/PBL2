using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 타이틀 화면 관리
/// 게임 시작, 플레이어 이름 설정, 계속하기 등
/// </summary>
public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private GameObject _nameInputPanel;
    [SerializeField] private InputField _nameInputField;
    [SerializeField] private Button _nameConfirmButton;
    [SerializeField] private Button _nameSkipButton;

    private bool _hasSavedData = false;
    private bool _eventsRegistered = false;

    private void OnEnable()
    {
        // 저장된 데이터 확인
        _hasSavedData = PlayerPrefs.HasKey("PlayerData");

        RegisterButtonEvents();
        Debug.Log($"TitleScreenManager: HasSavedData = {_hasSavedData}");
    }

    public void SetUIReferences(
        Button startButton,
        Button continueButton,
        Button settingsButton,
        GameObject nameInputPanel,
        InputField nameInputField,
        Button nameConfirmButton,
        Button nameSkipButton)
    {
        _startButton = startButton;
        _continueButton = continueButton;
        _settingsButton = settingsButton;
        _nameInputPanel = nameInputPanel;
        _nameInputField = nameInputField;
        _nameConfirmButton = nameConfirmButton;
        _nameSkipButton = nameSkipButton;

        RegisterButtonEvents();
    }

    private void RegisterButtonEvents()
    {
        if (_eventsRegistered)
            return;

        if (_startButton != null)
            _startButton.onClick.AddListener(OnStartClicked);
        if (_continueButton != null)
        {
            _continueButton.onClick.AddListener(OnContinueClicked);
            _continueButton.gameObject.SetActive(_hasSavedData);
        }
        if (_settingsButton != null)
            _settingsButton.onClick.AddListener(OnSettingsClicked);
        if (_nameConfirmButton != null)
            _nameConfirmButton.onClick.AddListener(OnNameConfirmClicked);
        if (_nameSkipButton != null)
            _nameSkipButton.onClick.AddListener(OnNameSkipClicked);

        _eventsRegistered = true;
    }

    private void OnDisable()
    {
        if (_startButton != null)
            _startButton.onClick.RemoveListener(OnStartClicked);
        if (_continueButton != null)
            _continueButton.onClick.RemoveListener(OnContinueClicked);
        if (_settingsButton != null)
            _settingsButton.onClick.RemoveListener(OnSettingsClicked);
        if (_nameConfirmButton != null)
            _nameConfirmButton.onClick.RemoveListener(OnNameConfirmClicked);
        if (_nameSkipButton != null)
            _nameSkipButton.onClick.RemoveListener(OnNameSkipClicked);
    }

    private void OnStartClicked()
    {
        Debug.Log("새 게임 시작");
        ShowNameInputPanel();
    }

    private void OnContinueClicked()
    {
        Debug.Log("게임 계속");
        // 저장된 데이터로 게임 시작
        LoadGameAndStart();
    }

    private void OnSettingsClicked()
    {
        Debug.Log("설정");
        // TODO: 설정 화면 표시
    }

    private void ShowNameInputPanel()
    {
        if (_nameInputPanel != null)
        {
            _nameInputPanel.SetActive(true);
            if (_nameInputField != null)
            {
                _nameInputField.text = "orangeCat";
                _nameInputField.Select();
                _nameInputField.ActivateInputField();
            }
        }
    }

    private void OnNameConfirmClicked()
    {
        string playerName = _nameInputField.text.Trim();
        if (string.IsNullOrEmpty(playerName))
        {
            playerName = "orangeCat";
        }

        PlayerDataManager.Instance.SetPlayerName(playerName);
        HideNameInputPanel();
        StartGame();
    }

    private void OnNameSkipClicked()
    {
        PlayerDataManager.Instance.SetPlayerName("orangeCat");
        HideNameInputPanel();
        StartGame();
    }

    private void HideNameInputPanel()
    {
        if (_nameInputPanel != null)
            _nameInputPanel.SetActive(false);
    }

    private void StartGame()
    {
        Debug.Log($"게임 시작: {PlayerDataManager.Instance.PlayerName}");
        GameManager.Instance.LoadScene("Main");
    }

    private void LoadGameAndStart()
    {
        PlayerDataManager.Instance.LoadPlayerData();
        GameManager.Instance.LoadScene("Main");
    }
}
