using System.Collections;
using UnityEngine;

/// <summary>
/// 각 Manager의 이벤트를 구독하여 NotificationData로 변환 후 NotificationManager에 Enqueue하며,
/// 알림 출력 루프(Queue 소비 및 표시 처리)를 코루틴 기반으로 총괄하는 Controller 클래스입니다.
/// </summary>
public class NotificationController : MonoBehaviour
{
    [SerializeField] private NotificationView view;

    private Coroutine _queueProcessCoroutine;
    private bool _isDisplaying = false;

    private void OnEnable()
    {
        // 1. NotificationManager 이벤트 구독
        if (NotificationManager.Instance != null)
        {
            NotificationManager.Instance.OnNotificationEnqueued += HandleNotificationEnqueued;
        }

        // 2. 각 매니저 이벤트 구독
        SubscribeManagerEvents();

        // 3. 기존 대기 큐 확인 및 출력 루프 가동
        StartQueueProcessing();
    }

    private void OnDisable()
    {
        if (NotificationManager.Instance != null)
        {
            NotificationManager.Instance.OnNotificationEnqueued -= HandleNotificationEnqueued;
        }

        UnsubscribeManagerEvents();

        if (_queueProcessCoroutine != null)
        {
            StopCoroutine(_queueProcessCoroutine);
            _queueProcessCoroutine = null;
        }
    }

    /// <summary>
    /// 시스템 매니저 이벤트 구독
    /// </summary>
    private void SubscribeManagerEvents()
    {
        if (MoneyManager.Instance != null)
            MoneyManager.Instance.OnMoneyChanged += HandleMoneyChanged;

        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.OnPurchaseSuccess += HandlePurchaseSuccess;
            ShopManager.Instance.OnPurchaseFailed += HandlePurchaseFailed;
        }

        if (UnlockManager.Instance != null)
            UnlockManager.Instance.OnUnlockStateChanged += HandleUnlockStateChanged;

        if (FurnitureManager.Instance != null)
            FurnitureManager.Instance.OnFurnitureStateChanged += HandleFurnitureStateChanged;

        if (TutorialManager.Instance != null)
            TutorialManager.Instance.OnTutorialCompleted += HandleTutorialCompleted;

        if (SaveManager.Instance != null)
            SaveManager.Instance.OnGameSaved += HandleGameSaved;
    }

    /// <summary>
    /// 시스템 매니저 이벤트 구독 해제
    /// </summary>
    private void UnsubscribeManagerEvents()
    {
        if (MoneyManager.Instance != null)
            MoneyManager.Instance.OnMoneyChanged -= HandleMoneyChanged;

        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.OnPurchaseSuccess -= HandlePurchaseSuccess;
            ShopManager.Instance.OnPurchaseFailed -= HandlePurchaseFailed;
        }

        if (UnlockManager.Instance != null)
            UnlockManager.Instance.OnUnlockStateChanged -= HandleUnlockStateChanged;

        if (FurnitureManager.Instance != null)
            FurnitureManager.Instance.OnFurnitureStateChanged -= HandleFurnitureStateChanged;

        if (TutorialManager.Instance != null)
            TutorialManager.Instance.OnTutorialCompleted -= HandleTutorialCompleted;

        if (SaveManager.Instance != null)
            SaveManager.Instance.OnGameSaved -= HandleGameSaved;
    }

    // ──────────────────────────────────────────────────────────────────────
    // 매니저 이벤트를 수신하여 NotificationData로 변환 후 Enqueue
    // ──────────────────────────────────────────────────────────────────────

    private void HandleMoneyChanged(int newMoney, int delta)
    {
        if (delta > 0)
        {
            EnqueueNotification(NotificationType.MoneyEarned, "notif_title_money_earned", "notif_desc_money_earned");
        }
        else if (delta < 0)
        {
            EnqueueNotification(NotificationType.MoneySpent, "notif_title_money_spent", "notif_desc_money_spent");
        }
    }

    private void HandlePurchaseSuccess(string ingredientId, int amount, int totalCost)
    {
        EnqueueNotification(NotificationType.PurchaseSuccess, "notif_title_purchase_success", "notif_desc_purchase_success");
    }

    private void HandlePurchaseFailed(string ingredientId, string reason)
    {
        EnqueueNotification(NotificationType.PurchaseFailed, "notif_title_purchase_failed", "notif_desc_purchase_failed");
    }

    private void HandleUnlockStateChanged(string id, bool isUnlocked)
    {
        if (isUnlocked)
        {
            EnqueueNotification(NotificationType.RecipeUnlocked, "notif_title_recipe_unlocked", "notif_desc_recipe_unlocked");
        }
    }

    private void HandleFurnitureStateChanged(string furnitureId, FurnitureStateChangeType changeType)
    {
        if (changeType == FurnitureStateChangeType.Placed)
        {
            EnqueueNotification(NotificationType.FurniturePlaced, "notif_title_furniture_placed", "notif_desc_furniture_placed");
        }
        else if (changeType == FurnitureStateChangeType.Owned)
        {
            EnqueueNotification(NotificationType.FurnitureUnlocked, "notif_title_furniture_unlocked", "notif_desc_furniture_unlocked");
        }
    }

    private void HandleTutorialCompleted()
    {
        EnqueueNotification(NotificationType.TutorialCompleted, "notif_title_tutorial_completed", "notif_desc_tutorial_completed");
    }

    private void HandleGameSaved()
    {
        EnqueueNotification(NotificationType.GameSaved, "notif_title_game_saved", "notif_desc_game_saved");
    }

    private void EnqueueNotification(NotificationType type, string titleKey, string descKey, Sprite icon = null, float duration = 2.5f)
    {
        if (NotificationManager.Instance != null)
        {
            NotificationData data = new NotificationData(type, titleKey, descKey, icon, duration);
            NotificationManager.Instance.Enqueue(data);
        }
    }

    // ──────────────────────────────────────────────────────────────────────
    // Notification 출력 루프 (Controller가 출력을 전담)
    // ──────────────────────────────────────────────────────────────────────

    private void HandleNotificationEnqueued()
    {
        StartQueueProcessing();
    }

    private void StartQueueProcessing()
    {
        if (_queueProcessCoroutine == null)
        {
            _queueProcessCoroutine = StartCoroutine(ProcessQueueCoroutine());
        }
    }

    private IEnumerator ProcessQueueCoroutine()
    {
        while (NotificationManager.Instance != null && NotificationManager.Instance.HasPendingNotifications)
        {
            if (!_isDisplaying && NotificationManager.Instance.TryDequeue(out NotificationData data))
            {
                _isDisplaying = true;

                // Controller가 다국어 번역 및 최종 표시용 DTO(NotificationDisplayData) 생성
                NotificationDisplayData displayData = CreateDisplayData(data);

                if (view != null)
                {
                    view.ShowNotification(displayData, () =>
                    {
                        _isDisplaying = false;
                    });
                }
                else
                {
                    _isDisplaying = false;
                }

                // 표시 종료될 때까지 대기
                while (_isDisplaying)
                {
                    yield return null;
                }
            }
            else
            {
                yield return null;
            }
        }

        _queueProcessCoroutine = null;
    }

    /// <summary>
    /// NotificationData를 바탕으로 LocalizationManager를 조회하여 화면 표시 전용 DTO(NotificationDisplayData)를 생성합니다.
    /// </summary>
    private NotificationDisplayData CreateDisplayData(NotificationData data)
    {
        string titleText = GetLocalizedText(data.titleKey);
        string descText = GetLocalizedText(data.descriptionKey);

        return new NotificationDisplayData
        {
            icon = data.icon,
            titleText = titleText,
            descriptionText = descText,
            duration = data.duration
        };
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
