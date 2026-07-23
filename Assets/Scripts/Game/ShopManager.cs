using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 재료 상점(Shop)의 상태 관리와 구매 로직을 정의하는 매니저 클래스입니다.
/// UI나 구매 연출과 독립되어 데이터 유효성 및 인벤토리 추가 기능만 제공합니다.
/// </summary>
public class ShopManager : Singleton<ShopManager>
{
    [Header("데이터베이스 연결")]
    [Tooltip("상점에서 판매할 수 있는 재료 정보 취득을 위해 IngredientDatabase 에셋을 연결합니다.")]
    [SerializeField] private IngredientDatabase _ingredientDatabase;

    // 상점 오픈 여부 (기본값 false, DayManager 등 외부 시스템에서 제어)
    public bool IsShopOpen { get; private set; } = false;

    // 구매 성공 이벤트
    public delegate void PurchaseSuccessHandler(string ingredientId, int amount, int totalCost);
    public event PurchaseSuccessHandler OnPurchaseSuccess;

    // 구매 실패 이벤트
    public delegate void PurchaseFailHandler(string ingredientId, string reason);
    public event PurchaseFailHandler OnPurchaseFailed;

    /// <summary>
    /// 상점 활성화 상태를 변경합니다. (DayManager 등에서 일별 시작/종료 시 호출)
    /// </summary>
    public void SetShopOpen(bool isOpen)
    {
        IsShopOpen = isOpen;
        Debug.Log($"[ShopManager] 상점 상태 변경: {(isOpen ? "오픈" : "마감")}");
    }

    /// <summary>
    /// 특정 재료를 지정 수량만큼 구매 가능한지 여부를 확인합니다.
    /// </summary>
    public bool CanPurchase(string ingredientId, int amount = 1)
    {
        if (!IsShopOpen || amount <= 0 || _ingredientDatabase == null) return false;

        IngredientData ingredient = _ingredientDatabase.GetIngredient(ingredientId);
        if (ingredient == null) return false;

        int totalCost = ingredient.buyPrice * amount;
        if (MoneyManager.Instance != null)
        {
            return MoneyManager.Instance.CanAfford(totalCost);
        }

        return true;
    }

    /// <summary>
    /// 상점에서 구매 가능한 모든 재료 목록을 반환합니다.
    /// </summary>
    public List<IngredientData> GetAvailableIngredients()
    {
        if (_ingredientDatabase == null)
        {
            Debug.LogWarning("[ShopManager] IngredientDatabase가 지정되지 않아 빈 목록을 반환합니다.");
            return new List<IngredientData>();
        }

        // 전체 재료 목록을 그대로 복사하여 반환 (unlockLevel 기반 제한 제외)
        return new List<IngredientData>(_ingredientDatabase.ingredients);
    }

    /// <summary>
    /// 특정 재료를 지정한 수량만큼 구매하여 인벤토리에 추가합니다.
    /// </summary>
    /// <param name="ingredientId">구매할 재료 ID</param>
    /// <param name="amount">구매할 수량 (양수)</param>
    /// <returns>구매 진행 성공 여부</returns>
    public bool BuyIngredient(string ingredientId, int amount)
    {
        // 1. 상점 영업 여부 검증
        if (!IsShopOpen)
        {
            string reason = "상점 영업 시간이 아닙니다.";
            Debug.LogWarning($"[ShopManager] 구매 실패: {reason}");
            OnPurchaseFailed?.Invoke(ingredientId, reason);
            return false;
        }

        // 2. 구매 수량 검증
        if (amount <= 0)
        {
            string reason = "구매 수량은 0보다 커야 합니다.";
            Debug.LogWarning($"[ShopManager] 구매 실패: {reason}");
            OnPurchaseFailed?.Invoke(ingredientId, reason);
            return false;
        }

        // 3. 재료 데이터베이스 유효성 및 존재 검증
        if (_ingredientDatabase == null)
        {
            string reason = "재료 데이터베이스가 연결되어 있지 않습니다.";
            Debug.LogError($"[ShopManager] 구매 실패: {reason}");
            OnPurchaseFailed?.Invoke(ingredientId, reason);
            return false;
        }

        IngredientData ingredient = _ingredientDatabase.GetIngredient(ingredientId);
        if (ingredient == null)
        {
            string reason = $"존재하지 않는 재료 ID '{ingredientId}'입니다.";
            Debug.LogWarning($"[ShopManager] 구매 실패: {reason}");
            OnPurchaseFailed?.Invoke(ingredientId, reason);
            return false;
        }

        // 4. 재화(돈) 검사 및 차감 (임시 훅 활용)
        int totalCost = ingredient.buyPrice * amount;
        if (!CheckAndConsumeMoney(totalCost))
        {
            string reason = "소지금이 부족합니다.";
            Debug.LogWarning($"[ShopManager] 구매 실패: {reason}");
            OnPurchaseFailed?.Invoke(ingredientId, reason);
            return false;
        }

        // 5. 인벤토리에 구매 품목 추가
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddIngredient(ingredientId, amount);
        }
        else
        {
            Debug.LogWarning("[ShopManager] InventoryManager 인스턴스를 찾을 수 없어 재고가 추가되지 않았습니다.");
        }

        Debug.Log($"[ShopManager] 구매 성공: {ingredient.displayName} x{amount} (총액: {totalCost}원)");
        OnPurchaseSuccess?.Invoke(ingredientId, amount, totalCost);
        return true;
    }

    /// <summary>
    /// 플레이어 재화를 확인하고 소모하는 Hook입니다.
    /// MoneyManager 싱글톤을 호출하여 실제 보유 자금을 차감합니다.
    /// </summary>
    private bool CheckAndConsumeMoney(int totalCost)
    {
        if (MoneyManager.Instance != null)
        {
            return MoneyManager.Instance.SpendMoney(totalCost);
        }

        // MoneyManager가 씬에 없으면 안전 장치로 통과시킵니다.
        Debug.LogWarning("[ShopManager] MoneyManager 인스턴스를 찾을 수 없습니다.");
        return true;
    }
}
