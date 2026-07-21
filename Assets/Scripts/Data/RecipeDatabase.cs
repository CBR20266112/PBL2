using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 RecipeData를 중앙에서 관리하는 데이터베이스 (ScriptableObject).
/// targetDrink의 ID를 키로 하여 캐시를 구성하며, OnEnable()에서도 안전하게 초기화됩니다.
/// </summary>
[CreateAssetMenu(fileName = "RecipeDatabase", menuName = "Tea Culture Game/Recipe Database")]
public class RecipeDatabase : ScriptableObject
{
    [Tooltip("등록된 모든 레시피 데이터 목록. Setup Tool이 자동으로 관리해 줍니다.")]
    public List<RecipeData> recipes = new List<RecipeData>();

    private Dictionary<string, RecipeData> _recipeDict;

    private void OnEnable()
    {
        Initialize();
    }

    /// <summary>
    /// 런타임 검색 속도 최적화를 위해 완성 음료(targetDrink)의 ID를 기준으로 딕셔너리를 빌드합니다.
    /// </summary>
    public void Initialize()
    {
        _recipeDict = new Dictionary<string, RecipeData>();
        foreach (var recipe in recipes)
        {
            if (recipe == null || recipe.targetDrink == null) continue;
            
            string drinkId = recipe.targetDrink.drinkId;
            if (string.IsNullOrEmpty(drinkId)) continue;

            if (_recipeDict.ContainsKey(drinkId))
            {
                Debug.LogWarning($"[RecipeDatabase] 중복된 음료 레시피 발견: '{drinkId}'. 첫 번째 항목만 등록됩니다.");
                continue;
            }
            _recipeDict.Add(drinkId, recipe);
        }
    }

    /// <summary>
    /// 음료 고유 ID(drinkId)를 통해 해당 음료의 레시피 데이터를 가져옵니다.
    /// </summary>
    public RecipeData GetRecipe(string drinkId)
    {
        EnsureInitialized();
        if (_recipeDict.TryGetValue(drinkId, out RecipeData data))
            return data;

        Debug.LogWarning($"[RecipeDatabase] 음료 ID '{drinkId}'에 해당하는 레시피를 찾을 수 없습니다.");
        return null;
    }

    /// <summary>
    /// 등록된 전체 레시피 목록을 반환합니다. (읽기 전용)
    /// </summary>
    public IReadOnlyList<RecipeData> GetAllRecipes()
    {
        return recipes.AsReadOnly();
    }

    private void EnsureInitialized()
    {
        if (_recipeDict == null)
            Initialize();
    }
}
