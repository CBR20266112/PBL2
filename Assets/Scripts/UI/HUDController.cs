using UnityEngine;

/// <summary>
/// 각 Manager의 이벤트를 구독하여 HUD 데이터를 갱신하고, 문자열로 포맷팅하여 View에 전달하는 Controller 클래스입니다.
/// </summary>
public class HUDController : MonoBehaviour
{
    [SerializeField] private HUDView view;

    private void OnEnable()
    {
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnDayNumberChanged += HandleDayNumberChanged;
            DayManager.Instance.OnDailyEarningsChanged += HandleDailyEarningsChanged;
            DayManager.Instance.OnDailyServedCustomersChanged += HandleDailyServedCustomersChanged;
        }

        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.OnMoneyChanged += HandleMoneyChanged;
        }

        if (OrderManager.Instance != null)
        {
            OrderManager.Instance.OnOrderGenerated += HandleOrderChanged;
            OrderManager.Instance.OnOrderCompleted += HandleOrderChanged;
            OrderManager.Instance.OnOrderCancelled += HandleOrderChanged;
        }

        RefreshAll();
    }

    private void OnDisable()
    {
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnDayNumberChanged -= HandleDayNumberChanged;
            DayManager.Instance.OnDailyEarningsChanged -= HandleDailyEarningsChanged;
            DayManager.Instance.OnDailyServedCustomersChanged -= HandleDailyServedCustomersChanged;
        }

        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.OnMoneyChanged -= HandleMoneyChanged;
        }

        if (OrderManager.Instance != null)
        {
            OrderManager.Instance.OnOrderGenerated -= HandleOrderChanged;
            OrderManager.Instance.OnOrderCompleted -= HandleOrderChanged;
            OrderManager.Instance.OnOrderCancelled -= HandleOrderChanged;
        }
    }

    /// <summary>
    /// 모든 HUD 요소들의 초기 동기화를 수행합니다.
    /// </summary>
    private void RefreshAll()
    {
        if (DayManager.Instance != null)
        {
            HandleDayNumberChanged(DayManager.Instance.CurrentDayNumber);
            HandleDailyEarningsChanged(DayManager.Instance.DailyEarnings);
            HandleDailyServedCustomersChanged(DayManager.Instance.DailyServedCustomers);
        }

        if (MoneyManager.Instance != null)
        {
            HandleMoneyChanged(MoneyManager.Instance.CurrentMoney, 0);
        }

        if (OrderManager.Instance != null)
        {
            HandleOrderChanged(null); // 이벤트 핸들러 인자를 무시하므로 null 가능
        }
    }

    private void HandleDayNumberChanged(int newDayNumber)
    {
        if (view != null)
        {
            string formattedString = $"{newDayNumber}일차";
            view.UpdateDayText(formattedString);
        }
    }

    private void HandleDailyEarningsChanged(int newEarnings)
    {
        if (view != null)
        {
            string formattedString = $"+{newEarnings}";
            view.UpdateDailyEarningsText(formattedString);
        }
    }

    private void HandleDailyServedCustomersChanged(int newCount)
    {
        if (view != null)
        {
            string formattedString = $"{newCount}명";
            view.UpdateGuestCountText(formattedString);
        }
    }

    private void HandleMoneyChanged(int currentMoney, int delta = 0)
    {
        if (view != null)
        {
            string formattedString = $"{currentMoney}냥";
            view.UpdateMoneyText(formattedString);
        }
    }

    private void HandleOrderChanged(OrderData order)
    {
        if (view != null && OrderManager.Instance != null)
        {
            int activeCount = OrderManager.Instance.GetActiveOrderCount();
            string formattedString = $"{activeCount}";
            view.UpdatePendingOrderCountText(formattedString);
        }
    }
}
