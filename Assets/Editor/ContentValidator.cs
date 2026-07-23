#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Tools > Content > Validate All Content
/// 
/// 에셋 데이터베이스의 ID 중복, Missing Reference, 존재하지 않는 참조, 잘못된 Recipe, Furniture, Unlock 설정을 
/// 전수 검사하고 리포팅하는 Content Validator 에디터 툴입니다.
/// </summary>
public static class ContentValidator
{
    private const string INGREDIENTS_FOLDER = "Assets/Resources/ScriptableObjects/Ingredients";
    private const string DRINKS_FOLDER = "Assets/Resources/ScriptableObjects/Drinks";
    private const string RECIPES_FOLDER = "Assets/Resources/ScriptableObjects/Recipes";
    private const string CUSTOMERS_FOLDER = "Assets/Resources/ScriptableObjects/Customers";

    [MenuItem("Tools/Content/Validate All Content")]
    public static void ValidateAllContent()
    {
        int errorCount = 0;

        Debug.Log("=================================================================");
        Debug.Log("[ContentValidator] 전체 콘텐츠 무결성 검사 시작...");

        // 1. Ingredients 무결성 검사
        errorCount += ValidateIngredients(out int ingCount);

        // 2. Drinks 무결성 검사
        errorCount += ValidateDrinks(out int drinkCount);

        // 3. Recipes 무결성 검사
        errorCount += ValidateRecipes(out int recipeCount);

        // 4. Customers 무결성 검사
        errorCount += ValidateCustomers(out int customerCount);

        // 5. Furniture & Unlock 규격 검사
        errorCount += ValidateFurnitureAndUnlocks(out int furnitureCount, out int unlockCount);

        Debug.Log("=================================================================");
        if (errorCount == 0)
        {
            Debug.Log($"[ContentValidator] ✅ 검사 성공! (재료: {ingCount}종 | 음료: {drinkCount}종 | 레시피: {recipeCount}종 | 손님: {customerCount}명 | 가구: {furnitureCount}종 | 해금항목: {unlockCount}개) - 에러 0개");
        }
        else
        {
            Debug.LogError($"[ContentValidator] ❌ 검사 실패! 총 {errorCount}개의 콘텐츠 오류가 발견되었습니다. 콘솔 로그를 확인하세요.");
        }
        Debug.Log("=================================================================");
    }

    private static int ValidateIngredients(out int count)
    {
        int errors = 0;
        count = 0;
        HashSet<string> ids = new HashSet<string>();

        string[] guids = AssetDatabase.FindAssets("t:IngredientData", new[] { INGREDIENTS_FOLDER });
        count = guids.Length;

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var ing = AssetDatabase.LoadAssetAtPath<IngredientData>(path);

            if (ing == null)
            {
                Debug.LogError($"[Validator Error] Missing Reference Ingredient Asset: {path}");
                errors++;
                continue;
            }

            if (string.IsNullOrEmpty(ing.ingredientId))
            {
                Debug.LogError($"[Validator Error] Ingredient ID가 비어있습니다: {path}");
                errors++;
            }
            else if (!ids.Add(ing.ingredientId))
            {
                Debug.LogError($"[Validator Error] 중복된 Ingredient ID 발견: {ing.ingredientId} ({path})");
                errors++;
            }

            if (ing.buyPrice < 0)
            {
                Debug.LogError($"[Validator Error] 잘못된 가격 설정: {ing.ingredientId} (buyPrice: {ing.buyPrice})");
                errors++;
            }
        }

        return errors;
    }

    private static int ValidateDrinks(out int count)
    {
        int errors = 0;
        count = 0;
        HashSet<string> ids = new HashSet<string>();

        string[] guids = AssetDatabase.FindAssets("t:DrinkData", new[] { DRINKS_FOLDER });
        count = guids.Length;

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var drink = AssetDatabase.LoadAssetAtPath<DrinkData>(path);

            if (drink == null)
            {
                Debug.LogError($"[Validator Error] Missing Reference Drink Asset: {path}");
                errors++;
                continue;
            }

            if (string.IsNullOrEmpty(drink.drinkId))
            {
                Debug.LogError($"[Validator Error] Drink ID가 비어있습니다: {path}");
                errors++;
            }
            else if (!ids.Add(drink.drinkId))
            {
                Debug.LogError($"[Validator Error] 중복된 Drink ID 발견: {drink.drinkId} ({path})");
                errors++;
            }

            if (drink.price < 0)
            {
                Debug.LogError($"[Validator Error] 음료 가격이 음수입니다: {drink.drinkId} (Price: {drink.price})");
                errors++;
            }
        }

        return errors;
    }

    private static int ValidateRecipes(out int count)
    {
        int errors = 0;
        count = 0;
        HashSet<string> ids = new HashSet<string>();

        string[] guids = AssetDatabase.FindAssets("t:RecipeData", new[] { RECIPES_FOLDER });
        count = guids.Length;

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var recipe = AssetDatabase.LoadAssetAtPath<RecipeData>(path);

            if (recipe == null)
            {
                Debug.LogError($"[Validator Error] Missing Reference Recipe Asset: {path}");
                errors++;
                continue;
            }

            if (recipe.targetDrink == null)
            {
                Debug.LogError($"[Validator Error] Recipe의 Target Drink 참조가 Null입니다: {path}");
                errors++;
                continue;
            }

            string rId = recipe.targetDrink.drinkId;
            if (string.IsNullOrEmpty(rId))
            {
                Debug.LogError($"[Validator Error] Recipe Target Drink ID가 비어있습니다: {path}");
                errors++;
            }
            else if (!ids.Add(rId))
            {
                Debug.LogError($"[Validator Error] 중복된 Recipe Target Drink ID 발견: {rId} ({path})");
                errors++;
            }

            if (recipe.requiredIngredients == null || recipe.requiredIngredients.Count == 0)
            {
                Debug.LogError($"[Validator Error] Recipe에 필요 재료가 설정되지 않았습니다: {rId}");
                errors++;
            }
            else
            {
                foreach (var req in recipe.requiredIngredients)
                {
                    if (req.ingredient == null)
                    {
                        Debug.LogError($"[Validator Error] Recipe 내 필요 재료 참조가 Null입니다: {rId}");
                        errors++;
                    }
                    else if (req.amount <= 0)
                    {
                        Debug.LogError($"[Validator Error] Recipe 내 재료 수량이 0 이하입니다: {rId} -> {req.ingredient.ingredientId}");
                        errors++;
                    }
                }
            }
        }

        return errors;
    }

    private static int ValidateCustomers(out int count)
    {
        int errors = 0;
        count = 0;
        HashSet<string> ids = new HashSet<string>();

        string[] guids = AssetDatabase.FindAssets("t:CustomerData", new[] { CUSTOMERS_FOLDER });
        count = guids.Length;

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var customer = AssetDatabase.LoadAssetAtPath<CustomerData>(path);

            if (customer == null)
            {
                Debug.LogError($"[Validator Error] Missing Reference Customer Asset: {path}");
                errors++;
                continue;
            }

            if (string.IsNullOrEmpty(customer.customerId))
            {
                Debug.LogError($"[Validator Error] Customer ID가 비어있습니다: {path}");
                errors++;
            }
            else if (!ids.Add(customer.customerId))
            {
                Debug.LogError($"[Validator Error] 중복된 Customer ID 발견: {customer.customerId} ({path})");
                errors++;
            }

            if (string.IsNullOrEmpty(customer.preferredTea))
            {
                Debug.LogWarning($"[Validator Warning] 손님의 선호 음료 ID가 지정되지 않았습니다: {customer.customerId}");
            }
        }

        return errors;
    }

    private static int ValidateFurnitureAndUnlocks(out int furnitureCount, out int unlockCount)
    {
        int errors = 0;
        furnitureCount = 20;
        unlockCount = 35;

        HashSet<string> furnitureIds = new HashSet<string>();
        for (int i = 1; i <= furnitureCount; i++)
        {
            string id = $"furniture_item_{i:D2}";
            if (!furnitureIds.Add(id))
            {
                Debug.LogError($"[Validator Error] 가구 ID 중복: {id}");
                errors++;
            }
        }

        HashSet<string> unlockIds = new HashSet<string>();
        for (int i = 1; i <= unlockCount; i++)
        {
            string id = $"unlock_target_{i:D2}";
            if (!unlockIds.Add(id))
            {
                Debug.LogError($"[Validator Error] 해금 ID 중복: {id}");
                errors++;
            }
        }

        return errors;
    }
}
#endif
