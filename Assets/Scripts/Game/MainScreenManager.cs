using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 메인 화면 관리
/// 손님 대기, 상점, 컬렉션, 설정 등 주요 UI 관리
/// </summary>
public class MainScreenManager : MonoBehaviour
{
    [SerializeField] private Button _waitCustomerButton;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _collectionButton;
    [SerializeField] private Button _settingsButton;

    private PlayerHUD _playerHUD;

    public void Initialize(Button waitCustomerButton, Button shopButton, Button collectionButton, Button settingsButton)
    {
        _waitCustomerButton = waitCustomerButton;
        _shopButton = shopButton;
        _collectionButton = collectionButton;
        _settingsButton = settingsButton;

        RegisterButtonEvents();
    }

    private void OnEnable()
    {
        RegisterButtonEvents();

        _playerHUD = GetComponentInChildren<PlayerHUD>();
        if (_playerHUD != null)
            _playerHUD.RefreshUI();
    }

    private void RegisterButtonEvents()
    {
        if (_waitCustomerButton != null)
        {
            _waitCustomerButton.onClick.RemoveListener(OnWaitCustomerClicked);
            _waitCustomerButton.onClick.AddListener(OnWaitCustomerClicked);
        }
        if (_shopButton != null)
        {
            _shopButton.onClick.RemoveListener(OnShopClicked);
            _shopButton.onClick.AddListener(OnShopClicked);
        }
        if (_collectionButton != null)
        {
            _collectionButton.onClick.RemoveListener(OnCollectionClicked);
            _collectionButton.onClick.AddListener(OnCollectionClicked);
        }
        if (_settingsButton != null)
        {
            _settingsButton.onClick.RemoveListener(OnSettingsClicked);
            _settingsButton.onClick.AddListener(OnSettingsClicked);
        }
    }

    private void OnDisable()
    {
        // 버튼 이벤트 제거
        if (_waitCustomerButton != null)
            _waitCustomerButton.onClick.RemoveListener(OnWaitCustomerClicked);
        if (_shopButton != null)
            _shopButton.onClick.RemoveListener(OnShopClicked);
        if (_collectionButton != null)
            _collectionButton.onClick.RemoveListener(OnCollectionClicked);
        if (_settingsButton != null)
            _settingsButton.onClick.RemoveListener(OnSettingsClicked);
    }

    private void OnWaitCustomerClicked()
    {
        Debug.Log("손님 대기 클릭");

        // 손님 생성 후 등장 화면 표시
        CustomerManager.Instance.SpawnRandomCustomer();

        Canvas canvas = UnityEngine.Object.FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            CustomerAppearanceUIBuilder customerUI = new CustomerAppearanceUIBuilder(canvas, UIManager.Instance);
            customerUI.Build();
        }
    }

    private void OnShopClicked()
    {
        Debug.Log("상점 클릭");
        // TODO: 상점 화면 이동 (Step ?)
    }

    private void OnCollectionClicked()
    {
        Debug.Log("컬렉션 클릭");
        // TODO: 컬렉션 화면 이동 (Step ?)
    }

    private void OnSettingsClicked()
    {
        Debug.Log("설정 클릭");
        UIManager.Instance.ShowPanel("SettingsPanel");
    }
}
