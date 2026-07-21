using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 IngredientData를 중앙에서 관리하는 데이터베이스 (ScriptableObject).
/// 런타임에 O(1) 조회를 위해 Dictionary 캐싱을 사용하며, OnEnable()에서도 안전하게 초기화됩니다.
/// </summary>
[CreateAssetMenu(fileName = "IngredientDatabase", menuName = "Tea Culture Game/Ingredient Database")]
public class IngredientDatabase : ScriptableObject
{
    [Tooltip("등록된 모든 재료 데이터 목록. Setup Tool이 자동으로 관리해 줍니다.")]
    public List<IngredientData> ingredients = new List<IngredientData>();

    private Dictionary<string, IngredientData> _ingredientDict;

    private void OnEnable()
    {
        Initialize();
    }

    /// <summary>
    /// 런타임 검색 속도 최적화를 위해 딕셔너리를 빌드합니다.
    /// </summary>
    public void Initialize()
    {
        _ingredientDict = new Dictionary<string, IngredientData>();
        foreach (var ing in ingredients)
        {
            if (ing == null) continue;
            if (string.IsNullOrEmpty(ing.ingredientId)) continue;
            if (_ingredientDict.ContainsKey(ing.ingredientId))
            {
                Debug.LogWarning($"[IngredientDatabase] 중복된 ingredientId 발견: '{ing.ingredientId}'. 첫 번째 항목만 등록됩니다.");
                continue;
            }
            _ingredientDict.Add(ing.ingredientId, ing);
        }
    }

    /// <summary>
    /// ID로 IngredientData를 조회합니다. 존재하지 않으면 null을 반환합니다.
    /// </summary>
    public IngredientData GetIngredient(string ingredientId)
    {
        EnsureInitialized();
        if (_ingredientDict.TryGetValue(ingredientId, out IngredientData data))
            return data;

        Debug.LogWarning($"[IngredientDatabase] ingredientId '{ingredientId}'를 찾을 수 없습니다.");
        return null;
    }

    /// <summary>
    /// 플레이어 레벨 기준으로 구매 가능한 재료 목록을 가져옵니다.
    /// </summary>
    public List<IngredientData> GetUnlockedIngredients(int playerLevel)
    {
        EnsureInitialized();
        var result = new List<IngredientData>();
        foreach (var ing in ingredients)
        {
            if (ing != null && playerLevel >= ing.unlockLevel)
                result.Add(ing);
        }
        return result;
    }

    /// <summary>
    /// 등록된 전체 재료 목록을 반환합니다. (읽기 전용)
    /// </summary>
    public IReadOnlyList<IngredientData> GetAllIngredients()
    {
        return ingredients.AsReadOnly();
    }

    private void EnsureInitialized()
    {
        if (_ingredientDict == null)
            Initialize();
    }
}
