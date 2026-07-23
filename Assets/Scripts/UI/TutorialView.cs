using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 튜토리얼 안내 텍스트, 버튼 및 강조 마스크 UI의 시각적 표시만 전담하는 View 클래스입니다.
/// Manager 클래스를 직접 참조하지 않으며 이벤트를 외부에 위임합니다.
/// </summary>
public class TutorialView : MonoBehaviour
{
    [Header("안내 UI 컴포넌트")]
    [SerializeField] private GameObject contentPanel;
    [SerializeField] private TMP_Text txtTitle;
    [SerializeField] private TMP_Text txtDescription;
    [SerializeField] private Button btnNext;
    [SerializeField] private Button btnSkip;

    [Header("강조 UI 컴포넌트")]
    [SerializeField] private GameObject highlightMask;
    [SerializeField] private RectTransform highlightBox;

    public event Action OnNextClicked;
    public event Action OnSkipClicked;

    private void Awake()
    {
        if (btnNext != null)
            btnNext.onClick.AddListener(() => OnNextClicked?.Invoke());

        if (btnSkip != null)
            btnSkip.onClick.AddListener(() => OnSkipClicked?.Invoke());
    }

    /// <summary>
    /// Controller가 포맷팅/번역을 완료한 안내 텍스트 및 버튼 노출 여부를 설정합니다.
    /// </summary>
    public void SetStepContent(string title, string description, bool allowSkip, bool showNext)
    {
        if (txtTitle != null)
            txtTitle.text = title;

        if (txtDescription != null)
            txtDescription.text = description;

        // 배경 패널이 마우스 클릭 Raycast를 블로킹하지 않도록 처리
        if (contentPanel != null)
        {
            Image panelImg = contentPanel.GetComponent<Image>();
            if (panelImg != null) panelImg.raycastTarget = false;
        }

        if (btnSkip != null)
        {
            btnSkip.gameObject.SetActive(allowSkip);
            btnSkip.transform.SetAsLastSibling();
            Image img = btnSkip.GetComponent<Image>();
            if (img != null) img.raycastTarget = true;
        }

        if (btnNext != null)
        {
            btnNext.gameObject.SetActive(showNext);
            btnNext.transform.SetAsLastSibling();
            Image img = btnNext.GetComponent<Image>();
            if (img != null) img.raycastTarget = true;
        }
    }

    /// <summary>
    /// TutorialHighlightRegistry에 등록된 ID를 조회하여 해당 UI 항목을 강조 표시합니다.
    /// </summary>
    public void SetHighlightTargetId(string highlightTargetId)
    {
        if (string.IsNullOrEmpty(highlightTargetId))
        {
            ClearHighlight();
            return;
        }

        RectTransform targetRect = null;
        if (TutorialHighlightRegistry.Instance != null)
        {
            targetRect = TutorialHighlightRegistry.Instance.GetTarget(highlightTargetId);
        }

        if (targetRect != null)
        {
            ApplyHighlight(targetRect);
        }
        else
        {
            ClearHighlight();
        }
    }

    /// <summary>
    /// 지정된 RectTransform 위치와 크기로 강조 박스 및 마스크를 배치합니다.
    /// </summary>
    private void ApplyHighlight(RectTransform targetRect)
    {
        if (highlightMask != null)
            highlightMask.SetActive(true);

        if (highlightBox != null && targetRect != null)
        {
            highlightBox.gameObject.SetActive(true);
            highlightBox.position = targetRect.position;
            highlightBox.sizeDelta = targetRect.sizeDelta;
        }
    }

    /// <summary>
    /// 강조 표시를 해제합니다.
    /// </summary>
    public void ClearHighlight()
    {
        if (highlightMask != null)
            highlightMask.SetActive(false);

        if (highlightBox != null)
            highlightBox.gameObject.SetActive(false);
    }

    public void Show()
    {
        if (contentPanel != null)
            contentPanel.SetActive(true);
        else
            gameObject.SetActive(true);
    }

    public void Hide()
    {
        ClearHighlight();

        if (contentPanel != null)
            contentPanel.SetActive(false);
        else
            gameObject.SetActive(false);
    }
}
