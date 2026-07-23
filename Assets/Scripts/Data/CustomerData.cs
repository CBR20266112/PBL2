using UnityEngine;

/// <summary>
/// 손님 데이터 (ScriptableObject)
/// </summary>
[CreateAssetMenu(fileName = "Customer_", menuName = "Tea Culture Game/Customer Data")]
public class CustomerData : ScriptableObject
{
    [Header("기본 식별 정보")]
    [Tooltip("손님 고유 ID (예: customer_luna)")]
    public string customerId;

    [Tooltip("UI 표시용 이름 Localization Key (예: customer.name.sora)")]
    public string displayNameKey;

    [Tooltip("손님 국적 분류 (CountryType Enum)")]
    public CountryType nationality = CountryType.Universal;

    [TextArea(2, 4)]
    [Tooltip("손님 설명 Localization Key (예: customer.desc.sora)")]
    public string descriptionKey;

    [Header("Visual 에셋")]
    [Tooltip("초상화 아이콘")]
    public Sprite portrait;

    [Tooltip("손님 캐릭터 스탠딩 스프라이트")]
    public Sprite sprite;

    // ──────────────────────────────────────────────────────────────────────
    // 기존 호환용 필드 (하위 호환성 유지)
    // ──────────────────────────────────────────────────────────────────────
    [Header("기존 호환 필드")]
    public string customerName;
    public string characterType; // "luna", "hyuntae", "wei", "sakura", "denu"
    public Sprite characterSprite;
    public string preferredTea; // "yuzu", "matcha", "puerh", "lotus", "chai"
    public int preferredTemperature; // 0=low, 1=medium, 2=high
    public int preferredSteepTime; // 0=short, 1=medium, 2=long
    public string personality; // 성격 설명
    public string storyBit; // 이 손님의 짧은 스토리
}
