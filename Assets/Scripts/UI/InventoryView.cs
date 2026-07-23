using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인벤토리 전체 목록 UI를 관리하는 View 클래스입니다.
/// InventoryItemView 인스턴스를 생성/관리하고, 아이템 이벤트를 릴레이합니다.
/// 내부 구현(Instantiate)은 향후 Object Pool로 교체 가능하도록 RefreshItems 중심으로 설계합니다.
/// </summary>
public class InventoryView : MonoBehaviour
{
    [SerializeField] private Transform itemContainer;
    [SerializeField] private InventoryItemView itemPrefab;

    // ItemView 이벤트를 릴레이하는 이벤트 (ingredientId 전달)
    public event Action<string> OnItemSelected;

    // 현재 생성된 아이템 뷰 목록
    private readonly List<InventoryItemView> _itemViews = new List<InventoryItemView>();

    /// <summary>
    /// Controller가 구성한 데이터 목록으로 전체 목록을 갱신합니다.
    /// 향후 Object Pool로 내부 구현 교체 가능.
    /// </summary>
    public void RefreshItems(List<InventoryItemData> items)
    {
        // 기존 아이템 뷰 정리
        foreach (var view in _itemViews)
        {
            if (view != null)
            {
                view.OnItemClicked -= RelayItemClicked;
                Destroy(view.gameObject);
            }
        }
        _itemViews.Clear();

        if (items == null || itemPrefab == null || itemContainer == null) return;

        // 새 아이템 뷰 생성
        foreach (var item in items)
        {
            InventoryItemView newView = Instantiate(itemPrefab, itemContainer);
            newView.Setup(item.ingredientId, item.icon, item.localizedName, item.formattedQuantity);
            newView.OnItemClicked += RelayItemClicked;
            _itemViews.Add(newView);
        }
    }

    /// <summary>
    /// 지정된 ingredientId의 아이템을 선택 상태로 표시하고 나머지는 해제합니다.
    /// </summary>
    public void SetItemSelected(string ingredientId)
    {
        foreach (var view in _itemViews)
        {
            if (view != null)
                view.SetSelected(view.GetIngredientId() == ingredientId);
        }
    }

    private void RelayItemClicked(string ingredientId)
    {
        OnItemSelected?.Invoke(ingredientId);
    }
}

/// <summary>
/// InventoryView.RefreshItems에 전달되는 단일 아이템 데이터 구조체입니다.
/// Controller가 포맷팅을 완료한 최종 표시 데이터입니다.
/// </summary>
public class InventoryItemData
{
    public string ingredientId;
    public UnityEngine.Sprite icon;
    public string localizedName;
    public string formattedQuantity;
}
