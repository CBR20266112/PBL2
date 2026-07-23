using UnityEngine;

/// <summary>
/// NotificationView 및 NotificationItemView에 전달되는 화면 표시 전용 DTO 클래스입니다.
/// Controller에서 번역 및 포맷팅이 완료된 최종 텍스트와 이미지 정보를 보관합니다.
/// </summary>
public class NotificationDisplayData
{
    public Sprite icon;
    public string titleText;
    public string descriptionText;
    public float duration = 2.5f;
}
