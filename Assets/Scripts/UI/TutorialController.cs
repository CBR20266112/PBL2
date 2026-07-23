using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 외부 매니저(DayManager, BrewingManager, ServingManager, ShopManager)의 이벤트를 수신하여
/// TutorialConditionType으로 변환 후 TutorialManager와 TutorialView를 연결하는 Controller 클래스입니다.
/// </summary>
public class TutorialController : MonoBehaviour
{
    [SerializeField] private TutorialView view;
    [SerializeField] private List<TutorialStepData> tutorialSteps = new List<TutorialStepData>();

    private void OnEnable()
    {
        // 1. 튜토리얼 매니저 이벤트 구독
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnTutorialStepChanged += HandleStepChanged;
            TutorialManager.Instance.OnTutorialCompleted += HandleTutorialCompleted;
            TutorialManager.Instance.OnTutorialStarted += HandleTutorialStarted;
        }

        // 2. View 이벤트 구독
        if (view != null)
        {
            view.OnNextClicked += HandleNextClicked;
            view.OnSkipClicked += HandleSkipClicked;
        }

        // 3. 외부 매니저 이벤트 구독 (이벤트를 TutorialConditionType으로 변환하여 전달)
        SubscribeExternalManagerEvents();

        // 4. 초기 상태 동기화
        SyncCurrentState();
    }

    private void OnDisable()
    {
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnTutorialStepChanged -= HandleStepChanged;
            TutorialManager.Instance.OnTutorialCompleted -= HandleTutorialCompleted;
            TutorialManager.Instance.OnTutorialStarted -= HandleTutorialStarted;
        }

        if (view != null)
        {
            view.OnNextClicked -= HandleNextClicked;
            view.OnSkipClicked -= HandleSkipClicked;
        }

        UnsubscribeExternalManagerEvents();
    }

    /// <summary>
    /// 외부 매니저의 이벤트를 구독합니다.
    /// </summary>
    private void SubscribeExternalManagerEvents()
    {
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.OnPurchaseSuccess += HandleShopPurchaseSuccess;
        }

        if (BrewingManager.Instance != null)
        {
            BrewingManager.Instance.OnBrewingComplete += HandleBrewingComplete;
        }

        if (ServingManager.Instance != null)
        {
            ServingManager.Instance.OnServeSucceeded += HandleServeSucceeded;
        }

        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnDayStateChanged += HandleDayStateChanged;
        }
    }

    /// <summary>
    /// 외부 매니저의 이벤트 구독을 해제합니다.
    /// </summary>
    private void UnsubscribeExternalManagerEvents()
    {
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.OnPurchaseSuccess -= HandleShopPurchaseSuccess;
        }

        if (BrewingManager.Instance != null)
        {
            BrewingManager.Instance.OnBrewingComplete -= HandleBrewingComplete;
        }

        if (ServingManager.Instance != null)
        {
            ServingManager.Instance.OnServeSucceeded -= HandleServeSucceeded;
        }

        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnDayStateChanged -= HandleDayStateChanged;
        }
    }

    // ──────────────────────────────────────────────────────────────────────
    // 외부 매니저 이벤트 수신 및 TutorialConditionType 변환
    // ──────────────────────────────────────────────────────────────────────

    private void HandleShopPurchaseSuccess(string ingredientId, int amount, int totalCost)
    {
        NotifyConditionMet(TutorialConditionType.ShopPurchaseSuccess);
    }

    private void HandleBrewingComplete()
    {
        NotifyConditionMet(TutorialConditionType.BrewingComplete);
    }

    private void HandleServeSucceeded(string customerId, string drinkId, int earnedMoney)
    {
        NotifyConditionMet(TutorialConditionType.ServeSuccess);
    }

    private void HandleDayStateChanged(DayState newState)
    {
        if (newState == DayState.Operating)
        {
            NotifyConditionMet(TutorialConditionType.DayStateOperating);
        }
        else if (newState == DayState.Settlement)
        {
            NotifyConditionMet(TutorialConditionType.DayStateSettlement);
        }
    }

    /// <summary>
    /// 조건이 달성되었음을 판별하고, 해당 조건과 일치하는 단계일 때 자동 진행(autoAdvanceOnCondition)을 처리합니다.
    /// </summary>
    private void NotifyConditionMet(TutorialConditionType metCondition)
    {
        if (TutorialManager.Instance == null || !TutorialManager.Instance.IsActive) return;

        TutorialStepData currentStep = GetCurrentStepData(TutorialManager.Instance.CurrentStepIndex);
        if (currentStep == null) return;

        if (currentStep.conditionType == metCondition)
        {
            Debug.Log($"[TutorialController] 조건 달성 확인 ({metCondition})");

            if (currentStep.autoAdvanceOnCondition)
            {
                TutorialManager.Instance.AdvanceStep();
            }
        }
    }

    // ──────────────────────────────────────────────────────────────────────
    // TutorialManager 및 View 핸들러
    // ──────────────────────────────────────────────────────────────────────

    private void HandleStepChanged(int stepIndex)
    {
        if (TutorialManager.Instance != null && TutorialManager.Instance.IsCompleted)
        {
            if (view != null) view.Hide();
            return;
        }

        TutorialStepData stepData = GetCurrentStepData(stepIndex);

        if (stepData == null)
        {
            // 모든 단계를 종료하였으면 완료 처리
            if (TutorialManager.Instance != null)
            {
                TutorialManager.Instance.CompleteTutorial();
            }
            return;
        }

        UpdateViewForStep(stepData);
    }

    private void HandleTutorialStarted()
    {
        SyncCurrentState();
    }

    private void HandleTutorialCompleted()
    {
        if (view != null)
        {
            view.Hide();
        }
    }

    private void HandleNextClicked()
    {
        Debug.Log("Next Click");
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.AdvanceStep();
        }
    }

    private void HandleSkipClicked()
    {
        Debug.Log("Skip Click");
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.SkipTutorial();
        }
    }

    private void SyncCurrentState()
    {
        if (TutorialManager.Instance == null || view == null) return;

        if (TutorialManager.Instance.IsCompleted || !TutorialManager.Instance.IsActive)
        {
            view.Hide();
            return;
        }

        HandleStepChanged(TutorialManager.Instance.CurrentStepIndex);
    }

    /// <summary>
    /// 현재 단계 데이터로 View를 갱신합니다. (LocalizationManager를 통해 다국어 문자열 세팅)
    /// </summary>
    private void UpdateViewForStep(TutorialStepData stepData)
    {
        if (view == null || stepData == null) return;

        string title = GetLocalizedText(stepData.titleKey);
        string description = GetLocalizedText(stepData.descriptionKey);

        // 유저가 수동으로 언제든지 다음 및 건너뛰기를 누를 수 있도록 클릭 가능 보장
        bool showNext = true;
        bool allowSkip = true;

        view.Show();
        view.SetStepContent(title, description, allowSkip, showNext);
        view.SetHighlightTargetId(stepData.highlightTargetId);
    }

    private TutorialStepData GetCurrentStepData(int index)
    {
        if (tutorialSteps != null && index >= 0 && index < tutorialSteps.Count)
        {
            return tutorialSteps[index];
        }
        return null;
    }

    private string GetLocalizedText(string key)
    {
        if (string.IsNullOrEmpty(key)) return string.Empty;

        if (LocalizationManager.Instance != null)
        {
            string text = LocalizationManager.Instance.GetText(key);
            if (!string.IsNullOrEmpty(text)) return text;
        }

        return key;
    }
}
