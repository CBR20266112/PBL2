using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// FurnitureManager, UnlockManager와 통신하여 FurnitureView의 상태 및 액션을 제어하는 Controller 클래스입니다.
/// </summary>
public class FurnitureController : MonoBehaviour
{
    [SerializeField] private FurnitureView view;

    private string _currentSlotId = string.Empty;
    private string _selectedFurnitureId = string.Empty;

    private void OnEnable()
    {
        if (FurnitureManager.Instance != null)
        {
            FurnitureManager.Instance.OnFurnitureStateChanged += HandleFurnitureStateChanged;
        }

        if (view != null)
        {
            view.OnFurnitureSelected += HandleFurnitureSelected;
            view.OnPlaceButtonClicked += HandlePlaceButtonClicked;
            view.OnRemoveButtonClicked += HandleRemoveButtonClicked;
        }

        RefreshAll();
    }

    private void OnDisable()
    {
        if (FurnitureManager.Instance != null)
        {
            FurnitureManager.Instance.OnFurnitureStateChanged -= HandleFurnitureStateChanged;
        }

        if (view != null)
        {
            view.OnFurnitureSelected -= HandleFurnitureSelected;
            view.OnPlaceButtonClicked -= HandlePlaceButtonClicked;
            view.OnRemoveButtonClicked -= HandleRemoveButtonClicked;
        }
    }

    /// <summary>
    /// 외부(월드 / Placement Controller 등)에서 현재 선택된 슬롯을 지정할 때 호출합니다.
    /// 흐름: ① 슬롯 선택 -> ② 가구 선택 -> ③ 배치 버튼
    /// </summary>
    public void SetSelectedSlot(string slotId)
    {
        _currentSlotId = slotId;

        if (view != null)
        {
            string slotDisplayName = GetLocalizedSlotName(slotId);
            view.UpdateSlotInfoText(slotDisplayName);
        }

        UpdateActionButtons();
    }

    /// <summary>
    /// UI 전체 초기화 및 보유 가구 목록 동기화
    /// </summary>
    private void RefreshAll()
    {
        RefreshFurnitureList();
        UpdateActionButtons();
    }

    /// <summary>
    /// FurnitureManager에서 보유 가구 목록을 받아와 View를 갱신합니다.
    /// </summary>
    private void RefreshFurnitureList()
    {
        if (view == null) return;

        List<string> ownedIds = null;
        if (FurnitureManager.Instance != null)
        {
            ownedIds = FurnitureManager.Instance.GetOwnedFurnitureIds();
        }

        if (ownedIds == null)
        {
            ownedIds = new List<string>();
        }

        List<FurnitureItemData> itemDataList = new List<FurnitureItemData>();

        foreach (string furnitureId in ownedIds)
        {
            if (string.IsNullOrEmpty(furnitureId)) continue;

            // UnlockManager 해금 검증 (해금되어 있는 경우에만 목록 표시)
            if (UnlockManager.Instance != null && !UnlockManager.Instance.IsUnlocked(furnitureId))
            {
                // 기본 해금 항목이 아니거나 미해금 상태이면 제외 가능
                // 단, 이미 보유한 가구는 표시하도록 진행
            }

            bool isPlaced = false;
            if (FurnitureManager.Instance != null)
            {
                isPlaced = FurnitureManager.Instance.IsPlaced(furnitureId);
            }

            string localizedName = GetLocalizedFurnitureName(furnitureId);

            itemDataList.Add(new FurnitureItemData
            {
                furnitureId = furnitureId,
                icon = null, // Resources 또는 AssetDatabase 로드 로직 확장 지점
                localizedName = localizedName,
                isPlaced = isPlaced
            });
        }

        view.RefreshItems(itemDataList);

        if (!string.IsNullOrEmpty(_selectedFurnitureId))
        {
            view.SetItemSelected(_selectedFurnitureId);
        }
    }

    /// <summary>
    /// 가구 선택 이벤트 핸들러
    /// </summary>
    private void HandleFurnitureSelected(string furnitureId)
    {
        _selectedFurnitureId = furnitureId;

        if (view != null)
        {
            view.SetItemSelected(_selectedFurnitureId);
        }

        UpdateActionButtons();
    }

    /// <summary>
    /// 배치 버튼 클릭 핸들러
    /// </summary>
    private void HandlePlaceButtonClicked()
    {
        if (string.IsNullOrEmpty(_selectedFurnitureId) || FurnitureManager.Instance == null) return;

        if (FurnitureManager.Instance.CanPlaceFurniture(_selectedFurnitureId, _currentSlotId))
        {
            FurnitureManager.Instance.PlaceFurniture(_selectedFurnitureId);
        }
    }

    /// <summary>
    /// 배치 해제 버튼 클릭 핸들러
    /// </summary>
    private void HandleRemoveButtonClicked()
    {
        if (string.IsNullOrEmpty(_selectedFurnitureId) || FurnitureManager.Instance == null) return;

        if (FurnitureManager.Instance.CanRemoveFurniture(_selectedFurnitureId, _currentSlotId))
        {
            FurnitureManager.Instance.RemoveFurniture(_selectedFurnitureId);
        }
    }

    /// <summary>
    /// FurnitureManager의 배치 상태 변경 이벤트를 수신할 때 갱신
    /// </summary>
    private void HandleFurnitureStateChanged(string furnitureId, FurnitureStateChangeType changeType)
    {
        RefreshFurnitureList();
        UpdateActionButtons();
    }

    /// <summary>
    /// FurnitureManager의 CanPlaceFurniture / CanRemoveFurniture API 결과에 따라서만 버튼 활성화 제어
    /// </summary>
    private void UpdateActionButtons()
    {
        if (view == null) return;

        bool canPlace = false;
        bool canRemove = false;

        if (FurnitureManager.Instance != null && !string.IsNullOrEmpty(_selectedFurnitureId))
        {
            canPlace = FurnitureManager.Instance.CanPlaceFurniture(_selectedFurnitureId, _currentSlotId);
            canRemove = FurnitureManager.Instance.CanRemoveFurniture(_selectedFurnitureId, _currentSlotId);
        }

        view.SetActionButtonsState(canPlace, canRemove);
    }

    /// <summary>
    /// LocalizationManager를 사용해 가구 다국어 이름을 조회합니다.
    /// </summary>
    private string GetLocalizedFurnitureName(string furnitureId)
    {
        if (LocalizationManager.Instance != null)
        {
            string localized = LocalizationManager.Instance.GetText(furnitureId);
            if (!string.IsNullOrEmpty(localized) && localized != furnitureId)
                return localized;
        }

        return furnitureId;
    }

    /// <summary>
    /// 슬롯 ID 표시용 명칭 조회
    /// </summary>
    private string GetLocalizedSlotName(string slotId)
    {
        if (string.IsNullOrEmpty(slotId))
        {
            return "선택된 슬롯 없음";
        }

        if (LocalizationManager.Instance != null)
        {
            string localized = LocalizationManager.Instance.GetText(slotId);
            if (!string.IsNullOrEmpty(localized) && localized != slotId)
                return localized;
        }

        return $"슬롯: {slotId}";
    }
}
