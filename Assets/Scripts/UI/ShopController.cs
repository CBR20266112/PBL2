using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ShopManager, MoneyManager, IngredientDatabase와 통신하여 ShopView를 제어하는 Controller 클래스입니다.
/// </summary>
public class ShopController : MonoBehaviour
{
    [SerializeField] private ShopView view;
    [SerializeField] private IngredientDatabase ingredientDatabase;

    private void Awake()
    {
        if (view == null)
        {
            Debug.LogError("[ShopController] view(ShopView) 참조가 할당되지 않았습니다. Inspector를 확인해 주세요.");
        }

        if (ingredientDatabase == null)
        {
            Debug.LogError("[ShopController] ingredientDatabase 참조가 할당되지 않았습니다. Inspector를 확인해 주세요.");
        }
    }

    private void OnEnable()
    {
        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.OnMoneyChanged += HandleMoneyChanged;
        }

        if (ShopViewHasInstance())
        {
            view.OnItemBuyRequested += HandleItemBuyRequested;
        }

        RefreshAll();
    }

    private void OnDisable()
    {
        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.OnMoneyChanged -= HandleMoneyChanged;
        }

        if (ShopViewHasInstance())
        {
            view.OnItemBuyRequested -= HandleItemBuyRequested;
        }
    }

    /// <summary>
    /// 상점 UI 전체 초기화 및 목록 동기화를 수행합니다.
    /// </summary>
    private void RefreshAll()
    {
        if (view == null) return;

        UpdateMoneyDisplay();
        RefreshShopList();
    }

    /// <summary>
    /// 상점 판매 목록을 새로 가져와 View에 전달합니다.
    /// </summary>
    private void RefreshShopList()
    {
        List<IngredientData> availableIngredients = null;

        if (ShopManager.Instance != null)
        {
            availableIngredients = ShopManager.Instance.GetAvailableIngredients();
        }
        else if (ingredientDatabase != null)
        {
            availableIngredients = new List<IngredientData>(ingredientDatabase.GetAllIngredients());
        }

        if (availableIngredients == null) return;

        List<ShopItemData> itemDataList = new List<ShopItemData>();

        foreach (IngredientData ingredient in availableIngredients)
        {
            if (ingredient == null) continue;

            string localizedName = GetLocalizedIngredientName(ingredient);
            string formattedPrice = $"{ingredient.buyPrice}냥";

            bool canBuy = false;
            if (ShopManager.Instance != null)
            {
                canBuy = ShopManager.Instance.CanPurchase(ingredient.ingredientId, 1);
            }

            itemDataList.Add(new ShopItemData
            {
                ingredientId = ingredient.ingredientId,
                icon = ingredient.icon,
                localizedName = localizedName,
                formattedPrice = formattedPrice,
                canBuy = canBuy
            });
        }

        view.RefreshItems(itemDataList);
    }

    /// <summary>
    /// 소지금 변경 시 소지금 텍스트와 각 아이템의 구매 가능 상태만 갱신합니다. (목록 전체 재생성 하지 않음)
    /// </summary>
    private void HandleMoneyChanged(int newMoney, int delta)
    {
        UpdateMoneyDisplay();
        UpdateBuyableStatesOnly();
    }

    /// <summary>
    /// 현재 소지금 표시 텍스트를 View에 전달합니다.
    /// </summary>
    private void UpdateMoneyDisplay()
    {
        if (view == null) return;

        int currentMoney = MoneyManager.Instance != null ? MoneyManager.Instance.CurrentMoney : 0;
        view.UpdateMoneyText($"{currentMoney}냥");
    }

    /// <summary>
    /// 각 아이템의 구매 가능 상태만 계산하여 View에 전달합니다.
    /// </summary>
    private void UpdateBuyableStatesOnly()
    {
        if (view == null || ShopManager.Instance == null) return;

        List<IngredientData> availableIngredients = ShopManager.Instance.GetAvailableIngredients();
        if (availableIngredients == null) return;

        Dictionary<string, bool> buyableStates = new Dictionary<string, bool>();

        foreach (IngredientData ingredient in availableIngredients)
        {
            if (ingredient == null) continue;
            bool canBuy = ShopManager.Instance.CanPurchase(ingredient.ingredientId, 1);
            buyableStates[ingredient.ingredientId] = canBuy;
        }

        view.UpdateBuyableStates(buyableStates);
    }

    /// <summary>
    /// 구매 버튼 클릭 시 구매 진행
    /// </summary>
    private void HandleItemBuyRequested(string ingredientId)
    {
        if (ShopManager.Instance == null) return;

        bool success = ShopManager.Instance.BuyIngredient(ingredientId, 1);

        if (success)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySfx("ui_purchase");

            if (NotificationManager.Instance != null)
                NotificationManager.Instance.Enqueue(new NotificationData(NotificationType.PurchaseSuccess, "재료 구매 완료", "재료를 성공적으로 구매했습니다."));
        }
        else
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySfx("ui_error");

            if (NotificationManager.Instance != null)
                NotificationManager.Instance.Enqueue(new NotificationData(NotificationType.PurchaseFailed, "구매 실패", "냥전이 부족합니다."));
        }
    }

    /// <summary>
    /// IngredientData의 localizationKey를 이용해 LocalizationManager로 이름을 조회합니다.
    /// </summary>
    private string GetLocalizedIngredientName(IngredientData ingredient)
    {
        if (!string.IsNullOrEmpty(ingredient.localizationKey) && LocalizationManager.Instance != null)
        {
            string localized = LocalizationManager.Instance.GetText(ingredient.localizationKey);
            if (localized != ingredient.localizationKey)
                return localized;
        }

        return string.IsNullOrEmpty(ingredient.displayName) ? ingredient.ingredientId : ingredient.displayName;
    }

    private bool ShopViewHasInstance()
    {
        return view != null;
    }
}
