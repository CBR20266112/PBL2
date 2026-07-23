using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 손님 생명주기(선택, 대기열, 현재 손님) 관리 매니저입니다.
/// Instantiate(화면 렌더링)나 주문 생성(OrderManager)은 담당하지 않고, 데이터 상태 관리 및 Hook만 노출합니다.
/// </summary>
public class CustomerManager : Singleton<CustomerManager>
{
    [Header("데이터베이스 연결")]
    [Tooltip("Inspector에서 CustomerDatabase 에셋을 연결합니다.")]
    [SerializeField] private CustomerDatabase _customerDatabase;

    private readonly Queue<CustomerData> _waitingCustomerQueue = new Queue<CustomerData>();
    private CustomerData _currentCustomerData;

    // 손님 도착/준비 완료 이벤트 (향후 OrderManager 및 View/Spawner 연동용)
    public delegate void CustomerArrivedHandler(CustomerData customerData);
    public event CustomerArrivedHandler OnCustomerArrived;

    protected override void Awake()
    {
        base.Awake();
        if (_customerDatabase == null)
        {
            Debug.LogError("[CustomerManager] _customerDatabase 참조가 할당되지 않았습니다. Inspector를 확인해 주세요.");
        }
    }

    /// <summary>
    /// 무작위 손님 데이터를 선택하여 대기열(Queue)에 추가합니다.
    /// DayState.Operating 상태에서만 손님을 대기열에 추가할 수 있습니다.
    /// </summary>
    /// <returns>대기열에 새로 추가된 CustomerData (실패 시 null)</returns>
    public CustomerData SpawnCustomer()
    {
        // DayManager 상태 확인 (Operating 상태에서만 대기열 추가)
        if (DayManager.Instance != null && DayManager.Instance.CurrentState != DayState.Operating)
        {
            Debug.LogWarning("[CustomerManager] 영업 중(Operating) 상태가 아니므로 손님을 추가하지 않습니다.");
            return null;
        }

        if (_customerDatabase == null)
        {
            Debug.LogWarning("[CustomerManager] CustomerDatabase가 연결되어 있지 않습니다.");
            return null;
        }

        CustomerData randomData = _customerDatabase.GetRandomCustomer();
        if (randomData == null)
        {
            Debug.LogWarning("[CustomerManager] 손님 데이터가 존재하지 않습니다.");
            return null;
        }

        _waitingCustomerQueue.Enqueue(randomData);
        Debug.Log($"[CustomerManager] 손님 대기열 추가: {randomData.customerId} (대기 수: {_waitingCustomerQueue.Count})");
        return randomData;
    }

    /// <summary>
    /// 현재 선택되어 서빙 중인 손님을 제거/완료 처리합니다.
    /// </summary>
    public void RemoveCustomer()
    {
        if (_currentCustomerData != null)
        {
            Debug.Log($"[CustomerManager] 현재 손님 퇴장 완료: {_currentCustomerData.customerId}");
            _currentCustomerData = null;
        }
    }

    /// <summary>
    /// 대기열의 다음 손님을 현재 손님으로 전환하고 OrderManager/View 알림 Hook을 발생시킵니다.
    /// </summary>
    public CustomerData CallNextCustomer()
    {
        if (_waitingCustomerQueue.Count == 0)
        {
            return null;
        }

        _currentCustomerData = _waitingCustomerQueue.Dequeue();
        
        // OrderManager 및 View 계층에 알리기 위한 Hook 호출
        NotifyCustomerReady(_currentCustomerData);

        return _currentCustomerData;
    }

    /// <summary>
    /// 현재 활성화된 손님 데이터를 반환합니다.
    /// </summary>
    public CustomerData GetCurrentCustomerData()
    {
        return _currentCustomerData;
    }

    /// <summary>
    /// 현재 서빙 중이거나 대기열에 손님이 존재하는지 여부를 반환합니다.
    /// </summary>
    public bool HasCustomer()
    {
        return _currentCustomerData != null || _waitingCustomerQueue.Count > 0;
    }

    /// <summary>
    /// 손님이 준비되었음을 OrderManager 및 View 계층에 알리는 Hook 메서드입니다.
    /// </summary>
    public void NotifyCustomerReady(CustomerData customerData)
    {
        // TODO: 향후 OrderManager.Instance.GenerateOrder(customerData) 호출 연동 지점
        Debug.Log($"[CustomerManager] 손님 준비 알림 (NotifyCustomerReady): {customerData?.customerId}");
        OnCustomerArrived?.Invoke(customerData);
    }

    // ──────────────────────────────────────────────────────────────────────
    // 기존 호환용 API (하위 호환성 및 래퍼)
    // ──────────────────────────────────────────────────────────────────────

    public void SpawnRandomCustomer() => SpawnCustomer();

    public Customer GetCurrentCustomer()
    {
        // 기존 Customer 컴포넌트 호환용 (단순 래퍼)
        return null;
    }

    public void RemoveCurrentCustomer() => RemoveCustomer();

    public int GetWaitingCustomerCount() => _waitingCustomerQueue.Count;
}
