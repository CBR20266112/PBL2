using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 인벤토리의 단일 재료 아이템 시각적 표현 컴포넌트입니다.
/// 프리팹으로 사용되며 InventoryView에 의해 생성/재사용됩니다.
/// </summary>
public class InventoryItemView : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text txtName;
    [SerializeField] private TMP_Text txtQuantity;
    [SerializeField] private Button btnSelect;
    [SerializeField] private GameObject selectedIndicator;

    public event Action<string> OnItemClicked;

    private string _ingredientId;

    private void Awake()
    {
        if (btnSelect != null)
            btnSelect.onClick.AddListener(HandleClicked);
    }

    /// <summary>
    /// Controller가 포맷팅한 문자열과 아이콘을 받아 UI를 초기화합니다.
    /// </summary>
    public void Setup(string ingredientId, Sprite icon, string localizedName, string formattedQuantity)
    {
        _ingredientId = ingredientId;

        if (iconImage != null)
            iconImage.sprite = icon;

        if (txtName != null)
            txtName.text = localizedName;

        if (txtQuantity != null)
            txtQuantity.text = formattedQuantity;

        SetSelected(false);
    }

    /// <summary>
    /// 선택 상태를 시각적으로 표시합니다.
    /// </summary>
    public void SetSelected(bool isSelected)
    {
        if (selectedIndicator != null)
            selectedIndicator.SetActive(isSelected);
    }

    /// <summary>
    /// 이 아이템이 나타내는 재료 ID를 반환합니다.
    /// </summary>
    public string GetIngredientId()
    {
        return _ingredientId;
    }

    private void HandleClicked()
    {
        OnItemClicked?.Invoke(_ingredientId);
    }
}
