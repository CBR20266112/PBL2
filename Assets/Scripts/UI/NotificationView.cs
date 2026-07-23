using System;
using UnityEngine;

/// <summary>
/// Toast 알림 창을 생성/제거하고 화면 출력을 관리하는 View 클래스입니다.
/// ShowNotification(NotificationDisplayData) API로 전용 DTO 하나만 전달받으며, 향후 Object Pool 적용이 가능한 구조입니다.
/// </summary>
public class NotificationView : MonoBehaviour
{
    [SerializeField] private Transform toastContainer;
    [SerializeField] private NotificationItemView itemPrefab;

    private NotificationItemView _currentItemView;

    /// <summary>
    /// 화면 표시 전용 DTO(NotificationDisplayData)를 전달받아 알림을 화면에 렌더링하고 연출을 진행합니다.
    /// </summary>
    public void ShowNotification(NotificationDisplayData displayData, Action onHideComplete)
    {
        if (displayData == null || itemPrefab == null || toastContainer == null)
        {
            onHideComplete?.Invoke();
            return;
        }

        // 기존 뷰 제거 (향후 Object Pool로 대체 가능)
        HideNotification();

        // 새 아이템 생성
        _currentItemView = Instantiate(itemPrefab, toastContainer);
        _currentItemView.Setup(displayData);
        _currentItemView.AnimateShow(displayData.duration, () =>
        {
            HideNotification();
            onHideComplete?.Invoke();
        });
    }

    /// <summary>
    /// 현재 활성화된 알림 아이템을 즉시 숨기고 제거합니다.
    /// (향후 Object Pool 적용 시 Pool.Release로 대체)
    /// </summary>
    public void HideNotification()
    {
        if (_currentItemView != null)
        {
            Destroy(_currentItemView.gameObject);
            _currentItemView = null;
        }
    }
}
