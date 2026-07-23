using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// FurnitureView에 전달되는 단일 가구 UI 데이터 구조체입니다.
/// UI 표시에 필요한 아이콘, 이름, 배치 여부만 포함합니다.
/// </summary>
public class FurnitureItemData
{
    public string furnitureId;
    public Sprite icon;
    public string localizedName;
    public bool isPlaced;
}

/// <summary>
/// 가구 UI 목록 표시, 현재 선택된 슬롯 정보 표시 및 배치/해제 액션 위임을 담당하는 View 클래스입니다.
/// 슬롯 선택 자체는 담당하지 않으며 전달받은 슬롯 정보만 표시합니다.
/// </summary>
public class FurnitureView : MonoBehaviour
{
    [SerializeField] private Transform itemContainer;
    [SerializeField] private FurnitureItemView itemPrefab;
    [SerializeField] private Button btnPlace;
    [SerializeField] private Button btnRemove;
    [SerializeField] private TMP_Text txtSelectedSlotInfo;

    // View가 발생하는 이벤트
    public event Action<string> OnFurnitureSelected;
    public event Action OnPlaceButtonClicked;
    public event Action OnRemoveButtonClicked;

    private readonly List<FurnitureItemView> _itemViews = new List<FurnitureItemView>();

    private void Awake()
    {
        if (btnPlace != null)
        {
            btnPlace.onClick.AddListener(() => OnPlaceButtonClicked?.Invoke());
        }

        if (btnRemove != null)
        {
            btnRemove.onClick.AddListener(() => OnRemoveButtonClicked?.Invoke());
        }
    }

    /// <summary>
    /// Controller가 구성한 데이터 목록으로 가구 아이템 목록을 전체 갱신합니다.
    /// 향후 Object Pool 교체 가능하도록 설계되었습니다.
    /// </summary>
    public void RefreshItems(List<FurnitureItemData> items)
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
            FurnitureItemView newView = Instantiate(itemPrefab, itemContainer);
            newView.Setup(item.furnitureId, item.icon, item.localizedName, item.isPlaced);
            newView.OnItemClicked += RelayItemClicked;
            _itemViews.Add(newView);
        }
    }

    /// <summary>
    /// 지정된 가구를 선택 상태로 표시하고 나머지는 해제합니다.
    /// </summary>
    public void SetItemSelected(string furnitureId)
    {
        foreach (var view in _itemViews)
        {
            if (view != null)
            {
                view.SetSelected(view.GetFurnitureId() == furnitureId);
            }
        }
    }

    /// <summary>
    /// 배치 및 배치 해제 버튼의 활성화(interactable) 상태를 설정합니다.
    /// </summary>
    public void SetActionButtonsState(bool canPlace, bool canRemove)
    {
        if (btnPlace != null)
        {
            btnPlace.interactable = canPlace;
        }

        if (btnRemove != null)
        {
            btnRemove.interactable = canRemove;
        }
    }

    /// <summary>
    /// 외부(Placement Controller 등)에서 전달된 현재 선택 슬롯 정보를 표시합니다.
    /// </summary>
    public void UpdateSlotInfoText(string formattedSlotInfo)
    {
        if (txtSelectedSlotInfo != null)
        {
            txtSelectedSlotInfo.text = formattedSlotInfo;
        }
    }

    private void RelayItemClicked(string furnitureId)
    {
        OnFurnitureSelected?.Invoke(furnitureId);
    }
}
