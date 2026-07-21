using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 재료와 요구 수량을 매핑하는 구조체입니다.
/// </summary>
[System.Serializable]
public struct IngredientRequirement
{
    [Tooltip("요구되는 재료 데이터 에셋")]
    public IngredientData ingredient;

    [Tooltip("필요한 소모 수량 (개수/g 등)")]
    public int amount;
}

/// <summary>
/// 특정 완제품 음료를 제조하기 위한 레시피 데이터 (ScriptableObject).
/// 별도의 고유 ID나 우림 조건(온도/시간)을 중복 정의하지 않고,
/// targetDrink 식별 정보와 재료 소모 조건만 깔끔하게 관리합니다.
/// </summary>
[CreateAssetMenu(fileName = "Recipe_", menuName = "Tea Culture Game/Recipe Data")]
public class RecipeData : ScriptableObject
{
    // ──────────────────────────────────────────────────────────────────────
    // 기본 정보
    // ──────────────────────────────────────────────────────────────────────
    [Header("기본 정보")]

    [Tooltip("이 레시피를 통해 완성되는 음료 데이터 에셋 (1:1 매핑)")]
    public DrinkData targetDrink;

    // ──────────────────────────────────────────────────────────────────────
    // 요구 재료
    // ──────────────────────────────────────────────────────────────────────
    [Header("요구 재료")]

    [Tooltip("이 음료를 만들기 위해 필요한 재료 및 수량 목록 (1:N 매핑)")]
    public List<IngredientRequirement> requiredIngredients = new List<IngredientRequirement>();
}
