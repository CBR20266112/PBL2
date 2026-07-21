using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 손님 매니저
/// 손님 데이터 로드 및 관리
/// </summary>
public class CustomerManager : Singleton<CustomerManager>
{
    [Header("Customer Data")]
    [Tooltip("Inspector에서 CustomerData ScriptableObject를 직접 연결합니다.\nAssets/Sprites/Character/Customers/ 에셋과 함께 설정하세요.")]
    [SerializeField] private List<CustomerData> _customerDataList = new List<CustomerData>();

    private List<CustomerData> _allCustomers = new List<CustomerData>();
    private Queue<Customer> _waitingCustomers = new Queue<Customer>();
    private Customer _currentCustomer;

    protected override void Awake()
    {
        base.Awake();
        LoadAllCustomers();
    }

    private void LoadAllCustomers()
    {
        _allCustomers.Clear();

        // 1순위: Inspector에서 직접 연결된 CustomerData 사용
        if (_customerDataList != null && _customerDataList.Count > 0)
        {
            _allCustomers.AddRange(_customerDataList);
            Debug.Log($"[CustomerManager] Inspector 연결 데이터 {_allCustomers.Count}명 로드");
            return;
        }

        // 2순위(폴백): Resources 폴더에서 로드 시도
        CustomerData[] loadedCustomers = Resources.LoadAll<CustomerData>("ScriptableObjects/Customers");
        if (loadedCustomers.Length > 0)
        {
            _allCustomers.AddRange(loadedCustomers);
            Debug.Log($"[CustomerManager] Resources 폴더에서 {_allCustomers.Count}명 로드");
            return;
        }

        // 3순위(폴백): 하드코딩 기본 데이터 (에셋 미연결 시 임시 동작 보장)
        Debug.LogWarning("[CustomerManager] CustomerData 없음 — 기본 데이터로 실행합니다. Inspector에서 CustomerData를 연결해 주세요.");
        _allCustomers.AddRange(CustomerDataHelper.CreateDefaultCustomers());
        Debug.Log($"[CustomerManager] 기본 데이터 {_allCustomers.Count}명 로드");
    }

    /// <summary>
    /// 무작위 손님 생성 및 대기 큐에 추가
    /// </summary>
    public void SpawnRandomCustomer()
    {
        if (_allCustomers.Count == 0)
        {
            Debug.LogWarning("No customers loaded!");
            return;
        }

        // 무작위 손님 선택
        CustomerData randomData = _allCustomers[Random.Range(0, _allCustomers.Count)];

        // Customer GameObject 생성
        GameObject customerObj = new GameObject(randomData.customerName);
        Customer customer = customerObj.AddComponent<Customer>();
        customer.Initialize(randomData);

        // 대기 큐에 추가
        _waitingCustomers.Enqueue(customer);
        Debug.Log($"Customer spawned: {randomData.customerName}, Queue count: {_waitingCustomers.Count}");
    }

    /// <summary>
    /// 대기 중인 손님을 현재 손님으로 호출
    /// </summary>
    public Customer CallNextCustomer()
    {
        if (_waitingCustomers.Count == 0)
        {
            Debug.LogWarning("No waiting customers!");
            return null;
        }

        _currentCustomer = _waitingCustomers.Dequeue();
        _currentCustomer.OnVisited();
        Debug.Log($"Customer called: {_currentCustomer.data.customerName}");
        return _currentCustomer;
    }

    /// <summary>
    /// 현재 손님 반환
    /// </summary>
    public Customer GetCurrentCustomer()
    {
        return _currentCustomer;
    }

    /// <summary>
    /// 현재 손님 제거 (손님 서빙 완료)
    /// </summary>
    public void RemoveCurrentCustomer()
    {
        if (_currentCustomer != null)
        {
            Destroy(_currentCustomer.gameObject);
            _currentCustomer = null;
            Debug.Log("Current customer removed");
        }
    }

    /// <summary>
    /// 대기 중인 손님 수
    /// </summary>
    public int GetWaitingCustomerCount()
    {
        return _waitingCustomers.Count;
    }
}
