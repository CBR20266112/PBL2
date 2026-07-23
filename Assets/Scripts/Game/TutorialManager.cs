using System;
using UnityEngine;

/// <summary>
/// 튜토리얼 세이브 컨테이너
/// </summary>
[System.Serializable]
public class TutorialSaveData
{
    public int currentStepIndex = 0;
    public bool isCompleted = false;
}

/// <summary>
/// 튜토리얼 진행 단계 및 완료 상태를 관리하는 독립 매니저 클래스입니다.
/// 다른 Manager를 직접 참조하지 않으며, GetSaveData/LoadSaveData만 제공합니다.
/// </summary>
public class TutorialManager : Singleton<TutorialManager>
{
    public int CurrentStepIndex { get; private set; } = 0;
    public bool IsCompleted { get; private set; } = false;
    public bool IsActive { get; private set; } = false;

    // 단계 변경 이벤트 (새 단계 index 전달)
    public delegate void TutorialStepChangedHandler(int stepIndex);
    public event TutorialStepChangedHandler OnTutorialStepChanged;

    // 튜토리얼 상태 변경 이벤트 (시작/종료)
    public event Action OnTutorialCompleted;
    public event Action OnTutorialStarted;

    /// <summary>
    /// 튜토리얼을 시작하거나 재시작합니다.
    /// </summary>
    public void StartTutorial()
    {
        CurrentStepIndex = 0;
        IsCompleted = false;
        IsActive = true;

        Debug.Log("[TutorialManager] 튜토리얼 시작");
        OnTutorialStarted?.Invoke();
        OnTutorialStepChanged?.Invoke(CurrentStepIndex);
    }

    /// <summary>
    /// 다음 튜토리얼 단계로 진행합니다.
    /// </summary>
    public void AdvanceStep()
    {
        if (!IsActive || IsCompleted) return;

        CurrentStepIndex++;
        Debug.Log($"[TutorialManager] 튜토리얼 단계 진행 -> {CurrentStepIndex}");
        OnTutorialStepChanged?.Invoke(CurrentStepIndex);
    }

    /// <summary>
    /// 특정 단계 index로 이동합니다.
    /// </summary>
    public void SetStepIndex(int index)
    {
        if (!IsActive || IsCompleted) return;

        CurrentStepIndex = Mathf.Max(0, index);
        OnTutorialStepChanged?.Invoke(CurrentStepIndex);
    }

    /// <summary>
    /// 튜토리얼을 완료 처리합니다.
    /// </summary>
    public void CompleteTutorial()
    {
        if (IsCompleted) return;

        IsCompleted = true;
        IsActive = false;
        Debug.Log("[TutorialManager] 튜토리얼 완료");
        OnTutorialCompleted?.Invoke();
    }

    /// <summary>
    /// 튜토리얼을 건너뜁니다.
    /// </summary>
    public void SkipTutorial()
    {
        Debug.Log("[TutorialManager] 튜토리얼 건너뛰기 수행");
        CompleteTutorial();
    }

    // ──────────────────────────────────────────────────────────────────────
    // SaveManager 연동 (GetSaveData / LoadSaveData)
    // ──────────────────────────────────────────────────────────────────────

    public TutorialSaveData GetSaveData()
    {
        return new TutorialSaveData
        {
            currentStepIndex = CurrentStepIndex,
            isCompleted = IsCompleted
        };
    }

    public void LoadSaveData(TutorialSaveData saveData)
    {
        if (saveData == null) return;

        CurrentStepIndex = saveData.currentStepIndex;
        IsCompleted = saveData.isCompleted;
        IsActive = !IsCompleted;

        Debug.Log($"[TutorialManager] 세이브 복원 (StepIndex: {CurrentStepIndex}, Completed: {IsCompleted})");
    }
}
