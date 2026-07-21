#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Tools > Setup > Create Drink Database
///
/// 사용자가 명시적으로 실행하는 Setup Tool입니다.
/// 자동 실행(InitializeOnLoad)을 사용하지 않으며,
/// 여러 번 실행해도 기존 데이터를 덮어쓰지 않는 Idempotent 동작을 보장합니다.
///
/// 동작 규칙:
///  - DrinkDatabase가 이미 존재하면 새로 생성하지 않음
///  - DrinkData는 존재하지 않는 항목만 신규 생성
///  - 이미 존재하는 DrinkData는 수정하지 않음
///  - 여러 번 실행해도 안전
/// </summary>
public static class SetupDrinkDatabase
{
    private const string DRINKS_FOLDER = "Assets/Resources/ScriptableObjects/Drinks";
    private const string DATABASE_PATH = "Assets/Resources/ScriptableObjects/Drinks/DrinkDatabase.asset";

    [MenuItem("Tools/Setup/Create Drink Database")]
    public static void CreateDrinkDatabase()
    {
        // ── 1. 폴더 생성 ─────────────────────────────────────────────────
        EnsureFolder("Assets/Resources");
        EnsureFolder("Assets/Resources/ScriptableObjects");
        EnsureFolder(DRINKS_FOLDER);

        // ── 2. DrinkData 5종 생성 (없는 것만) ────────────────────────────
        int createdCount = 0;
        int skippedCount = 0;

        DrinkData yuzu     = CreateDrinkIfNotExists("Drink_Yuzu",     ConfigureYuzu,     ref createdCount, ref skippedCount);
        DrinkData matcha   = CreateDrinkIfNotExists("Drink_Matcha",   ConfigureMatcha,   ref createdCount, ref skippedCount);
        DrinkData puerh    = CreateDrinkIfNotExists("Drink_Puerh",    ConfigurePuerh,    ref createdCount, ref skippedCount);
        DrinkData lotus    = CreateDrinkIfNotExists("Drink_Lotus",    ConfigureLotus,    ref createdCount, ref skippedCount);
        DrinkData shyrChai = CreateDrinkIfNotExists("Drink_ShyrChai", ConfigureShyrChai, ref createdCount, ref skippedCount);

        // ── 3. DrinkDatabase 생성 또는 기존 DB에 누락된 항목만 추가 ──────
        DrinkDatabase db = EnsureDatabase();
        bool dbChanged = false;

        DrinkData[] allDrinks = { yuzu, matcha, puerh, lotus, shyrChai };
        foreach (var drink in allDrinks)
        {
            if (drink != null && !db.drinks.Contains(drink))
            {
                db.drinks.Add(drink);
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
            $"DrinkDatabase 셋업 완료\n\n" +
            $"신규 생성: {createdCount}개\n" +
            $"이미 존재(스킵): {skippedCount}개\n\n" +
            $"경로: {DRINKS_FOLDER}\n\n" +
            (createdCount == 0
                ? "✅ 모든 에셋이 이미 존재합니다. 변경된 항목이 없습니다."
                : "[사용자 작업 필요]\n각 DrinkData의 icon 스프라이트를 Inspector에서 지정해 주세요.");

        Debug.Log($"[SetupDrinkDatabase] {summary.Replace("\n", " | ")}");

        EditorUtility.DisplayDialog("DrinkDatabase 셋업 완료", summary, "확인");

        Selection.activeObject = db;
    }

    // ──────────────────────────────────────────────────────────────────────
    // 핵심 로직: 존재하지 않는 경우에만 생성
    // ──────────────────────────────────────────────────────────────────────

    /// <summary>
    /// 에셋이 없는 경우에만 새로 생성합니다. 이미 있으면 그대로 반환합니다.
    /// </summary>
    private static DrinkData CreateDrinkIfNotExists(
        string assetName,
        System.Action<DrinkData> configure,
        ref int createdCount,
        ref int skippedCount)
    {
        string path = $"{DRINKS_FOLDER}/{assetName}.asset";
        DrinkData existing = AssetDatabase.LoadAssetAtPath<DrinkData>(path);

        if (existing != null)
        {
            skippedCount++;
            return existing;
        }

        DrinkData data = ScriptableObject.CreateInstance<DrinkData>();
        configure(data);
        AssetDatabase.CreateAsset(data, path);
        createdCount++;
        Debug.Log($"[SetupDrinkDatabase] 생성: {path}");
        return data;
    }

    /// <summary>
    /// DrinkDatabase가 없는 경우에만 새로 생성합니다. 이미 있으면 그대로 반환합니다.
    /// </summary>
    private static DrinkDatabase EnsureDatabase()
    {
        DrinkDatabase existing = AssetDatabase.LoadAssetAtPath<DrinkDatabase>(DATABASE_PATH);
        if (existing != null)
        {
            return existing;
        }

        DrinkDatabase db = ScriptableObject.CreateInstance<DrinkDatabase>();
        AssetDatabase.CreateAsset(db, DATABASE_PATH);
        Debug.Log($"[SetupDrinkDatabase] DrinkDatabase 생성: {DATABASE_PATH}");
        return db;
    }

    // ──────────────────────────────────────────────────────────────────────
    // 음료 초기값 설정 (신규 생성 시에만 호출됨)
    // ──────────────────────────────────────────────────────────────────────

    private static void ConfigureYuzu(DrinkData d)
    {
        d.drinkId            = "yuzu";
        d.displayName        = "유자차";
        d.origin             = CountryType.Korea;
        d.price              = 4000;
        d.cost               = 1500;
        d.unlockLevel        = 0;
        d.defaultTemperature = 1;
        d.defaultSteepTime   = 1;
        d.description        = "상큼한 유자향이 가득한 한국 전통 차. 카페를 처음 열 때부터 제공됩니다.";
    }

    private static void ConfigureMatcha(DrinkData d)
    {
        d.drinkId            = "matcha";
        d.displayName        = "말차";
        d.origin             = CountryType.Japan;
        d.price              = 5000;
        d.cost               = 2000;
        d.unlockLevel        = 0;
        d.defaultTemperature = 1;
        d.defaultSteepTime   = 0;
        d.description        = "깊고 진한 녹색이 특징인 일본 전통 말차. 섬세한 쓴맛과 감칠맛이 조화롭습니다.";
    }

    private static void ConfigurePuerh(DrinkData d)
    {
        d.drinkId            = "puerh";
        d.displayName        = "보이차";
        d.origin             = CountryType.China;
        d.price              = 7000;
        d.cost               = 2500;
        d.unlockLevel        = 5;
        d.defaultTemperature = 2;
        d.defaultSteepTime   = 2;
        d.description        = "오랜 발효 과정을 거친 중국 차. 깊고 풍부한 맛이 특징이며 레벨 5부터 해금됩니다.";
    }

    private static void ConfigureLotus(DrinkData d)
    {
        d.drinkId            = "lotus";
        d.displayName        = "연꽃차";
        d.origin             = CountryType.Vietnam;
        d.price              = 5500;
        d.cost               = 1800;
        d.unlockLevel        = 5;
        d.defaultTemperature = 1;
        d.defaultSteepTime   = 1;
        d.description        = "베트남의 연꽃에서 추출한 향긋한 차. 은은하고 우아한 꽃향기가 매력적입니다.";
    }

    private static void ConfigureShyrChai(DrinkData d)
    {
        d.drinkId            = "shyr_chai";
        d.displayName        = "Shyr Chai";
        d.origin             = CountryType.Kyrgyzstan;
        d.price              = 6000;
        d.cost               = 2200;
        d.unlockLevel        = 10;
        d.defaultTemperature = 2;
        d.defaultSteepTime   = 2;
        d.description        = "키르기스스탄의 전통 소금 차. 독특한 풍미가 특별한 경험을 선사합니다.";
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
            Debug.Log($"[SetupDrinkDatabase] 폴더 생성: {path}");
        }
    }
}
#endif
