using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 손님 매니저
/// 손님 데이터 로드 및 관리
/// </summary>
public class CustomerManager : Singleton<CustomerManager>
{
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
        // Resources 폴더에서 모든 CustomerData 로드
        CustomerData[] loadedCustomers = Resources.LoadAll<CustomerData>("ScriptableObjects/Customers");
        
        // 로드된 데이터가 없으면 기본 데이터 생성
        if (loadedCustomers.Length == 0)
        {
            Debug.LogWarning("No CustomerData found in Resources. Creating default customers...");
            loadedCustomers = CustomerDataHelper.CreateDefaultCustomers();
        }

        _allCustomers.AddRange(loadedCustomers);
        Debug.Log($"Loaded {_allCustomers.Count} customers");
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
