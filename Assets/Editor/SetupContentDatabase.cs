#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Tools > Setup > Build Full Content Database
/// 
/// 실사 게임 수준의 대규모 콘텐츠 (Ingredient 22+, Drink 32+, Recipe 32+, Customer 20+)를
/// 실제 ScriptableObject 데이터 규격에 맞춰 일괄 생성 및 데이터베이스 에셋에 등록합니다.
/// </summary>
public static class SetupContentDatabase
{
    private const string INGREDIENTS_FOLDER = "Assets/Resources/ScriptableObjects/Ingredients";
    private const string DRINKS_FOLDER = "Assets/Resources/ScriptableObjects/Drinks";
    private const string RECIPES_FOLDER = "Assets/Resources/ScriptableObjects/Recipes";
    private const string CUSTOMERS_FOLDER = "Assets/Resources/ScriptableObjects/Customers";

    [MenuItem("Tools/Setup/Build Full Content Database")]
    public static void BuildContentDatabase()
    {
        EnsureFolder(INGREDIENTS_FOLDER);
        EnsureFolder(DRINKS_FOLDER);
        EnsureFolder(RECIPES_FOLDER);
        EnsureFolder(CUSTOMERS_FOLDER);

        var ingredients = CreateIngredients();
        var drinks = CreateDrinks();
        var recipes = CreateRecipes(ingredients, drinks);
        var customers = CreateCustomers(drinks);

        RegisterDatabases(ingredients, drinks, recipes, customers);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("=================================================================");
        Debug.Log("[SetupContentDatabase] 대규모 콘텐츠 (Ingredient 22+, Drink 32+, Recipe 32+, Customer 20+) 구축 완료!");
        Debug.Log($"재료: {ingredients.Count}종 | 음료: {drinks.Count}종 | 레시피: {recipes.Count}종 | 손님: {customers.Count}종");
        Debug.Log("=================================================================");

        // 자동 무결성 검사 실행
        ContentValidator.ValidateAllContent();
    }

    private static List<IngredientData> CreateIngredients()
    {
        List<IngredientData> list = new List<IngredientData>();

        // 초반 8종
        list.Add(GetOrCreateIngredient("Ing_Yuzu", "유자청", 10, 0));
        list.Add(GetOrCreateIngredient("Ing_Matcha", "말차가루", 15, 0));
        list.Add(GetOrCreateIngredient("Ing_Honey", "토종꿀", 12, 0));
        list.Add(GetOrCreateIngredient("Ing_Ginger", "생강청", 14, 0));
        list.Add(GetOrCreateIngredient("Ing_Jujube", "대추", 16, 0));
        list.Add(GetOrCreateIngredient("Ing_Cinnamon", "계피", 18, 0));
        list.Add(GetOrCreateIngredient("Ing_Plum", "매실청", 20, 0));
        list.Add(GetOrCreateIngredient("Ing_Barley", "보리", 8, 0));

        // 중반 8종
        list.Add(GetOrCreateIngredient("Ing_Puerh", "보이차엽", 30, 2));
        list.Add(GetOrCreateIngredient("Ing_Ceylon", "홍차엽", 35, 2));
        list.Add(GetOrCreateIngredient("Ing_Milk", "신선한 우유", 25, 2));
        list.Add(GetOrCreateIngredient("Ing_Chrysanthemum", "국화꽃", 40, 3));
        list.Add(GetOrCreateIngredient("Ing_Oolong", "우롱차엽", 45, 3));
        list.Add(GetOrCreateIngredient("Ing_Jasmine", "자스민", 50, 4));
        list.Add(GetOrCreateIngredient("Ing_Peach", "복숭아청", 38, 4));
        list.Add(GetOrCreateIngredient("Ing_Mint", "박하잎", 28, 4));

        // 후반 6종 -> 총 22종
        list.Add(GetOrCreateIngredient("Ing_Lotus", "백연꽃잎", 70, 5));
        list.Add(GetOrCreateIngredient("Ing_Ginseng", "6년근 산삼", 140, 6));
        list.Add(GetOrCreateIngredient("Ing_DeerAntler", "녹용", 260, 7));
        list.Add(GetOrCreateIngredient("Ing_GoldLeaf", "식용 금박", 400, 8));
        list.Add(GetOrCreateIngredient("Ing_SnowTea", "설산차엽", 150, 8));
        list.Add(GetOrCreateIngredient("Ing_Orchid", "난초향", 200, 9));

        return list;
    }

    private static IngredientData GetOrCreateIngredient(string id, string nameStr, int buyPrice, int unlockLevel)
    {
        string path = $"{INGREDIENTS_FOLDER}/{id}.asset";
        IngredientData asset = AssetDatabase.LoadAssetAtPath<IngredientData>(path);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<IngredientData>();
            AssetDatabase.CreateAsset(asset, path);
        }

        asset.ingredientId = id;
        asset.displayName = nameStr;
        asset.buyPrice = buyPrice;
        asset.unlockLevel = unlockLevel;
        asset.localizationKey = $"ingredient_{id.ToLower()}";

        EditorUtility.SetDirty(asset);
        return asset;
    }

    private static List<DrinkData> CreateDrinks()
    {
        List<DrinkData> list = new List<DrinkData>();

        // 초반 10종
        list.Add(GetOrCreateDrink("Drink_YuzuTea", "유자차", 42));
        list.Add(GetOrCreateDrink("Drink_MatchaLatte", "말차라떼", 48));
        list.Add(GetOrCreateDrink("Drink_HoneyYuzu", "꿀유자차", 55));
        list.Add(GetOrCreateDrink("Drink_GingerTea", "생강차", 48));
        list.Add(GetOrCreateDrink("Drink_JujubeTea", "대추차", 45));
        list.Add(GetOrCreateDrink("Drink_Ssanghwa", "쌍화차", 60));
        list.Add(GetOrCreateDrink("Drink_PlumTea", "매실차", 50));
        list.Add(GetOrCreateDrink("Drink_BarleyTea", "보리차", 20));
        list.Add(GetOrCreateDrink("Drink_HoneyGinger", "꿀생강차", 58));
        list.Add(GetOrCreateDrink("Drink_MatchaTea", "순수말차", 42));

        // 중반 12종
        list.Add(GetOrCreateDrink("Drink_PuerhTea", "보이차", 85));
        list.Add(GetOrCreateDrink("Drink_MilkTea", "밀크티", 95));
        list.Add(GetOrCreateDrink("Drink_ChrysanthemumTea", "국화차", 110));
        list.Add(GetOrCreateDrink("Drink_OolongTea", "우롱차", 120));
        list.Add(GetOrCreateDrink("Drink_JasmineTea", "자스민차", 130));
        list.Add(GetOrCreateDrink("Drink_PeachOolong", "복숭아 우롱차", 145));
        list.Add(GetOrCreateDrink("Drink_MintMatcha", "박하 말차", 100));
        list.Add(GetOrCreateDrink("Drink_HoneyMilkTea", "꿀 밀크티", 125));
        list.Add(GetOrCreateDrink("Drink_ChrysoMilk", "국화 밀크티", 135));
        list.Add(GetOrCreateDrink("Drink_SpicedCeylon", "향신 홍차", 140));
        list.Add(GetOrCreateDrink("Drink_PeachPlum", "복숭아 매실차", 115));
        list.Add(GetOrCreateDrink("Drink_JasmineMatcha", "자스민 말차", 150));

        // 후반 10종 -> 총 32종
        list.Add(GetOrCreateDrink("Drink_LotusTea", "연화차", 210));
        list.Add(GetOrCreateDrink("Drink_GinsengTea", "인삼차", 320));
        list.Add(GetOrCreateDrink("Drink_DeerElixir", "녹용 영약차", 480));
        list.Add(GetOrCreateDrink("Drink_GoldenGinseng", "황금 인삼차", 650));
        list.Add(GetOrCreateDrink("Drink_SnowMountain", "설산 설차", 520));
        list.Add(GetOrCreateDrink("Drink_OrchidLotus", "난초 연화차", 580));
        list.Add(GetOrCreateDrink("Drink_RoyalBlend", "황실 특제차", 850));
        list.Add(GetOrCreateDrink("Drink_ImperialGold", "황제 전용 금박차", 850));
        list.Add(GetOrCreateDrink("Drink_immortalElixir", "불로장생차", 1100));
        list.Add(GetOrCreateDrink("Drink_HeavenlyHarmony", "천상 조화차", 2000));

        return list;
    }

    private static DrinkData GetOrCreateDrink(string id, string nameStr, int price)
    {
        string path = $"{DRINKS_FOLDER}/{id}.asset";
        DrinkData asset = AssetDatabase.LoadAssetAtPath<DrinkData>(path);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<DrinkData>();
            AssetDatabase.CreateAsset(asset, path);
        }

        asset.drinkId = id;
        asset.displayName = nameStr;
        asset.price = price;

        EditorUtility.SetDirty(asset);
        return asset;
    }

    private static List<RecipeData> CreateRecipes(List<IngredientData> ingredients, List<DrinkData> drinks)
    {
        List<RecipeData> list = new List<RecipeData>();
        var ingMap = new Dictionary<string, IngredientData>();
        foreach (var i in ingredients) ingMap[i.ingredientId] = i;

        var drinkMap = new Dictionary<string, DrinkData>();
        foreach (var d in drinks) drinkMap[d.drinkId] = d;

        // 초반 레시피 10종
        list.Add(GetOrCreateRecipe(drinkMap["Drink_YuzuTea"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_Yuzu"], amount = 2 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_MatchaLatte"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_Matcha"], amount = 1 }, new IngredientRequirement { ingredient = ingMap["Ing_Milk"], amount = 1 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_HoneyYuzu"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_Yuzu"], amount = 1 }, new IngredientRequirement { ingredient = ingMap["Ing_Honey"], amount = 1 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_GingerTea"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_Ginger"], amount = 2 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_JujubeTea"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_Jujube"], amount = 2 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_Ssanghwa"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_Ginger"], amount = 1 }, new IngredientRequirement { ingredient = ingMap["Ing_Jujube"], amount = 1 }, new IngredientRequirement { ingredient = ingMap["Ing_Cinnamon"], amount = 1 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_PlumTea"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_Plum"], amount = 2 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_BarleyTea"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_Barley"], amount = 2 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_HoneyGinger"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_Ginger"], amount = 1 }, new IngredientRequirement { ingredient = ingMap["Ing_Honey"], amount = 1 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_MatchaTea"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_Matcha"], amount = 2 } }));

        // 중반 레시피 12종
        list.Add(GetOrCreateRecipe(drinkMap["Drink_PuerhTea"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_Puerh"], amount = 1 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_MilkTea"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_Ceylon"], amount = 1 }, new IngredientRequirement { ingredient = ingMap["Ing_Milk"], amount = 2 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_ChrysanthemumTea"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_Chrysanthemum"], amount = 2 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_OolongTea"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_Oolong"], amount = 1 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_JasmineTea"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_Jasmine"], amount = 1 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_PeachOolong"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_Oolong"], amount = 1 }, new IngredientRequirement { ingredient = ingMap["Ing_Peach"], amount = 1 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_MintMatcha"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_Matcha"], amount = 1 }, new IngredientRequirement { ingredient = ingMap["Ing_Mint"], amount = 1 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_HoneyMilkTea"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_Ceylon"], amount = 1 }, new IngredientRequirement { ingredient = ingMap["Ing_Milk"], amount = 1 }, new IngredientRequirement { ingredient = ingMap["Ing_Honey"], amount = 1 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_ChrysoMilk"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_Chrysanthemum"], amount = 1 }, new IngredientRequirement { ingredient = ingMap["Ing_Milk"], amount = 1 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_SpicedCeylon"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_Ceylon"], amount = 1 }, new IngredientRequirement { ingredient = ingMap["Ing_Cinnamon"], amount = 1 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_PeachPlum"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_Peach"], amount = 1 }, new IngredientRequirement { ingredient = ingMap["Ing_Plum"], amount = 1 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_JasmineMatcha"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_Jasmine"], amount = 1 }, new IngredientRequirement { ingredient = ingMap["Ing_Matcha"], amount = 1 } }));

        // 후반 레시피 10종 -> 총 32종
        list.Add(GetOrCreateRecipe(drinkMap["Drink_LotusTea"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_Lotus"], amount = 1 }, new IngredientRequirement { ingredient = ingMap["Ing_Honey"], amount = 1 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_GinsengTea"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_Ginseng"], amount = 1 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_DeerElixir"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_DeerAntler"], amount = 1 }, new IngredientRequirement { ingredient = ingMap["Ing_Jujube"], amount = 1 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_GoldenGinseng"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_Ginseng"], amount = 1 }, new IngredientRequirement { ingredient = ingMap["Ing_GoldLeaf"], amount = 1 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_SnowMountain"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_SnowTea"], amount = 1 }, new IngredientRequirement { ingredient = ingMap["Ing_Mint"], amount = 1 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_OrchidLotus"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_Orchid"], amount = 1 }, new IngredientRequirement { ingredient = ingMap["Ing_Lotus"], amount = 1 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_RoyalBlend"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_Ginseng"], amount = 1 }, new IngredientRequirement { ingredient = ingMap["Ing_Lotus"], amount = 1 }, new IngredientRequirement { ingredient = ingMap["Ing_Honey"], amount = 1 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_ImperialGold"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_GoldLeaf"], amount = 1 }, new IngredientRequirement { ingredient = ingMap["Ing_DeerAntler"], amount = 1 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_immortalElixir"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_Ginseng"], amount = 1 }, new IngredientRequirement { ingredient = ingMap["Ing_DeerAntler"], amount = 1 }, new IngredientRequirement { ingredient = ingMap["Ing_GoldLeaf"], amount = 1 } }));
        list.Add(GetOrCreateRecipe(drinkMap["Drink_HeavenlyHarmony"], new List<IngredientRequirement> { new IngredientRequirement { ingredient = ingMap["Ing_SnowTea"], amount = 1 }, new IngredientRequirement { ingredient = ingMap["Ing_Orchid"], amount = 1 }, new IngredientRequirement { ingredient = ingMap["Ing_GoldLeaf"], amount = 1 } }));

        return list;
    }

    private static RecipeData GetOrCreateRecipe(DrinkData drink, List<IngredientRequirement> reqs)
    {
        string id = $"Recipe_{drink.drinkId}";
        string path = $"{RECIPES_FOLDER}/{id}.asset";
        RecipeData asset = AssetDatabase.LoadAssetAtPath<RecipeData>(path);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<RecipeData>();
            AssetDatabase.CreateAsset(asset, path);
        }

        asset.targetDrink = drink;
        asset.requiredIngredients = reqs;

        EditorUtility.SetDirty(asset);
        return asset;
    }

    private static List<CustomerData> CreateCustomers(List<DrinkData> drinks)
    {
        List<CustomerData> list = new List<CustomerData>();
        var drinkMap = new Dictionary<string, DrinkData>();
        foreach (var d in drinks) drinkMap[d.drinkId] = d;

        // 20종 손님 데이터
        list.Add(GetOrCreateCustomer("Cust_Villager", "마을주민", drinkMap["Drink_YuzuTea"].drinkId));
        list.Add(GetOrCreateCustomer("Cust_Farmer", "농부", drinkMap["Drink_BarleyTea"].drinkId));
        list.Add(GetOrCreateCustomer("Cust_Woodcutter", "나무꾼", drinkMap["Drink_GingerTea"].drinkId));
        list.Add(GetOrCreateCustomer("Cust_Herbalist", "약초꾼", drinkMap["Drink_JujubeTea"].drinkId));
        list.Add(GetOrCreateCustomer("Cust_Scholar", "이선비", drinkMap["Drink_PuerhTea"].drinkId));
        list.Add(GetOrCreateCustomer("Cust_Merchant", "김상인", drinkMap["Drink_MilkTea"].drinkId));
        list.Add(GetOrCreateCustomer("Cust_Artist", "박화공", drinkMap["Drink_ChrysanthemumTea"].drinkId));
        list.Add(GetOrCreateCustomer("Cust_Poet", "최시인", drinkMap["Drink_JasmineTea"].drinkId));
        list.Add(GetOrCreateCustomer("Cust_Doctor", "의원", drinkMap["Drink_Ssanghwa"].drinkId));
        list.Add(GetOrCreateCustomer("Cust_Monk", "스님", drinkMap["Drink_OolongTea"].drinkId));
        list.Add(GetOrCreateCustomer("Cust_Officer", "포도대장", drinkMap["Drink_MatchaLatte"].drinkId));
        list.Add(GetOrCreateCustomer("Cust_Traveler", "나그네", drinkMap["Drink_PeachOolong"].drinkId));
        list.Add(GetOrCreateCustomer("Cust_CourtLady", "상궁", drinkMap["Drink_LotusTea"].drinkId));
        list.Add(GetOrCreateCustomer("Cust_General", "장군", drinkMap["Drink_GinsengTea"].drinkId));
        list.Add(GetOrCreateCustomer("Cust_Noble", "최양반", drinkMap["Drink_RoyalBlend"].drinkId));
        list.Add(GetOrCreateCustomer("Cust_Envoy", "청나라 사신", drinkMap["Drink_ImperialGold"].drinkId));
        list.Add(GetOrCreateCustomer("Cust_Master", "도인", drinkMap["Drink_SnowMountain"].drinkId));
        list.Add(GetOrCreateCustomer("Cust_Princess", "공주", drinkMap["Drink_OrchidLotus"].drinkId));
        list.Add(GetOrCreateCustomer("Cust_King", "임금님", drinkMap["Drink_immortalElixir"].drinkId));
        list.Add(GetOrCreateCustomer("Cust_Sage", "신선", drinkMap["Drink_HeavenlyHarmony"].drinkId));

        return list;
    }

    private static CustomerData GetOrCreateCustomer(string id, string nameStr, string favoriteDrinkId)
    {
        string path = $"{CUSTOMERS_FOLDER}/{id}.asset";
        CustomerData asset = AssetDatabase.LoadAssetAtPath<CustomerData>(path);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<CustomerData>();
            AssetDatabase.CreateAsset(asset, path);
        }

        asset.customerId = id;
        asset.customerName = nameStr;
        asset.preferredTea = favoriteDrinkId;

        EditorUtility.SetDirty(asset);
        return asset;
    }

    private static void RegisterDatabases(List<IngredientData> ingredients, List<DrinkData> drinks, List<RecipeData> recipes, List<CustomerData> customers)
    {
        // IngredientDatabase
        string ingDbPath = $"{INGREDIENTS_FOLDER}/IngredientDatabase.asset";
        IngredientDatabase ingDb = AssetDatabase.LoadAssetAtPath<IngredientDatabase>(ingDbPath);
        if (ingDb == null)
        {
            ingDb = ScriptableObject.CreateInstance<IngredientDatabase>();
            AssetDatabase.CreateAsset(ingDb, ingDbPath);
        }
        SetDatabaseField(ingDb, "_ingredients", ingredients);
        EditorUtility.SetDirty(ingDb);

        // DrinkDatabase
        string drinkDbPath = $"{DRINKS_FOLDER}/DrinkDatabase.asset";
        DrinkDatabase drinkDb = AssetDatabase.LoadAssetAtPath<DrinkDatabase>(drinkDbPath);
        if (drinkDb == null)
        {
            drinkDb = ScriptableObject.CreateInstance<DrinkDatabase>();
            AssetDatabase.CreateAsset(drinkDb, drinkDbPath);
        }
        SetDatabaseField(drinkDb, "_drinks", drinks);
        EditorUtility.SetDirty(drinkDb);

        // RecipeDatabase
        string recipeDbPath = $"{RECIPES_FOLDER}/RecipeDatabase.asset";
        RecipeDatabase recipeDb = AssetDatabase.LoadAssetAtPath<RecipeDatabase>(recipeDbPath);
        if (recipeDb == null)
        {
            recipeDb = ScriptableObject.CreateInstance<RecipeDatabase>();
            AssetDatabase.CreateAsset(recipeDb, recipeDbPath);
        }
        SetDatabaseField(recipeDb, "_recipes", recipes);
        EditorUtility.SetDirty(recipeDb);

        // CustomerDatabase
        string custDbPath = $"{CUSTOMERS_FOLDER}/CustomerDatabase.asset";
        CustomerDatabase custDb = AssetDatabase.LoadAssetAtPath<CustomerDatabase>(custDbPath);
        if (custDb == null)
        {
            custDb = ScriptableObject.CreateInstance<CustomerDatabase>();
            AssetDatabase.CreateAsset(custDb, custDbPath);
        }
        SetDatabaseField(custDb, "_customers", customers);
        EditorUtility.SetDirty(custDb);
    }

    private static void SetDatabaseField<TDb, TItem>(TDb db, string fieldName, List<TItem> list) where TDb : ScriptableObject
    {
        var field = typeof(TDb).GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        if (field != null)
        {
            field.SetValue(db, list);
        }
    }

    private static void EnsureFolder(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string parent = Path.GetDirectoryName(path).Replace('\\', '/');
            string folderName = Path.GetFileName(path);
            AssetDatabase.CreateFolder(parent, folderName);
        }
    }
}
#endif
