using UnityEngine;
using TMPro;

/// <summary>
/// 인게임 HUD의 텍스트와 이미지 요소들의 참조를 보관하고, Controller가 제공한 포맷팅된 문자열을 표시하는 View 클래스입니다.
/// </summary>
public class HUDView : MonoBehaviour
{
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text dailyEarningsText;
    [SerializeField] private TMP_Text guestCountText;
    [SerializeField] private TMP_Text pendingOrderCountText;

    public void UpdateDayText(string text)
    {
        if (dayText != null)
            dayText.text = text;
    }

    public void UpdateMoneyText(string text)
    {
        if (moneyText != null)
            moneyText.text = text;
    }

    public void UpdateDailyEarningsText(string text)
    {
        if (dailyEarningsText != null)
            dailyEarningsText.text = text;
    }

    public void UpdateGuestCountText(string text)
    {
        if (guestCountText != null)
            guestCountText.text = text;
    }

    public void UpdatePendingOrderCountText(string text)
    {
        if (pendingOrderCountText != null)
            pendingOrderCountText.text = text;
    }
}
