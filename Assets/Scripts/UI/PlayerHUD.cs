using UnityEngine;
using TMPro;

/// <summary>
/// 플레이어 HUD (Heads-Up Display)
/// 상단에 플레이어 정보 표시 (레벨, 돈, 경험치)
/// </summary>
public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private Image _expFillImage;

    private void Start()
    {
        RefreshUI();

        // PlayerDataManager의 이벤트 구독 (나중에 추가)
        // PlayerDataManager.Instance.OnDataChanged += RefreshUI;
    }

    public void RefreshUI()
    {
        if (PlayerDataManager.Instance == null)
            return;

        // 레벨 표시
        if (_levelText != null)
            _levelText.text = $"Lv.{PlayerDataManager.Instance.CurrentLevel}";

        // 돈 표시
        if (_moneyText != null)
            _moneyText.text = $"₩{PlayerDataManager.Instance.CurrentMoney:N0}";

        // 경험치 표시 (임시)
        if (_expFillImage != null)
        {
            int nextLevelExp = GameConstants.BASE_EXP_PER_LEVEL + 
                              (PlayerDataManager.Instance.CurrentLevel - 1) * 200;
            float expRatio = Mathf.Clamp01((float)PlayerDataManager.Instance.CurrentExp / nextLevelExp);
            _expFillImage.fillAmount = expRatio;
        }
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제 (나중에 추가)
    }
}
