using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 상점의 단일 판매 아이템 시각적 표현 컴포넌트입니다.
/// 프리팹으로 사용되며 ShopView에 의해 생성/재사용됩니다.
/// </summary>
public class ShopItemView : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text txtName;
    [SerializeField] private TMP_Text txtPrice;
    [SerializeField] private Button btnBuy;

    public event Action<string> OnBuyClicked;

    private string _ingredientId;

    private void Awake()
    {
        if (btnBuy != null)
        {
            btnBuy.onClick.AddListener(HandleBuyClicked);
        }
    }

    /// <summary>
    /// Controller가 포맷팅한 데이터를 받아 UI를 초기화합니다.
    /// </summary>
    public void Setup(string ingredientId, Sprite icon, string localizedName, string formattedPrice)
    {
        _ingredientId = ingredientId;

        if (iconImage != null)
            iconImage.sprite = icon;

        if (txtName != null)
            txtName.text = localizedName;

        if (txtPrice != null)
            txtPrice.text = formattedPrice;
    }

    /// <summary>
    /// 구매 가능 여부에 따라 버튼 활성화 상태를 제어합니다.
    /// </summary>
    public void SetBuyable(bool canBuy)
    {
        if (btnBuy != null)
        {
            btnBuy.interactable = canBuy;
        }
    }

    /// <summary>
    /// 이 아이템의 재료 ID를 반환합니다.
    /// </summary>
    public string GetIngredientId()
    {
        return _ingredientId;
    }

    private void HandleBuyClicked()
    {
        OnBuyClicked?.Invoke(_ingredientId);
    }
}
