using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 재료 재고(Inventory)를 관리하는 매니저 클래스입니다.
/// 런타임 재고 관리, 재료 추가/소모, 재고 확인 API를 제공합니다.
/// 저장/로드 API는 껍데기만 제공하며 향후 Save 시스템 구현 단계에서 연결합니다.
/// </summary>
public class InventoryManager : Singleton<InventoryManager>
{
    [Header("데이터베이스 연결")]
    [Tooltip("유효한 재료인지 확인하기 위해 IngredientDatabase 에셋을 연결합니다.")]
    [SerializeField] private IngredientDatabase _ingredientDatabase;

    // 런타임 재고 데이터 (ingredientId -> 수량)
    private readonly Dictionary<string, int> _inventory = new Dictionary<string, int>();

    // 재고 변경 이벤트 (UI 등에서 구독 가능)
    public delegate void InventoryChangedHandler(string ingredientId, int newQuantity);
    public event InventoryChangedHandler OnInventoryChanged;

    /// <summary>
    /// 특정 재료를 인벤토리에 추가합니다.
    /// </summary>
    /// <param name="ingredientId">재료 고유 ID</param>
    /// <param name="amount">추가할 수량 (양수여야 함)</param>
    public void AddIngredient(string ingredientId, int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning($"[InventoryManager] 0 이하의 수량({amount})은 추가할 수 없습니다.");
            return;
        }

        if (!IsValidIngredient(ingredientId))
        {
            Debug.LogWarning($"[InventoryManager] 존재하지 않는 재료 ID '{ingredientId}'입니다.");
            return;
        }

        if (_inventory.ContainsKey(ingredientId))
        {
            _inventory[ingredientId] += amount;
        }
        else
        {
            _inventory[ingredientId] = amount;
        }

        Debug.Log($"[InventoryManager] 재료 추가: {ingredientId} (+{amount}) -> 현재: {_inventory[ingredientId]}개");
        OnInventoryChanged?.Invoke(ingredientId, _inventory[ingredientId]);
    }

    /// <summary>
    /// 특정 재료를 인벤토리에서 소모합니다.
    /// </summary>
    /// <param name="ingredientId">재료 고유 ID</param>
    /// <param name="amount">소모할 수량 (양수여야 함)</param>
    /// <returns>소모 성공 여부 (재고 부족 시 false 반환 및 소모 미진행)</returns>
    public bool ConsumeIngredient(string ingredientId, int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning($"[InventoryManager] 0 이하의 수량({amount})은 소모할 수 없습니다.");
            return false;
        }

        if (!HasEnoughIngredient(ingredientId, amount))
        {
            Debug.LogWarning($"[InventoryManager] 재고가 부족합니다: {ingredientId} (요구: {amount}, 현재: {GetIngredientCount(ingredientId)})");
            return false;
        }

        _inventory[ingredientId] -= amount;
        Debug.Log($"[InventoryManager] 재료 소모: {ingredientId} (-{amount}) -> 현재: {_inventory[ingredientId]}개");
        OnInventoryChanged?.Invoke(ingredientId, _inventory[ingredientId]);
        return true;
    }

    /// <summary>
    /// 특정 재료의 현재 보유 수량을 가져옵니다.
    /// </summary>
    public int GetIngredientCount(string ingredientId)
    {
        if (_inventory.TryGetValue(ingredientId, out int quantity))
        {
            return quantity;
        }
        return 0;
    }

    /// <summary>
    /// 특정 재료가 필요한 수량만큼 충분히 존재하는지 확인합니다.
    /// </summary>
    public bool HasEnoughIngredient(string ingredientId, int amount)
    {
        if (amount <= 0) return true;
        return GetIngredientCount(ingredientId) >= amount;
    }

    /// <summary>
    /// 세이브 데이터 구조체에 담아 현재 재고 데이터를 반환합니다.
    /// InventoryManager는 데이터 저장의 책임을 직접 가지지 않고 데이터 제공 책임만 가집니다.
    /// </summary>
    public InventorySaveData GetSaveData()
    {
        InventorySaveData saveData = new InventorySaveData();
        foreach (var pair in _inventory)
        {
            saveData.items.Add(new InventoryItemSaveData
            {
                ingredientId = pair.Key,
                quantity = pair.Value
            });
        }
        return saveData;
    }

    /// <summary>
    /// 저장된 재고 데이터를 인계받아 런타임 인벤토리 상태를 복구합니다.
    /// </summary>
    public void LoadSaveData(InventorySaveData saveData)
    {
        if (saveData == null || saveData.items == null) return;

        _inventory.Clear();
        foreach (var item in saveData.items)
        {
            if (IsValidIngredient(item.ingredientId))
            {
                _inventory[item.ingredientId] = item.quantity;
            }
        }
        Debug.Log("[InventoryManager] 재고 데이터 복원 완료");
    }

    /// <summary>
    /// (껍데기 API) 기존 명세 유지를 위해 남겨둡니다. 실제 저장은 외부에 구현된 세이브 매니저가 일괄 처리합니다.
    /// </summary>
    public void SaveInventory()
    {
        Debug.Log("[InventoryManager] SaveInventory() 호출됨 (자체 저장하지 않으며, 외부 SaveManager에 의해 관리됩니다)");
    }

    /// <summary>
    /// (껍데기 API) 기존 명세 유지를 위해 남겨둡니다. 실제 로드는 외부에 구현된 세이브 매니저가 일괄 처리합니다.
    /// </summary>
    public void LoadInventory()
    {
        Debug.Log("[InventoryManager] LoadInventory() 호출됨 (자체 로드하지 않으며, 외부 SaveManager에 의해 관리됩니다)");
    }

    /// <summary>
    /// ID가 IngredientDatabase에 존재하는 유효한 재료인지 확인합니다.
    /// </summary>
    private bool IsValidIngredient(string ingredientId)
    {
        if (_ingredientDatabase == null)
        {
            // 데이터베이스가 연결되어 있지 않은 경우, 안전을 위해 모든 ID를 유효한 것으로 가정
            return true;
        }
        return _ingredientDatabase.GetIngredient(ingredientId) != null;
    }
}

// ──────────────────────────────────────────────────────────────────────
// 세이브/로드 연동을 위한 데이터 컨테이너 정의
// ──────────────────────────────────────────────────────────────────────

[System.Serializable]
public struct InventoryItemSaveData
{
    public string ingredientId;
    public int quantity;
}

[System.Serializable]
public class InventorySaveData
{
    public List<InventoryItemSaveData> items = new List<InventoryItemSaveData>();
}

