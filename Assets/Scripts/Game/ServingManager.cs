using UnityEngine;

/// <summary>
/// 음료 제공(Serve)의 전체 흐름을 조정하는 오케스트레이터 클래스입니다.
/// 직접 제조, 주문 생성, 손님 생성, UI 렌더링을 하지 않으며 각 Manager 간의 연쇄 처리를 담당합니다.
/// </summary>
public class ServingManager : Singleton<ServingManager>
{
    [Header("데이터베이스 연결")]
    [Tooltip("음료 냥전 가격 조회를 위한 DrinkDatabase 참조")]
    [SerializeField] private DrinkDatabase _drinkDatabase;

    // 이벤트 정의 (GameFlow/Presenter/View/AffinityManager 등에서 구독)
    public delegate void ServeSucceededHandler(string customerId, string drinkId, int earnedMoney);
    public event ServeSucceededHandler OnServeSucceeded;

    public delegate void ServeFailedHandler(string drinkId, string reason);
    public event ServeFailedHandler OnServeFailed;

    protected override void Awake()
    {
        base.Awake();
        if (_drinkDatabase == null)
        {
            Debug.LogError("[ServingManager] _drinkDatabase 참조가 할당되지 않았습니다. Inspector를 확인해 주세요.");
        }
    }

    /// <summary>
    /// 완성된 음료 ID를 받아 주문 검증 및 서빙 연쇄 처리를 실행합니다.
    /// </summary>
    /// <param name="brewedDrinkId">제조 완료된 음료 ID</param>
    /// <returns>서빙 성공 여부</returns>
    public bool ServeDrink(string brewedDrinkId)
    {
        if (string.IsNullOrEmpty(brewedDrinkId))
        {
            HandleServeFailure(brewedDrinkId, "음료 ID가 올바르지 않습니다.");
            return false;
        }

        // 주문 검증 완료 전 customerId 백업 (CompleteOrder 완료 시 currentOrder가 초기화될 수 있음)
        string customerId = string.Empty;
        if (OrderManager.Instance != null)
        {
            OrderData currentOrder = OrderManager.Instance.GetCurrentOrder();
            if (currentOrder != null)
            {
                customerId = currentOrder.customerId;
            }
        }

        // 1. OrderManager에 주문 일치 검증 요청
        if (OrderManager.Instance == null || !OrderManager.Instance.CompleteOrder(brewedDrinkId))
        {
            HandleServeFailure(brewedDrinkId, "주문과 일치하지 않는 음료입니다.");
            return false;
        }

        // 2. 음료 가격 (냥전) 조회
        // TODO: 향후 할인, VIP, 이벤트 보상 등을 위해 OrderData가 최종 보상 금액을 보관하는 구조로 확장 예정입니다.
        int price = 0;
        if (_drinkDatabase != null)
        {
            DrinkData drink = _drinkDatabase.GetDrink(brewedDrinkId);
            if (drink != null)
            {
                price = drink.price;
            }
        }

        // 3. 순서대로 성공 연쇄 처리 호출
        // 3-1. 재화(냥전) 추가
        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.AddMoney(price);
        }

        // 3-2. DayManager 통계 기록 (매출 냥전 및 서빙 성공 횟수)
        if (DayManager.Instance != null)
        {
            DayManager.Instance.RecordSale(price);
            DayManager.Instance.RecordCustomerServed();
        }

        // 3-3. CustomerManager 데이터 상 손님 퇴장 처리
        if (CustomerManager.Instance != null)
        {
            CustomerManager.Instance.RemoveCustomer();
        }

        // 4. 확장 보상 훅 실행 (별점 품질, 팁, 친밀도, XP 등)
        ProcessServeSuccessRewardsHook(brewedDrinkId, price);

        Debug.Log($"[ServingManager] 서빙 성공! 손님: {customerId}, 음료: {brewedDrinkId}, 획득 냥전: {price}");

        // 5. 성공 이벤트 발생 (customerId, drinkId, price 전달)
        OnServeSucceeded?.Invoke(customerId, brewedDrinkId, price);
        return true;
    }

    /// <summary>
    /// 서빙 실패 시의 예외 처리 및 확장 패널티 훅을 실행합니다.
    /// </summary>
    private void HandleServeFailure(string drinkId, string reason)
    {
        ProcessServeFailurePenaltyHook(drinkId, reason);

        Debug.LogWarning($"[ServingManager] 서빙 실패! 사유: {reason}");
        OnServeFailed?.Invoke(drinkId, reason);
    }

    /// <summary>
    /// (Hook) 서빙 성공 시 추가 확장 요소(별점 품질, 팁, 친밀도, XP, 퀘스트 등)를 처리하는 훅입니다.
    /// </summary>
    private void ProcessServeSuccessRewardsHook(string drinkId, int price)
    {
        // TODO: 향후 PlayerDataManager.AddExp(), Quest/Achievement 연동
    }

    /// <summary>
    /// (Hook) 서빙 실패 시 패널티 요소(손님 불만족, 패널티 차감 등)를 처리하는 훅입니다.
    /// </summary>
    private void ProcessServeFailurePenaltyHook(string drinkId, string reason)
    {
        // TODO: 향후 실패 패널티 시스템 연동
    }
}
