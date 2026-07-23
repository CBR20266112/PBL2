using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// SceneTransitionManager 요청을 수신하여 비동기 씬 로딩(AsyncOperation), Fade In/Out 및 로딩 진행률 정규화(0~1)를 조율하는 Controller 클래스입니다.
/// 매 프레임 Update()는 사용하지 않으며 코루틴 비동기 루프를 활용합니다.
/// </summary>
public class SceneTransitionController : MonoBehaviour
{
    [SerializeField] private SceneTransitionView view;
    [SerializeField] private float minTransitionDuration = 0.5f; // 최소 전환 보장 시간 (기본 0.5초)

    private Coroutine _transitionCoroutine;

    private void OnEnable()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.OnTransitionRequested += HandleTransitionRequested;
        }

        if (view != null)
        {
            view.ResetView();
        }
    }

    private void OnDisable()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.OnTransitionRequested -= HandleTransitionRequested;
        }

        if (_transitionCoroutine != null)
        {
            StopCoroutine(_transitionCoroutine);
            _transitionCoroutine = null;
        }
    }

    private void HandleTransitionRequested(SceneTransitionRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.sceneName)) return;

        if (_transitionCoroutine != null)
        {
            StopCoroutine(_transitionCoroutine);
        }

        _transitionCoroutine = StartCoroutine(TransitionSequenceCoroutine(request));
    }

    /// <summary>
    /// 씬 전환 시퀀스 코루틴 (Fade In -> 비동기 로딩 -> 0~1 정규화 갱신 -> 최소 보장 시간 대기 -> Fade Out)
    /// </summary>
    private IEnumerator TransitionSequenceCoroutine(SceneTransitionRequest request)
    {
        if (SceneTransitionManager.Instance == null || view == null) yield break;

        // 1. 전환 시작
        SceneTransitionManager.Instance.BeginTransition();
        view.SetInputBlock(true);

        // 2. Fade-In 진행
        bool isFadeInComplete = false;
        float fadeDuration = Mathf.Max(0.1f, request.fadeDuration);
        view.FadeIn(fadeDuration, () => isFadeInComplete = true);

        while (!isFadeInComplete)
        {
            yield return null;
        }

        // 3. 로딩 UI 표시 및 로딩 상태 전환
        if (request.showLoadingUI)
        {
            view.ShowLoadingUI(true);
            view.UpdateProgress(0f);
        }
        SceneTransitionManager.Instance.SetLoadingState();

        // 최소 전환 보장 타이머 기록
        float startTime = Time.time;

        // 4. 비동기 씬 로딩 시작
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(request.sceneName);
        if (asyncOp != null)
        {
            asyncOp.allowSceneActivation = false;

            while (!asyncOp.isDone)
            {
                // AsyncOperation.progress는 0.0 ~ 0.9 범위입니다.
                // 0.0 ~ 1.0으로 정규화하여 View에 전달합니다. (수정 지침 3번)
                float normalizedProgress = Mathf.Clamp01(asyncOp.progress / 0.9f);
                if (request.showLoadingUI)
                {
                    view.UpdateProgress(normalizedProgress);
                }

                // 0.9 도달 시 로딩 완료 준비
                if (asyncOp.progress >= 0.9f)
                {
                    if (request.showLoadingUI)
                    {
                        view.UpdateProgress(1f);
                    }

                    // 5. 최소 전환 보장 시간(기본 0.5초) 충족 대기 (수정 지침 4번)
                    float elapsedTime = Time.time - startTime;
                    if (elapsedTime < minTransitionDuration)
                    {
                        yield return new WaitForSeconds(minTransitionDuration - elapsedTime);
                    }

                    // 씬 활성화
                    asyncOp.allowSceneActivation = true;
                }

                yield return null;
            }
        }

        // 6. Fade-Out 진행 및 마무리
        SceneTransitionManager.Instance.SetFadingOutState();
        if (request.showLoadingUI)
        {
            view.ShowLoadingUI(false);
        }

        bool isFadeOutComplete = false;
        view.FadeOut(fadeDuration, () => isFadeOutComplete = true);

        while (!isFadeOutComplete)
        {
            yield return null;
        }

        // 7. View 상태 완전 초기화 및 전환 완료
        view.ResetView();
        SceneTransitionManager.Instance.CompleteTransition();

        _transitionCoroutine = null;
    }
}
