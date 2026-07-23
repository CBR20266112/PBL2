using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 가구 UI의 단일 항목 시각적 표현 컴포넌트입니다.
/// 프리팹으로 사용되며 FurnitureView에 의해 생성/재사용됩니다.
/// </summary>
public class FurnitureItemView : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text txtName;
    [SerializeField] private GameObject placedBadge;
    [SerializeField] private GameObject selectedIndicator;
    [SerializeField] private Button btnSelect;

    public event Action<string> OnItemClicked;

    private string _furnitureId;

    private void Awake()
    {
        if (btnSelect != null)
        {
            btnSelect.onClick.AddListener(HandleClicked);
        }
    }

    /// <summary>
    /// Controller가 포맷팅한 데이터를 받아 UI를 세팅합니다.
    /// </summary>
    public void Setup(string furnitureId, Sprite icon, string localizedName, bool isPlaced)
    {
        _furnitureId = furnitureId;

        if (iconImage != null)
            iconImage.sprite = icon;

        if (txtName != null)
            txtName.text = localizedName;

        SetPlacedState(isPlaced);
        SetSelected(false);
    }

    /// <summary>
    /// 선택 상태 하이라이트를 표시/숨김 처리합니다.
    /// </summary>
    public void SetSelected(bool isSelected)
    {
        if (selectedIndicator != null)
        {
            selectedIndicator.SetActive(isSelected);
        }
    }

    /// <summary>
    /// 배치 중 표시 배지/텍스트를 표시/숨김 처리합니다.
    /// </summary>
    public void SetPlacedState(bool isPlaced)
    {
        if (placedBadge != null)
        {
            placedBadge.SetActive(isPlaced);
        }
    }

    /// <summary>
    /// 이 컴포넌트가 참조하는 가구 ID를 반환합니다.
    /// </summary>
    public string GetFurnitureId()
    {
        return _furnitureId;
    }

    private void HandleClicked()
    {
        OnItemClicked?.Invoke(_furnitureId);
    }
}
