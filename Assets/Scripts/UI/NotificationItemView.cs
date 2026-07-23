using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 단일 Toast 알림의 시각적 요소(아이콘, 제목, 설명) 배치 및 Fade 애니메이션 연출을 담당하는 View 컴포넌트입니다.
/// 재사용 가능한 프리팹으로 설계되었습니다.
/// </summary>
public class NotificationItemView : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text txtTitle;
    [SerializeField] private TMP_Text txtDescription;
    [SerializeField] private CanvasGroup canvasGroup;

    private Coroutine _animateCoroutine;

    /// <summary>
    /// Controller/View에서 전달받은 최종 표시 데이터(DisplayData)를 대입합니다.
    /// </summary>
    public void Setup(NotificationDisplayData displayData)
    {
        if (displayData == null) return;

        if (iconImage != null)
        {
            if (displayData.icon != null)
            {
                iconImage.sprite = displayData.icon;
                iconImage.gameObject.SetActive(true);
            }
            else
            {
                iconImage.gameObject.SetActive(false);
            }
        }

        if (txtTitle != null)
            txtTitle.text = displayData.titleText ?? string.Empty;

        if (txtDescription != null)
            txtDescription.text = displayData.descriptionText ?? string.Empty;

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    /// <summary>
    /// 알림 표시 애니메이션(Fade In -> Stay -> Fade Out)을 실행합니다.
    /// </summary>
    public void AnimateShow(float duration, Action onComplete)
    {
        if (_animateCoroutine != null)
        {
            StopCoroutine(_animateCoroutine);
        }

        _animateCoroutine = StartCoroutine(AnimateShowCoroutine(duration, onComplete));
    }

    private IEnumerator AnimateShowCoroutine(float duration, Action onComplete)
    {
        float fadeDuration = 0.25f;
        Vector3 initialScale = new Vector3(0.8f, 0.8f, 1f);
        Vector3 targetScale = Vector3.one;
        Vector3 punchScale = new Vector3(1.08f, 1.08f, 1f);

        transform.localScale = initialScale;

        // 1. Fade In + Scale Pop
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            if (canvasGroup != null)
                canvasGroup.alpha = t;

            // 0 -> 1 지점에서 1.08f 스케일 펀치 후 1.0f 원복
            if (t < 0.7f)
                transform.localScale = Vector3.Lerp(initialScale, punchScale, t / 0.7f);
            else
                transform.localScale = Vector3.Lerp(punchScale, targetScale, (t - 0.7f) / 0.3f);

            yield return null;
        }

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
        transform.localScale = targetScale;

        // 2. Display Hold
        yield return new WaitForSeconds(Mathf.Max(0.5f, duration));

        // 3. Fade Out + Shrink
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            if (canvasGroup != null)
                canvasGroup.alpha = 1f - t;

            transform.localScale = Vector3.Lerp(targetScale, initialScale, t);
            yield return null;
        }

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
        transform.localScale = targetScale;

        _animateCoroutine = null;
        onComplete?.Invoke();
    }
}
