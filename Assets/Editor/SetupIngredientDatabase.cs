#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Tools > Setup > Create Ingredient Database
///
/// 사용자가 명시적으로 실행하는 Setup Tool입니다.
/// 자동 실행하지 않으며, 이미 존재하는 에셋을 덮어쓰지 않고 보존합니다.
///
/// 동작 규칙:
///  - IngredientDatabase가 이미 존재하면 새로 생성하지 않음
///  - IngredientData는 존재하지 않는 에셋만 신규 생성
///  - 이미 존재하는 IngredientData의 기존 데이터는 절대 덮어쓰지 않음
///  - 여러 번 실행해도 안전(Idempotent)하게 동작
/// </summary>
public static class SetupIngredientDatabase
{
    private const string INGREDIENTS_FOLDER = "Assets/Resources/ScriptableObjects/Ingredients";
    private const string DATABASE_PATH = "Assets/Resources/ScriptableObjects/Ingredients/IngredientDatabase.asset";

    [MenuItem("Tools/Setup/Create Ingredient Database")]
    public static void CreateIngredientDatabase()
    {
        // ── 1. 폴더 생성 ─────────────────────────────────────────────────
        EnsureFolder("Assets/Resources");
        EnsureFolder("Assets/Resources/ScriptableObjects");
        EnsureFolder(INGREDIENTS_FOLDER);

        // ── 2. IngredientData 7종 생성 (없는 것만) ───────────────────────
        int createdCount = 0;
        int skippedCount = 0;

        IngredientData yuzu     = CreateIngredientIfNotExists("Ing_Yuzu",     ConfigureYuzu,     ref createdCount, ref skippedCount);
        IngredientData ceylon   = CreateIngredientIfNotExists("Ing_Ceylon",   ConfigureCeylon,   ref createdCount, ref skippedCount);
        IngredientData matcha   = CreateIngredientIfNotExists("Ing_Matcha",   ConfigureMatcha,   ref createdCount, ref skippedCount);
        IngredientData puerh    = CreateIngredientIfNotExists("Ing_Puerh",    ConfigurePuerh,    ref createdCount, ref skippedCount);
        IngredientData lotus    = CreateIngredientIfNotExists("Ing_Lotus",    ConfigureLotus,    ref createdCount, ref skippedCount);
        IngredientData milk     = CreateIngredientIfNotExists("Ing_Milk",     ConfigureMilk,     ref createdCount, ref skippedCount);
        IngredientData salt     = CreateIngredientIfNotExists("Ing_Salt",     ConfigureSalt,     ref createdCount, ref skippedCount);

        // ── 3. IngredientDatabase 생성 또는 기존 DB에 누락된 에셋 연결 ──
        IngredientDatabase db = EnsureDatabase();
        bool dbChanged = false;

        IngredientData[] allIngredients = { yuzu, ceylon, matcha, puerh, lotus, milk, salt };
        foreach (var ing in allIngredients)
        {
            if (ing != null && !db.ingredients.Contains(ing))
            {
                db.ingredients.Add(ing);
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
            $"IngredientDatabase 셋업 완료\n\n" +
            $"신규 생성: {createdCount}개\n" +
            $"이미 존재(스킵): {skippedCount}개\n\n" +
            $"경로: {INGREDIENTS_FOLDER}\n\n" +
            (createdCount == 0
                ? "✅ 모든 재료 에셋이 이미 존재합니다. 변경된 항목이 없습니다."
                : "[사용자 작업 필요]\n각 IngredientData의 icon 스프라이트를 Inspector에서 지정해 주세요.");

        Debug.Log($"[SetupIngredientDatabase] {summary.Replace("\n", " | ")}");

        EditorUtility.DisplayDialog("IngredientDatabase 셋업 완료", summary, "확인");

        Selection.activeObject = db;
    }

    // ──────────────────────────────────────────────────────────────────────
    // 핵심 로직: 존재하지 않는 경우에만 생성 (Idempotent)
    // ──────────────────────────────────────────────────────────────────────

    private static IngredientData CreateIngredientIfNotExists(
        string assetName,
        System.Action<IngredientData> configure,
        ref int createdCount,
        ref int skippedCount)
    {
        string path = $"{INGREDIENTS_FOLDER}/{assetName}.asset";
        IngredientData existing = AssetDatabase.LoadAssetAtPath<IngredientData>(path);

        if (existing != null)
        {
            skippedCount++;
            return existing;
        }

        IngredientData data = ScriptableObject.CreateInstance<IngredientData>();
        configure(data);
        AssetDatabase.CreateAsset(data, path);
        createdCount++;
        Debug.Log($"[SetupIngredientDatabase] 생성: {path}");
        return data;
    }

    private static IngredientDatabase EnsureDatabase()
    {
        IngredientDatabase existing = AssetDatabase.LoadAssetAtPath<IngredientDatabase>(DATABASE_PATH);
        if (existing != null)
        {
            return existing;
        }

        IngredientDatabase db = ScriptableObject.CreateInstance<IngredientDatabase>();
        AssetDatabase.CreateAsset(db, DATABASE_PATH);
        Debug.Log($"[SetupIngredientDatabase] IngredientDatabase 생성: {DATABASE_PATH}");
        return db;
    }

    // ──────────────────────────────────────────────────────────────────────
    // 재료 초기값 설정 (신규 생성 시에만 적용)
    // ──────────────────────────────────────────────────────────────────────

    private static void ConfigureYuzu(IngredientData d)
    {
        d.ingredientId = "yuzu_syrup";
        d.displayName  = "유자 청";
        d.buyPrice     = 800;
        d.unlockLevel  = 0;
        d.description  = "달콤하고 상큼한 전통 유자 청입니다.";
    }

    private static void ConfigureCeylon(IngredientData d)
    {
        d.ingredientId = "ceylon_leaves";
        d.displayName  = "실론 홍찻잎";
        d.buyPrice     = 1000;
        d.unlockLevel  = 0;
        d.description  = "풍부한 바디감과 향을 가진 기본 홍찻잎입니다.";
    }

    private static void ConfigureMatcha(IngredientData d)
    {
        d.ingredientId = "matcha_powder";
        d.displayName  = "말차 가루";
        d.buyPrice     = 1200;
        d.unlockLevel  = 0;
        d.description  = "곱게 갈아 만든 진한 연두빛의 일본식 말차 가루입니다.";
    }

    private static void ConfigurePuerh(IngredientData d)
    {
        d.ingredientId = "puerh_leaves";
        d.displayName  = "보이찻잎";
        d.buyPrice     = 2000;
        d.unlockLevel  = 5;
        d.description  = "정성스럽게 발효되어 깊은 흙내음이 나는 중국 보이찻잎입니다.";
    }

    private static void ConfigureLotus(IngredientData d)
    {
        d.ingredientId = "lotus_leaves";
        d.displayName  = "연꽃잎";
        d.buyPrice     = 1500;
        d.unlockLevel  = 5;
        d.description  = "베트남 전통 방식으로 연꽃 향을 입힌 향긋한 연잎입니다.";
    }

    private static void ConfigureMilk(IngredientData d)
    {
        d.ingredientId = "milk";
        d.displayName  = "우유";
        d.buyPrice     = 500;
        d.unlockLevel  = 0;
        d.description  = "Shyr Chai 및 밀크티 제조를 위한 고소한 신선 우유입니다.";
    }

    private static void ConfigureSalt(IngredientData d)
    {
        d.ingredientId = "salt";
        d.displayName  = "소금";
        d.buyPrice     = 100;
        d.unlockLevel  = 0;
        d.description  = "Shyr Chai 제조에 들어가는 부드러운 천일염입니다.";
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
            Debug.Log($"[SetupIngredientDatabase] 폴더 생성: {path}");
        }
    }
}
#endif
