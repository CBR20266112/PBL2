using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 씬 전환 연출(Fade In/Out), 로딩 UI 및 클릭 입력 차단을 관리하는 View 클래스입니다.
/// 계산 로직이 없으며 Controller가 넘겨준 정규화(0~1) 데이터만 단순히 대입/연출합니다.
/// </summary>
public class SceneTransitionView : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TMP_Text txtProgress;

    private Coroutine _fadeCoroutine;

    private void Awake()
    {
        ResetView();
    }

    /// <summary>
    /// View 상태를 완전히 초기화합니다.
    /// (Progress 초기화, Canvas Alpha 초기화, Loading UI 숨김, Input Block 해제)
    /// </summary>
    public void ResetView()
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = null;
        }

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
        }

        if (progressSlider != null)
        {
            progressSlider.value = 0f;
        }

        if (txtProgress != null)
        {
            txtProgress.text = "0%";
        }

        ShowLoadingUI(false);
    }

    /// <summary>
    /// 화면 클릭 입력 차단 온/오프를 설정합니다.
    /// </summary>
    public void SetInputBlock(bool block)
    {
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.blocksRaycasts = block;
        }
    }

    /// <summary>
    /// Controller가 0.0~1.0으로 정규화하여 전달한 로딩 진행률 데이터를 UI에 대입합니다.
    /// View는 직접 계산하지 않습니다.
    /// </summary>
    public void UpdateProgress(float normalizedProgress)
    {
        float clamped = Mathf.Clamp01(normalizedProgress);

        if (progressSlider != null)
        {
            progressSlider.value = clamped;
        }

        if (txtProgress != null)
        {
            txtProgress.text = $"{Mathf.RoundToInt(clamped * 100f)}%";
        }
    }

    /// <summary>
    /// 로딩 프로그래스 UI 패널 표시/숨김
    /// </summary>
    public void ShowLoadingUI(bool show)
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(show);
        }
    }

    /// <summary>
    /// Fade-In 연출 (화면 어두워짐/마스크 채워짐, 알파 0 -> 1)
    /// </summary>
    public void FadeIn(float duration, Action onComplete)
    {
        StartFade(0f, 1f, duration, onComplete);
    }

    /// <summary>
    /// Fade-Out 연출 (화면 밝아짐/마스크 투명해짐, 알파 1 -> 0)
    /// </summary>
    public void FadeOut(float duration, Action onComplete)
    {
        StartFade(1f, 0f, duration, onComplete);
    }

    private void StartFade(float startAlpha, float targetAlpha, float duration, Action onComplete)
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        _fadeCoroutine = StartCoroutine(FadeCoroutine(startAlpha, targetAlpha, duration, onComplete));
    }

    private IEnumerator FadeCoroutine(float startAlpha, float targetAlpha, float duration, Action onComplete)
    {
        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = startAlpha;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (fadeCanvasGroup != null)
                fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }

        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = targetAlpha;

        _fadeCoroutine = null;
        onComplete?.Invoke();
    }
}
