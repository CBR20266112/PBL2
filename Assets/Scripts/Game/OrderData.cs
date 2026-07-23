using UnityEngine;

/// <summary>
/// 주문 데이터 클래스
/// </summary>
[System.Serializable]
public class OrderData
{
    [Tooltip("주문 고유 ID (예: order_000001)")]
    public string orderId;

    [Tooltip("주문한 손님 고유 ID")]
    public string customerId;

    [Tooltip("요구하는 음료 ID (DrinkData.drinkId 참조)")]
    public string requestedDrinkId;

    [Tooltip("주문 완료 여부")]
    public bool isCompleted;
}
