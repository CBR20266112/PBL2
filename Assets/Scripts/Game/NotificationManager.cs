using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 알림 데이터(NotificationData)의 큐(Queue) 저장 및 관리를 전담하는 독립 싱글톤 매니저입니다.
/// 다른 Manager를 직접 참조하지 않으며, 알림 출력 연출이나 타이머는 Controller가 담당합니다.
/// </summary>
public class NotificationManager : Singleton<NotificationManager>
{
    private readonly Queue<NotificationData> _queue = new Queue<NotificationData>();

    // 새로운 알림이 큐에 추가될 때 발생하는 이벤트
    public event Action OnNotificationEnqueued;

    /// <summary>
    /// 큐에 보관된 알림 수
    /// </summary>
    public int QueueCount => _queue.Count;

    /// <summary>
    /// 큐에 알림이 존재하는지 여부
    /// </summary>
    public bool HasPendingNotifications => _queue.Count > 0;

    /// <summary>
    /// 알림 데이터를 큐에 추가합니다.
    /// 향후 동일 알림 병합(Aggregation) 기능을 확장할 수 있는 진입점입니다.
    /// </summary>
    public void Enqueue(NotificationData data)
    {
        if (data == null) return;

        _queue.Enqueue(data);
        Debug.Log($"[NotificationManager] 알림 Enqueue: {data.notificationType} (대기 수: {_queue.Count})");
        OnNotificationEnqueued?.Invoke();
    }

    /// <summary>
    /// Controller가 큐에서 알림을 하나씩 꺼내 출력하기 위한 큐 꺼내기 메서드입니다.
    /// </summary>
    public bool TryDequeue(out NotificationData data)
    {
        if (_queue.Count > 0)
        {
            data = _queue.Dequeue();
            return true;
        }

        data = null;
        return false;
    }

    /// <summary>
    /// 큐의 가장 앞 알림을 조회합니다. (선부 제거 없음)
    /// </summary>
    public NotificationData Peek()
    {
        return _queue.Count > 0 ? _queue.Peek() : null;
    }

    /// <summary>
    /// 큐를 완전히 초기화합니다.
    /// </summary>
    public void ClearQueue()
    {
        _queue.Clear();
    }
}
