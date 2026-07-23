using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// InventoryManager와 IngredientDatabase에서 데이터를 조합하여 InventoryView에 전달하는 Controller 클래스입니다.
/// 선택 상태 관리 및 문자열 포맷팅(LocalizationManager 경유)을 담당합니다.
/// </summary>
public class InventoryController : MonoBehaviour
{
    [SerializeField] private InventoryView view;
    [SerializeField] private IngredientDatabase ingredientDatabase;

    private string _selectedIngredientId = string.Empty;

    private void Awake()
    {
        if (view == null)
        {
            Debug.LogError("[InventoryController] view(InventoryView) 참조가 할당되지 않았습니다. Inspector를 확인해 주세요.");
        }

        if (ingredientDatabase == null)
        {
            Debug.LogError("[InventoryController] ingredientDatabase 참조가 할당되지 않았습니다. Inspector를 확인해 주세요.");
        }
    }

    private void OnEnable()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged += HandleInventoryChanged;
        }

        if (view != null)
        {
            view.OnItemSelected += HandleItemSelected;
        }

        RefreshInventoryList();
    }

    private void OnDisable()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged -= HandleInventoryChanged;
        }

        if (view != null)
        {
            view.OnItemSelected -= HandleItemSelected;
        }
    }

    // ──────────────────────────────────────────────────────────────────────
    // 이벤트 핸들러
    // ──────────────────────────────────────────────────────────────────────

    private void HandleInventoryChanged(string ingredientId, int newQuantity)
    {
        // 재고 변경 시 전체 목록 갱신 (MVP: 향후 단일 아이템 갱신으로 최적화 가능)
        RefreshInventoryList();
    }

    private void HandleItemSelected(string ingredientId)
    {
        _selectedIngredientId = ingredientId;

        if (view != null)
        {
            view.SetItemSelected(_selectedIngredientId);
        }

        Debug.Log($"[InventoryController] 재료 선택: {ingredientId}");
    }

    // ──────────────────────────────────────────────────────────────────────
    // 데이터 조합 및 View 갱신
    // ──────────────────────────────────────────────────────────────────────

    private void RefreshInventoryList()
    {
        if (view == null || ingredientDatabase == null) return;

        IReadOnlyList<IngredientData> allIngredients = ingredientDatabase.GetAllIngredients();
        List<InventoryItemData> itemDataList = new List<InventoryItemData>();

        foreach (IngredientData ingredient in allIngredients)
        {
            if (ingredient == null) continue;

            int quantity = 0;
            if (InventoryManager.Instance != null)
            {
                quantity = InventoryManager.Instance.GetIngredientCount(ingredient.ingredientId);
            }

            string localizedName = GetLocalizedIngredientName(ingredient);

            itemDataList.Add(new InventoryItemData
            {
                ingredientId = ingredient.ingredientId,
                icon = ingredient.icon,
                localizedName = localizedName,
                formattedQuantity = $"{quantity}"
            });
        }

        view.RefreshItems(itemDataList);

        // 갱신 후 기존 선택 상태 복원
        if (!string.IsNullOrEmpty(_selectedIngredientId))
        {
            view.SetItemSelected(_selectedIngredientId);
        }
    }

    /// <summary>
    /// IngredientData의 localizationKey를 이용해 LocalizationManager로 이름을 조회합니다.
    /// localizationKey가 없거나 미등록 시 displayName을 Fallback으로 사용합니다.
    /// </summary>
    private string GetLocalizedIngredientName(IngredientData ingredient)
    {
        if (!string.IsNullOrEmpty(ingredient.localizationKey) && LocalizationManager.Instance != null)
        {
            string localized = LocalizationManager.Instance.GetText(ingredient.localizationKey);
            // GetText가 키 자체를 반환하는 경우(미등록)에는 displayName 사용
            if (localized != ingredient.localizationKey)
                return localized;
        }

        return string.IsNullOrEmpty(ingredient.displayName) ? ingredient.ingredientId : ingredient.displayName;
    }
}
