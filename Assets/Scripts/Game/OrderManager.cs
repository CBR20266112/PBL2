using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 손님의 주문 생성 및 생명주기 관리 매니저입니다.
/// 손님 생성이나 음료 제조를 직접 수행하지 않고 주문 데이터의 검증 및 상태 변경만 담당합니다.
/// </summary>
public class OrderManager : Singleton<OrderManager>
{
    [Header("데이터베이스 연결")]
    [Tooltip("Inspector에서 DrinkDatabase 에셋을 연결합니다.")]
    [SerializeField] private DrinkDatabase _drinkDatabase;

    private OrderData _currentOrder;
    private int _orderIdCounter = 0;

    // 주문 상태 변경 이벤트
    public delegate void OrderChangedHandler(OrderData order);
    public event OrderChangedHandler OnOrderGenerated;
    public event OrderChangedHandler OnOrderCompleted;
    public event OrderChangedHandler OnOrderCancelled;

    private void Start()
    {
        // CustomerManager 손님 도착 이벤트 구독
        if (CustomerManager.Instance != null)
        {
            CustomerManager.Instance.OnCustomerArrived += OnCustomerArrivedHandler;
        }
    }

    private void OnDestroy()
    {
        if (CustomerManager.Instance != null)
        {
            CustomerManager.Instance.OnCustomerArrived -= OnCustomerArrivedHandler;
        }
    }

    private void OnCustomerArrivedHandler(CustomerData customerData)
    {
        GenerateOrder(customerData);
    }

    /// <summary>
    /// 손님 데이터를 받아 신규 주문을 생성합니다.
    /// </summary>
    public OrderData GenerateOrder(CustomerData customerData)
    {
        if (customerData == null) return null;

        DrinkData targetDrink = SelectDrinkForCustomer(customerData);
        if (targetDrink == null)
        {
            Debug.LogWarning("[OrderManager] 주문 가능한 음료가 없습니다.");
            return null;
        }

        _orderIdCounter++;
        _currentOrder = new OrderData
        {
            orderId = $"order_{_orderIdCounter:D6}",
            customerId = customerData.customerId,
            requestedDrinkId = targetDrink.drinkId,
            isCompleted = false
        };

        Debug.Log($"[OrderManager] 주문 생성: {_currentOrder.orderId} (손님: {customerData.customerId}, 요구음료: {targetDrink.drinkId})");
        OnOrderGenerated?.Invoke(_currentOrder);
        return _currentOrder;
    }

    /// <summary>
    /// 완성된 음료 ID를 전달받아 주문 성공 여부를 최종 검증하고 완료 처리합니다.
    /// </summary>
    /// <param name="brewedDrinkId">제조 완료된 음료 ID</param>
    /// <returns>주문 완료 처리 성공 여부 (요구 음료와 일치 시 true)</returns>
    public bool CompleteOrder(string brewedDrinkId)
    {
        if (!HasActiveOrder())
        {
            Debug.LogWarning("[OrderManager] 완료 처리할 활성화된 주문이 없습니다.");
            return false;
        }

        if (_currentOrder.requestedDrinkId != brewedDrinkId)
        {
            Debug.LogWarning($"[OrderManager] 주문 불일치! (요구: {_currentOrder.requestedDrinkId}, 제공: {brewedDrinkId})");
            return false;
        }

        _currentOrder.isCompleted = true;
        Debug.Log($"[OrderManager] 주문 성공 완료: {_currentOrder.orderId}");

        OrderData completedOrder = _currentOrder;
        _currentOrder = null;

        OnOrderCompleted?.Invoke(completedOrder);
        return true;
    }

    /// <summary>
    /// 현재 진행 중인 주문을 취소합니다.
    /// </summary>
    public void CancelOrder()
    {
        if (_currentOrder != null)
        {
            Debug.Log($"[OrderManager] 주문 취소: {_currentOrder.orderId}");

            OrderData cancelledOrder = _currentOrder;
            _currentOrder = null;

            OnOrderCancelled?.Invoke(cancelledOrder);
        }
    }

    /// <summary>
    /// 현재 진행 중인 주문 데이터를 반환합니다.
    /// </summary>
    public OrderData GetCurrentOrder()
    {
        return _currentOrder;
    }

    /// <summary>
    /// 현재 활성화된(완료되지 않은) 주문이 존재하는지 반환합니다.
    /// </summary>
    public bool HasActiveOrder()
    {
        return _currentOrder != null && !_currentOrder.isCompleted;
    }

    /// <summary>
    /// 현재 활성화된(완료되지 않은) 주문의 개수를 반환합니다.
    /// </summary>
    public int GetActiveOrderCount()
    {
        return HasActiveOrder() ? 1 : 0;
    }

    /// <summary>
    /// 손님 데이터를 기반으로 주문할 음료를 선택합니다.
    /// 향후 국가별 확률, 선호도, 이벤트 주문 등으로 확장하기 쉬운 구조입니다.
    /// </summary>
    public DrinkData SelectDrinkForCustomer(CustomerData customer)
    {
        if (_drinkDatabase == null || _drinkDatabase.drinks.Count == 0) return null;

        // 현재는 무작위 선택 처리 (향후 customer.nationality, preferredTea, unlockLevel 등 연동 가능)
        List<DrinkData> availableDrinks = _drinkDatabase.drinks;
        return availableDrinks[Random.Range(0, availableDrinks.Count)];
    }
}
