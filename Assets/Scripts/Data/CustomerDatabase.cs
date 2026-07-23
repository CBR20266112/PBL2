using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 손님 데이터베이스 (ScriptableObject)
/// </summary>
[CreateAssetMenu(fileName = "CustomerDatabase", menuName = "Tea Culture Game/Customer Database")]
public class CustomerDatabase : ScriptableObject
{
    [Tooltip("등록된 손님 데이터 목록")]
    public List<CustomerData> customers = new List<CustomerData>();

    private readonly Dictionary<string, CustomerData> _customerMap = new Dictionary<string, CustomerData>();

    /// <summary>
    /// 딕셔너리 맵 초기화
    /// </summary>
    public void Initialize()
    {
        _customerMap.Clear();
        foreach (var c in customers)
        {
            if (c != null && !string.IsNullOrEmpty(c.customerId))
            {
                _customerMap[c.customerId] = c;
            }
        }
    }

    /// <summary>
    /// ID로 손님 데이터 조회
    /// </summary>
    public CustomerData GetCustomer(string customerId)
    {
        if (_customerMap.Count == 0) Initialize();
        _customerMap.TryGetValue(customerId, out CustomerData data);
        return data;
    }

    /// <summary>
    /// 무작위 손님 데이터 하나 선택
    /// </summary>
    public CustomerData GetRandomCustomer()
    {
        if (customers == null || customers.Count == 0) return null;
        return customers[Random.Range(0, customers.Count)];
    }

    private void OnEnable()
    {
        Initialize();
    }
}
