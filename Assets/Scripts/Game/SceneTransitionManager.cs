using System;
using UnityEngine;

/// <summary>
/// 씬 전환 요청 및 현재 전환 상태 관리를 전담하는 독립 싱글톤 매니저입니다.
/// 다른 Manager를 직접 참조하지 않으며, SetState 퍼블릭 API 대신 BeginTransition/CompleteTransition 명확한 API만 제공합니다.
/// </summary>
public class SceneTransitionManager : Singleton<SceneTransitionManager>
{
    /// <summary>
    /// 현재 씬 전환 상태
    /// </summary>
    public SceneTransitionState CurrentState { get; private set; } = SceneTransitionState.Idle;

    /// <summary>
    /// 현재 씬 전환이 진행 중인지 여부 (중복 호출 방지용)
    /// </summary>
    public bool IsTransitioning => CurrentState != SceneTransitionState.Idle;

    // 씬 전환 요청 이벤트 (Controller 수신용)
    public event Action<SceneTransitionRequest> OnTransitionRequested;

    // 전환 상태 변경 이벤트
    public event Action<SceneTransitionState> OnStateChanged;

    /// <summary>
    /// 씬 전환을 요청합니다. (RequestTransition DTO 구조)
    /// 이미 씬 전환 진행 중인 경우 요청을 거부하여 중복 전환을 방지합니다.
    /// </summary>
    public bool RequestTransition(SceneTransitionRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.sceneName))
        {
            Debug.LogWarning("[SceneTransitionManager] 유효하지 않은 씬 전환 요청입니다.");
            return false;
        }

        if (IsTransitioning)
        {
            Debug.LogWarning($"[SceneTransitionManager] 이미 씬 전환이 진행 중입니다. (요구 씬: {request.sceneName})");
            return false;
        }

        Debug.Log($"[SceneTransitionManager] 씬 전환 요청 접수: {request.sceneName}");
        OnTransitionRequested?.Invoke(request);
        return true;
    }

    /// <summary>
    /// Controller가 전환 시퀀스를 시작할 때 호출합니다. (상태 변경: FadingIn)
    /// </summary>
    public void BeginTransition()
    {
        SetStateInternal(SceneTransitionState.FadingIn);
    }

    /// <summary>
    /// Controller가 비동기 씬 로딩 단계 진입 시 호출합니다. (상태 변경: LoadingScene)
    /// </summary>
    public void SetLoadingState()
    {
        SetStateInternal(SceneTransitionState.LoadingScene);
    }

    /// <summary>
    /// Controller가 Fade-Out 단계 진입 시 호출합니다. (상태 변경: FadingOut)
    /// </summary>
    public void SetFadingOutState()
    {
        SetStateInternal(SceneTransitionState.FadingOut);
    }

    /// <summary>
    /// Controller가 전환 시퀀스를 완료했을 때 호출합니다. (상태 변경: Idle)
    /// </summary>
    public void CompleteTransition()
    {
        SetStateInternal(SceneTransitionState.Idle);
        Debug.Log("[SceneTransitionManager] 씬 전환 시퀀스 완료 (상태: Idle)");
    }

    private void SetStateInternal(SceneTransitionState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        OnStateChanged?.Invoke(CurrentState);
    }
}
