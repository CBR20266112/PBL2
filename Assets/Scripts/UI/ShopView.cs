using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// ShopView에서 사용하는 단일 아이템 데이터 구조체입니다.
/// </summary>
public class ShopItemData
{
    public string ingredientId;
    public Sprite icon;
    public string localizedName;
    public string formattedPrice;
    public bool canBuy;
}

/// <summary>
/// 상점 판매 목록 UI 및 소지금 표시를 관리하는 View 클래스입니다.
/// ShopItemView 인스턴스를 생성/관리하고, 구매 요청 이벤트를 릴레이합니다.
/// </summary>
public class ShopView : MonoBehaviour
{
    [SerializeField] private Transform itemContainer;
    [SerializeField] private ShopItemView itemPrefab;
    [SerializeField] private TMP_Text txtCurrentMoney;

    // ShopItemView의 구매 이벤트를 릴레이하는 이벤트 (ingredientId 전달)
    public event Action<string> OnItemBuyRequested;

    private readonly List<ShopItemView> _itemViews = new List<ShopItemView>();

    /// <summary>
    /// Controller가 구성한 데이터 목록으로 전체 아이템 목록을 갱신합니다.
    /// 향후 Object Pool로 내부 구현 교체 가능하도록 설계되었습니다.
    /// </summary>
    public void RefreshItems(List<ShopItemData> items)
    {
        // 기존 아이템 뷰 정리
        foreach (var view in _itemViews)
        {
            if (view != null)
            {
                view.OnBuyClicked -= RelayItemBuyClicked;
                Destroy(view.gameObject);
            }
        }
        _itemViews.Clear();

        if (items == null || itemPrefab == null || itemContainer == null) return;

        // 새 아이템 뷰 생성
        foreach (var item in items)
        {
            ShopItemView newView = Instantiate(itemPrefab, itemContainer);
            newView.Setup(item.ingredientId, item.icon, item.localizedName, item.formattedPrice);
            newView.SetBuyable(item.canBuy);
            newView.OnBuyClicked += RelayItemBuyClicked;
            _itemViews.Add(newView);
        }
    }

    /// <summary>
    /// 소지금 표시 텍스트를 갱신합니다. (Controller가 포맷팅한 최종 문자열)
    /// </summary>
    public void UpdateMoneyText(string formattedMoney)
    {
        if (txtCurrentMoney != null)
        {
            txtCurrentMoney.text = formattedMoney;
        }
    }

    /// <summary>
    /// 각 아이템의 구매 가능 여부 상태를 갱신합니다.
    /// </summary>
    public void UpdateBuyableStates(Dictionary<string, bool> buyableStates)
    {
        if (buyableStates == null) return;

        foreach (var view in _itemViews)
        {
            if (view != null && buyableStates.TryGetValue(view.GetIngredientId(), out bool canBuy))
            {
                view.SetBuyable(canBuy);
            }
        }
    }

    private void RelayItemBuyClicked(string ingredientId)
    {
        OnItemBuyRequested?.Invoke(ingredientId);
    }
}
