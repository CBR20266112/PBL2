#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Tools > Setup > Create Recipe Database
///
/// 사용자가 명시적으로 실행하는 Setup Tool입니다.
/// 기존 에셋을 덮어쓰지 않고 보존하며, 5종 레시피 데이터와 DB를 멱등성 있게 생성합니다.
/// </summary>
public static class SetupRecipeDatabase
{
    private const string RECIPES_FOLDER = "Assets/Resources/ScriptableObjects/Recipes";
    private const string DATABASE_PATH = "Assets/Resources/ScriptableObjects/Recipes/RecipeDatabase.asset";

    private const string DRINKS_FOLDER = "Assets/Resources/ScriptableObjects/Drinks";
    private const string INGREDIENTS_FOLDER = "Assets/Resources/ScriptableObjects/Ingredients";

    [MenuItem("Tools/Setup/Create Recipe Database")]
    public static void CreateRecipeDatabase()
    {
        // ── 1. 폴더 생성 ─────────────────────────────────────────────────
        EnsureFolder("Assets/Resources");
        EnsureFolder("Assets/Resources/ScriptableObjects");
        EnsureFolder(RECIPES_FOLDER);

        // ── 2. RecipeData 5종 생성 (없는 것만) ────────────────────────────
        int createdCount = 0;
        int skippedCount = 0;

        RecipeData yuzu     = CreateRecipeIfNotExists("Recipe_Yuzu",     ConfigureYuzu,     ref createdCount, ref skippedCount);
        RecipeData matcha   = CreateRecipeIfNotExists("Recipe_Matcha",   ConfigureMatcha,   ref createdCount, ref skippedCount);
        RecipeData puerh    = CreateRecipeIfNotExists("Recipe_Puerh",    ConfigurePuerh,    ref createdCount, ref skippedCount);
        RecipeData lotus    = CreateRecipeIfNotExists("Recipe_Lotus",    ConfigureLotus,    ref createdCount, ref skippedCount);
        RecipeData shyrChai = CreateRecipeIfNotExists("Recipe_ShyrChai", ConfigureShyrChai, ref createdCount, ref skippedCount);

        // ── 3. RecipeDatabase 생성 또는 기존 DB에 누락된 에셋 연결 ──────
        RecipeDatabase db = EnsureDatabase();
        bool dbChanged = false;

        RecipeData[] allRecipes = { yuzu, matcha, puerh, lotus, shyrChai };
        foreach (var recipe in allRecipes)
        {
            if (recipe != null && !db.recipes.Contains(recipe))
            {
                db.recipes.Add(recipe);
                dbChanged = true;
            }
        }

        if (dbChanged)
        {
            EditorUtility.SetDirty(db);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        string summary =
            $"RecipeDatabase 셋업 완료\n\n" +
            $"신규 생성: {createdCount}개\n" +
            $"이미 존재(스킵): {skippedCount}개\n\n" +
            $"경로: {RECIPES_FOLDER}\n\n" +
            (createdCount == 0
                ? "✅ 모든 레시피 에셋이 이미 존재합니다. 변경된 항목이 없습니다."
                : "5종 차 레시피 에셋이 Drink/Ingredient 데이터베이스와 성공적으로 연결되었습니다.");

        Debug.Log($"[SetupRecipeDatabase] {summary.Replace("\n", " | ")}");

        EditorUtility.DisplayDialog("RecipeDatabase 셋업 완료", summary, "확인");

        Selection.activeObject = db;
    }

    // ──────────────────────────────────────────────────────────────────────
    // 핵심 로직: 존재하지 않는 경우에만 생성 (Idempotent)
    // ──────────────────────────────────────────────────────────────────────

    private static RecipeData CreateRecipeIfNotExists(
        string assetName,
        System.Action<RecipeData> configure,
        ref int createdCount,
        ref int skippedCount)
    {
        string path = $"{RECIPES_FOLDER}/{assetName}.asset";
        RecipeData existing = AssetDatabase.LoadAssetAtPath<RecipeData>(path);

        if (existing != null)
        {
            skippedCount++;
            return existing;
        }

        RecipeData data = ScriptableObject.CreateInstance<RecipeData>();
        configure(data);
        
        // targetDrink가 제대로 찾기지 않은 경우 에셋 생성을 건너뛰어 무결성 유지
        if (data.targetDrink == null)
        {
            Debug.LogError($"[SetupRecipeDatabase] 완성 음료 데이터(DrinkData)를 찾지 못해 레시피 생성을 중단했습니다: {assetName}");
            return null;
        }

        AssetDatabase.CreateAsset(data, path);
        createdCount++;
        Debug.Log($"[SetupRecipeDatabase] 생성: {path}");
        return data;
    }

    private static RecipeDatabase EnsureDatabase()
    {
        RecipeDatabase existing = AssetDatabase.LoadAssetAtPath<RecipeDatabase>(DATABASE_PATH);
        if (existing != null)
        {
            return existing;
        }

        RecipeDatabase db = ScriptableObject.CreateInstance<RecipeDatabase>();
        AssetDatabase.CreateAsset(db, DATABASE_PATH);
        Debug.Log($"[SetupRecipeDatabase] RecipeDatabase 생성: {DATABASE_PATH}");
        return db;
    }

    // ──────────────────────────────────────────────────────────────────────
    // 레시피 데이터 자동 링킹 및 셋업 (신규 생성 시에만 적용)
    // ──────────────────────────────────────────────────────────────────────

    private static void ConfigureYuzu(RecipeData r)
    {
        r.targetDrink = AssetDatabase.LoadAssetAtPath<DrinkData>($"{DRINKS_FOLDER}/Drink_Yuzu.asset");
        
        var req1 = new IngredientRequirement
        {
            ingredient = AssetDatabase.LoadAssetAtPath<IngredientData>($"{INGREDIENTS_FOLDER}/Ing_Yuzu.asset"),
            amount = 2
        };
        r.requiredIngredients = new List<IngredientRequirement> { req1 };
    }

    private static void ConfigureMatcha(RecipeData r)
    {
        r.targetDrink = AssetDatabase.LoadAssetAtPath<DrinkData>($"{DRINKS_FOLDER}/Drink_Matcha.asset");
        
        var req1 = new IngredientRequirement
        {
            ingredient = AssetDatabase.LoadAssetAtPath<IngredientData>($"{INGREDIENTS_FOLDER}/Ing_Matcha.asset"),
            amount = 1
        };
        r.requiredIngredients = new List<IngredientRequirement> { req1 };
    }

    private static void ConfigurePuerh(RecipeData r)
    {
        r.targetDrink = AssetDatabase.LoadAssetAtPath<DrinkData>($"{DRINKS_FOLDER}/Drink_Puerh.asset");
        
        var req1 = new IngredientRequirement
        {
            ingredient = AssetDatabase.LoadAssetAtPath<IngredientData>($"{INGREDIENTS_FOLDER}/Ing_Puerh.asset"),
            amount = 1
        };
        r.requiredIngredients = new List<IngredientRequirement> { req1 };
    }

    private static void ConfigureLotus(RecipeData r)
    {
        r.targetDrink = AssetDatabase.LoadAssetAtPath<DrinkData>($"{DRINKS_FOLDER}/Drink_Lotus.asset");
        
        var req1 = new IngredientRequirement
        {
            ingredient = AssetDatabase.LoadAssetAtPath<IngredientData>($"{INGREDIENTS_FOLDER}/Ing_Lotus.asset"),
            amount = 1
        };
        r.requiredIngredients = new List<IngredientRequirement> { req1 };
    }

    private static void ConfigureShyrChai(RecipeData r)
    {
        r.targetDrink = AssetDatabase.LoadAssetAtPath<DrinkData>($"{DRINKS_FOLDER}/Drink_ShyrChai.asset");
        
        var req1 = new IngredientRequirement
        {
            ingredient = AssetDatabase.LoadAssetAtPath<IngredientData>($"{INGREDIENTS_FOLDER}/Ing_Ceylon.asset"),
            amount = 1
        };
        var req2 = new IngredientRequirement
        {
            ingredient = AssetDatabase.LoadAssetAtPath<IngredientData>($"{INGREDIENTS_FOLDER}/Ing_Milk.asset"),
            amount = 2
        };
        var req3 = new IngredientRequirement
        {
            ingredient = AssetDatabase.LoadAssetAtPath<IngredientData>($"{INGREDIENTS_FOLDER}/Ing_Salt.asset"),
            amount = 1
        };
        r.requiredIngredients = new List<IngredientRequirement> { req1, req2, req3 };
    }

    // ──────────────────────────────────────────────────────────────────────
    // 내부 유틸
    // ──────────────────────────────────────────────────────────────────────

    private static void EnsureFolder(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string parent = Path.GetDirectoryName(path).Replace('\\', '/');
            string folderName = Path.GetFileName(path);
            AssetDatabase.CreateFolder(parent, folderName);
            Debug.Log($"[SetupRecipeDatabase] 폴더 생성: {path}");
        }
    }
}
#endif
