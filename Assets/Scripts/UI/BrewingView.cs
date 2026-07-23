using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Brewing UI의 시각적 컴포넌트를 참조하고, 사용자 입력을 Action으로 외부에 전달하는 View 클래스입니다.
/// </summary>
public class BrewingView : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button btnSelectRecipe;
    [SerializeField] private Button btnStartBrew;

    [Header("정보 표시")]
    [SerializeField] private TMP_Text txtIngredients;
    [SerializeField] private TMP_Text txtStatus;

    [Header("진행도")]
    [SerializeField] private Slider progressSlider;

    // View가 발생하는 이벤트들
    public event Action OnSelectRecipeClicked;
    public event Action OnStartBrewClicked;

    private void Awake()
    {
        if (btnSelectRecipe != null)
            btnSelectRecipe.onClick.AddListener(() => OnSelectRecipeClicked?.Invoke());

        if (btnStartBrew != null)
            btnStartBrew.onClick.AddListener(() => OnStartBrewClicked?.Invoke());
    }

    /// <summary>
    /// 재료 정보 텍스트를 갱신합니다. (Controller가 포맷팅한 최종 문자열)
    /// </summary>
    public void SetIngredientsText(string text)
    {
        if (txtIngredients != null)
            txtIngredients.text = text;
    }

    /// <summary>
    /// 상태 텍스트를 갱신합니다. (Controller가 포맷팅한 최종 문자열)
    /// </summary>
    public void SetStatusText(string text)
    {
        if (txtStatus != null)
            txtStatus.text = text;
    }

    /// <summary>
    /// 제조 진행도 슬라이더 값을 갱신합니다. (0.0 ~ 1.0)
    /// </summary>
    public void SetProgressValue(float value)
    {
        if (progressSlider != null)
            progressSlider.value = Mathf.Clamp01(value);
    }

    /// <summary>
    /// 제조 시작 버튼의 활성화 여부를 설정합니다.
    /// </summary>
    public void SetStartButtonInteractable(bool isInteractable)
    {
        if (btnStartBrew != null)
            btnStartBrew.interactable = isInteractable;
    }
}
