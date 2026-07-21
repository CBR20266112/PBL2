using UnityEngine;

/// <summary>
/// 문화권(국가) 분류 열거형.
/// DrinkData.origin에 사용하며, 향후 테마 시스템(ThemeAudioData)과도 연동 가능합니다.
/// </summary>
public enum CountryType
{
    Universal,      // 특정 문화권에 속하지 않는 공통 음료
    Korea,          // 한국
    China,          // 중국
    Japan,          // 일본
    Vietnam,        // 베트남
    Kyrgyzstan      // 키르기스스탄
}

/// <summary>
/// 음료 데이터 컨테이너 (ScriptableObject).
/// 하나의 음료에 대한 기본 정보, 경제 데이터, 제조 파라미터를 Inspector에서 관리합니다.
///
/// [다국어 설계 원칙]
/// displayName은 현재 직접 string으로 관리합니다.
/// 향후 Unity Localization 패키지 도입 시, displayName을 LocalizedString으로
/// 교체할 수 있도록 비직렬화 래퍼(LocalizedString)를 별도 레이어에서 추가하는 방식으로
/// 확장합니다. 이 파일 자체는 수정 최소화를 목표로 합니다.
/// </summary>
[CreateAssetMenu(fileName = "Drink_", menuName = "Tea Culture Game/Drink Data")]
public class DrinkData : ScriptableObject
{
    // ──────────────────────────────────────────────────────────────────────
    // 기본 정보
    // ──────────────────────────────────────────────────────────────────────
    [Header("기본 정보")]

    [Tooltip("음료의 고유 ID. 코드 및 저장 데이터에서 이 값을 키로 사용합니다. (예: yuzu, matcha)")]
    public string drinkId;

    [Tooltip("UI에 표시될 이름. 향후 Localization 적용 시 LocalizedString 레이어로 대체 가능.")]
    public string displayName;

    [Tooltip("이 음료가 속한 문화권")]
    public CountryType origin = CountryType.Universal;

    [Tooltip("향후 UI 아이콘. 현재 아트 에셋 미확정 시 비워 두어도 동작합니다.")]
    public Sprite icon;

    // ──────────────────────────────────────────────────────────────────────
    // 경제
    // ──────────────────────────────────────────────────────────────────────
    [Header("경제")]

    [Tooltip("손님에게 판매하는 가격 (원)")]
    public int price = 4000;

    [Tooltip("음료 제조에 소모되는 재료비 (원)")]
    public int cost = 1500;

    [Tooltip("이 음료를 해금하는 데 필요한 최소 플레이어 레벨. 0이면 기본 해금.")]
    public int unlockLevel = 0;

    // ──────────────────────────────────────────────────────────────────────
    // 제조
    // ──────────────────────────────────────────────────────────────────────
    [Header("제조 파라미터")]

    [Tooltip("이 음료의 적정 온도 단계. 0=낮음, 1=중간, 2=높음")]
    [Range(0, 2)]
    public int defaultTemperature = 1;

    [Tooltip("이 음료의 적정 우림 시간 단계. 0=짧음, 1=중간, 2=길음")]
    [Range(0, 2)]
    public int defaultSteepTime = 1;

    // ──────────────────────────────────────────────────────────────────────
    // 설명
    // ──────────────────────────────────────────────────────────────────────
    [Header("설명")]

    [TextArea(2, 4)]
    [Tooltip("음료에 대한 짧은 게임 내 설명 (향후 툴팁 또는 컬렉션 화면에서 사용)")]
    public string description;
}
