using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// confirmation 대화상자의 시각적 표현(제목, 내용, 버튼 텍스트), 표시/숨김 연출 및 버튼 클릭 이벤트 위임만 전담하는 View 클래스입니다.
/// Manager를 호출하지 않으며 ESC 입력 해석 또한 담당하지 않습니다 (Controller가 전담).
/// </summary>
public class DialogView : MonoBehaviour
{
    [SerializeField] private GameObject contentPanel;
    [SerializeField] private TMP_Text txtTitle;
    [SerializeField] private TMP_Text txtDescription;
    [SerializeField] private Button btnConfirm;
    [SerializeField] private Button btnCancel;
    [SerializeField] private TMP_Text txtConfirmBtn;
    [SerializeField] private TMP_Text txtCancelBtn;
    [SerializeField] private CanvasGroup canvasGroup;

    public event Action OnConfirmClicked;
    public event Action OnCancelClicked;

    private Coroutine _fadeCoroutine;

    private void Awake()
    {
        if (btnConfirm != null)
            btnConfirm.onClick.AddListener(() => OnConfirmClicked?.Invoke());

        if (btnCancel != null)
            btnCancel.onClick.AddListener(() => OnCancelClicked?.Invoke());
    }

    /// <summary>
    /// Controller가 포맷팅한 전용 DTO(DialogDisplayData)를 대입하고 Fade-In 연출을 수행합니다.
    /// </summary>
    public void ShowDialog(DialogDisplayData displayData)
    {
        if (displayData == null) return;

        if (txtTitle != null)
            txtTitle.text = displayData.titleText ?? string.Empty;

        if (txtDescription != null)
            txtDescription.text = displayData.descriptionText ?? string.Empty;

        if (txtConfirmBtn != null)
            txtConfirmBtn.text = displayData.confirmButtonText ?? "확인";

        if (txtCancelBtn != null)
            txtCancelBtn.text = displayData.cancelButtonText ?? "취소";

        if (btnCancel != null)
            btnCancel.gameObject.SetActive(displayData.showCancelButton);

        if (contentPanel != null)
            contentPanel.SetActive(true);
        else
            gameObject.SetActive(true);

        StartFade(0f, 1f, 0.2f, null);
    }

    /// <summary>
    /// Fade-Out 연출 후 대화상자를 숨깁니다.
    /// </summary>
    public void HideDialog(Action onComplete = null)
    {
        StartFade(1f, 0f, 0.15f, () =>
        {
            if (contentPanel != null)
                contentPanel.SetActive(false);
            else
                gameObject.SetActive(false);

            onComplete?.Invoke();
        });
    }

    private void StartFade(float startAlpha, float targetAlpha, float duration, Action onComplete)
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        _fadeCoroutine = StartCoroutine(FadeCoroutine(startAlpha, targetAlpha, duration, onComplete));
    }

    private IEnumerator FadeCoroutine(float startAlpha, float targetAlpha, float duration, Action onComplete)
    {
        Transform targetTransform = contentPanel != null ? contentPanel.transform : transform;
        Vector3 startScale = targetAlpha > startAlpha ? new Vector3(0.85f, 0.85f, 1f) : Vector3.one;
        Vector3 endScale = targetAlpha > startAlpha ? Vector3.one : new Vector3(0.85f, 0.85f, 1f);
        Vector3 popScale = new Vector3(1.05f, 1.05f, 1f);

        if (canvasGroup != null)
            canvasGroup.alpha = startAlpha;

        targetTransform.localScale = startScale;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            if (targetAlpha > startAlpha)
            {
                if (t < 0.7f)
                    targetTransform.localScale = Vector3.Lerp(startScale, popScale, t / 0.7f);
                else
                    targetTransform.localScale = Vector3.Lerp(popScale, endScale, (t - 0.7f) / 0.3f);
            }
            else
            {
                targetTransform.localScale = Vector3.Lerp(startScale, endScale, t);
            }

            yield return null;
        }

        if (canvasGroup != null)
            canvasGroup.alpha = targetAlpha;

        targetTransform.localScale = Vector3.one;

        _fadeCoroutine = null;
        onComplete?.Invoke();
    }
}
