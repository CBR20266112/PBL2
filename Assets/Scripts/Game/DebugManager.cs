using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
/// <summary>
/// 디버그 명령 요청을 받아 기존 시스템 매니저들의 Public API만 순수하게 1:1로 전달/호출하는 얇은 Wrapper 매니저입니다.
/// 내부에서 씬/DB를 직접 순회하거나 복합 게임 규칙 로직을 구현하지 않습니다.
/// 릴리즈 빌드 시 전처리기(#if UNITY_EDITOR || DEVELOPMENT_BUILD)에 의해 컴파일에서 전면 제외됩니다.
/// </summary>
public class DebugManager : Singleton<DebugManager>
{
    // ──────────────────────────────────────────────────────────────────────
    // 1. Money Category
    // ──────────────────────────────────────────────────────────────────────

    public void AddMoney(int amount)
    {
        if (MoneyManager.Instance != null)
            MoneyManager.Instance.AddMoney(amount);
    }

    public void ResetMoney()
    {
        if (MoneyManager.Instance != null)
            MoneyManager.Instance.SpendMoney(MoneyManager.Instance.CurrentMoney);
    }

    // ──────────────────────────────────────────────────────────────────────
    // 2. Day Category
    // ──────────────────────────────────────────────────────────────────────

    public void AdvanceDay()
    {
        if (DayManager.Instance != null)
            DayManager.Instance.AdvanceToNextDay();
    }

    public void ForceEndDay()
    {
        if (DayManager.Instance != null)
            DayManager.Instance.EndDay();
    }

    // ──────────────────────────────────────────────────────────────────────
    // 3. Customer Category
    // ──────────────────────────────────────────────────────────────────────

    public void SpawnCustomer()
    {
        if (CustomerManager.Instance != null)
            CustomerManager.Instance.SpawnCustomer();
    }

    public void CompleteCurrentOrder()
    {
        if (OrderManager.Instance != null && OrderManager.Instance.HasActiveOrder())
        {
            OrderData order = OrderManager.Instance.GetCurrentOrder();
            if (order != null)
            {
                OrderManager.Instance.CompleteOrder(order.requestedDrinkId);
            }
        }
    }

    // ──────────────────────────────────────────────────────────────────────
    // 4. Unlock Category
    // ──────────────────────────────────────────────────────────────────────

    public void UnlockAll(IEnumerable<string> ids)
    {
        if (UnlockManager.Instance != null)
            UnlockManager.Instance.UnlockAll(ids);
    }

    public void OwnFurniture(string furnitureId)
    {
        if (FurnitureManager.Instance != null)
            FurnitureManager.Instance.OwnFurniture(furnitureId);
    }

    public void AddAffinity(string customerId, int amount)
    {
        if (AffinityManager.Instance != null)
            AffinityManager.Instance.AddAffinity(customerId, amount);
    }

    // ──────────────────────────────────────────────────────────────────────
    // 5. Save Category
    // ──────────────────────────────────────────────────────────────────────

    public void SaveGame()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.SaveGame();
    }

    public void LoadGame()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.LoadGame();
    }

    public void DeleteSave()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.DeleteSave();
    }

    // ──────────────────────────────────────────────────────────────────────
    // 6. Scene Category
    // ──────────────────────────────────────────────────────────────────────

    public void RequestSceneTransition(SceneTransitionRequest request)
    {
        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.RequestTransition(request);
    }
}
#endif
