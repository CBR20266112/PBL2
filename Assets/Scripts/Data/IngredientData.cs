using UnityEngine;

/// <summary>
/// 재료 데이터 컨테이너 (ScriptableObject).
/// 차 제조에 사용되는 원재료의 데이터를 정의합니다.
/// 재료는 모든 음료가 공유하는 공통 자원으로 취급하며, 국가 정보는 가지지 않습니다.
/// </summary>
[CreateAssetMenu(fileName = "Ingredient_", menuName = "Tea Culture Game/Ingredient Data")]
public class IngredientData : ScriptableObject
{
    // ──────────────────────────────────────────────────────────────────────
    // 기본 정보
    // ──────────────────────────────────────────────────────────────────────
    [Header("기본 정보")]

    [Tooltip("재료의 고유 ID. 코드 및 저장 데이터에서 이 값을 키로 사용합니다. (예: green_tea_leaves, milk)")]
    public string ingredientId;

    [Tooltip("UI에 표시될 이름.")]
    public string displayName;

    [Tooltip("인벤토리 및 샵 UI에 표시될 스프라이트 아이콘.")]
    public Sprite icon;

    // ──────────────────────────────────────────────────────────────────────
    // 경제
    // ──────────────────────────────────────────────────────────────────────
    [Header("경제")]

    [Tooltip("상점에서 구매할 때의 가격 (원)")]
    public int buyPrice = 1000;

    [Tooltip("이 재료를 상점에서 해금하기 위해 필요한 플레이어 최소 레벨. 0이면 처음부터 제공.")]
    public int unlockLevel = 0;

    // [향후 경제 시스템 확장 가능 지점]
    // 예: public int sellPrice; // 플레이어가 재료를 직접 되팔 때의 가격 등

    // ──────────────────────────────────────────────────────────────────────
    // 설명
    // ──────────────────────────────────────────────────────────────────────
    [Header("설명")]

    [TextArea(2, 4)]
    [Tooltip("재료에 대한 게임 내 설명.")]
    public string description;
}
