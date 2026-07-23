using UnityEngine;

/// <summary>
/// 알림 데이터를 나타내는 순수 DTO 클래스입니다.
/// 비즈니스 객체 및 직접적인 렌더링 로직을 가지지 않습니다.
/// </summary>
public class NotificationData
{
    public NotificationType notificationType;
    public string titleKey;
    public string descriptionKey;
    public Sprite icon;
    public float duration = 2.5f;

    public NotificationData() { }

    public NotificationData(NotificationType type, string titleKey, string descriptionKey, Sprite icon = null, float duration = 2.5f)
    {
        this.notificationType = type;
        this.titleKey = titleKey;
        this.descriptionKey = descriptionKey;
        this.icon = icon;
        this.duration = duration;
    }
}
