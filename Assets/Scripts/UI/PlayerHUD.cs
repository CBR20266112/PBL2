using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어 HUD (Heads-Up Display)
/// 상단에 플레이어 정보 표시 (레벨, 돈, 경험치)
/// </summary>
public class PlayerHUD : MonoBehaviour
{
    public Text _levelText;
    public Text _moneyText;
    public Image _expFillImage;

    private void Start()
    {
        RefreshUI();
    }

    public void Initialize(Text levelText, Text moneyText, Image expFillImage)
    {
        _levelText = levelText;
        _moneyText = moneyText;
        _expFillImage = expFillImage;
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (PlayerDataManager.Instance == null)
            return;

        if (_levelText != null)
            _levelText.text = $"Lv.{PlayerDataManager.Instance.CurrentLevel}";

        if (_moneyText != null)
            _moneyText.text = $"₩{PlayerDataManager.Instance.CurrentMoney:N0}";

        if (_expFillImage != null)
        {
            int nextLevelExp = GameConstants.BASE_EXP_PER_LEVEL +
                              (PlayerDataManager.Instance.CurrentLevel - 1) * 200;
            float expRatio = Mathf.Clamp01((float)PlayerDataManager.Instance.CurrentExp / nextLevelExp);
            _expFillImage.fillAmount = expRatio;
        }
    }
}
