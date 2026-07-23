using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BrewingView 이벤트를 수신하여 BrewingManager/RecipeDatabase/InventoryManager와 통신하는 Controller 클래스입니다.
/// </summary>
public class BrewingController : MonoBehaviour
{
    [SerializeField] private BrewingView view;
    [SerializeField] private RecipeDatabase recipeDatabase;

    // Localization Key 상수 (하드코딩 문자열 제거)
    private const string LOC_KEY_STATUS_READY = "brewing_status_ready";
    private const string LOC_KEY_STATUS_BREWING = "brewing_status_brewing";
    private const string LOC_KEY_STATUS_COMPLETE = "brewing_status_complete";
    private const string LOC_KEY_STATUS_NO_INGREDIENTS = "brewing_status_no_ingredients";

    private string _selectedRecipeId = string.Empty;

    private void Awake()
    {
        if (view == null)
        {
            Debug.LogError("[BrewingController] view(BrewingView) 참조가 할당되지 않았습니다. Inspector를 확인해 주세요.");
        }

        if (recipeDatabase == null)
        {
            Debug.LogError("[BrewingController] recipeDatabase 참조가 할당되지 않았습니다. Inspector를 확인해 주세요.");
        }
    }

    private void OnEnable()
    {
        if (view != null)
        {
            view.OnSelectRecipeClicked += HandleSelectRecipeClicked;
            view.OnStartBrewClicked += HandleStartBrewClicked;
        }

        if (BrewingManager.Instance != null)
        {
            BrewingManager.Instance.OnBrewingProgressChanged += HandleBrewingProgressChanged;
            BrewingManager.Instance.OnBrewingComplete += HandleBrewingComplete;
        }

        RefreshAll();
    }

    private void OnDisable()
    {
        if (view != null)
        {
            view.OnSelectRecipeClicked -= HandleSelectRecipeClicked;
            view.OnStartBrewClicked -= HandleStartBrewClicked;
        }

        if (BrewingManager.Instance != null)
        {
            BrewingManager.Instance.OnBrewingProgressChanged -= HandleBrewingProgressChanged;
            BrewingManager.Instance.OnBrewingComplete -= HandleBrewingComplete;
        }
    }

    /// <summary>
    /// 초기 동기화
    /// </summary>
    private void RefreshAll()
    {
        if (view == null) return;

        view.SetProgressValue(0f);
        view.SetStartButtonInteractable(false);
        view.SetStatusText(GetLocalizedText(LOC_KEY_STATUS_READY));
        view.SetIngredientsText(string.Empty);
    }

    // ──────────────────────────────────────────────────────────────────────
    // View 이벤트 핸들러
    // ──────────────────────────────────────────────────────────────────────

    private void HandleSelectRecipeClicked()
    {
        if (recipeDatabase == null) return;

        // 현재는 전체 레시피 중 첫 번째를 순환 선택하는 MVP 구조
        // 향후 레시피 선택 UI(그리드/드롭다운)로 교체 가능
        IReadOnlyList<RecipeData> allRecipes = recipeDatabase.GetAllRecipes();
        if (allRecipes == null || allRecipes.Count == 0) return;

        // 순환 선택
        int currentIndex = FindCurrentRecipeIndex(allRecipes);
        int nextIndex = (currentIndex + 1) % allRecipes.Count;

        RecipeData selectedRecipe = allRecipes[nextIndex];
        if (selectedRecipe == null || selectedRecipe.targetDrink == null) return;

        _selectedRecipeId = selectedRecipe.targetDrink.drinkId;

        // BrewingManager에 차 선택 전달
        if (BrewingManager.Instance != null)
        {
            BrewingManager.Instance.SelectTea(_selectedRecipeId);
        }

        // 재료 텍스트 조합 및 버튼 활성화 갱신
        UpdateIngredientsDisplay(selectedRecipe);
    }

    private void HandleStartBrewClicked()
    {
        if (BrewingManager.Instance == null) return;
        if (string.IsNullOrEmpty(_selectedRecipeId)) return;

        bool success = BrewingManager.Instance.StartBrewingWithValidation();
        if (success)
        {
            if (view != null)
            {
                view.SetStatusText(GetLocalizedText(LOC_KEY_STATUS_BREWING));
                view.SetStartButtonInteractable(false);
                view.SetProgressValue(0f);
            }
        }
    }

    // ──────────────────────────────────────────────────────────────────────
    // BrewingManager 이벤트 핸들러
    // ──────────────────────────────────────────────────────────────────────

    private void HandleBrewingProgressChanged(float progress)
    {
        if (view != null)
        {
            view.SetProgressValue(progress);
        }
    }

    private void HandleBrewingComplete()
    {
        if (view != null)
        {
            view.SetProgressValue(1f);
            view.SetStatusText(GetLocalizedText(LOC_KEY_STATUS_COMPLETE));
            view.SetStartButtonInteractable(false);
        }
    }

    // ──────────────────────────────────────────────────────────────────────
    // 헬퍼
    // ──────────────────────────────────────────────────────────────────────

    /// <summary>
    /// 선택된 레시피의 재료 텍스트를 조합하고 Start 버튼 활성화 여부를 결정합니다.
    /// </summary>
    private void UpdateIngredientsDisplay(RecipeData recipe)
    {
        if (view == null || recipe == null) return;

        bool canBrew = true;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        foreach (var req in recipe.requiredIngredients)
        {
            if (req.ingredient == null) continue;

            int owned = 0;
            if (InventoryManager.Instance != null)
            {
                owned = InventoryManager.Instance.GetIngredientCount(req.ingredient.ingredientId);
            }

            bool enough = owned >= req.amount;
            if (!enough) canBrew = false;

            // "유자: 2/3" 형태로 조합
            sb.AppendLine($"{req.ingredient.displayName}: {owned}/{req.amount}");
        }

        view.SetIngredientsText(sb.ToString().TrimEnd());
        view.SetStartButtonInteractable(canBrew);

        if (!canBrew)
        {
            view.SetStatusText(GetLocalizedText(LOC_KEY_STATUS_NO_INGREDIENTS));
        }
        else
        {
            view.SetStatusText(GetLocalizedText(LOC_KEY_STATUS_READY));
        }
    }

    /// <summary>
    /// 현재 선택된 레시피의 인덱스를 찾습니다.
    /// </summary>
    private int FindCurrentRecipeIndex(IReadOnlyList<RecipeData> allRecipes)
    {
        if (string.IsNullOrEmpty(_selectedRecipeId)) return -1;

        for (int i = 0; i < allRecipes.Count; i++)
        {
            if (allRecipes[i] != null && allRecipes[i].targetDrink != null
                && allRecipes[i].targetDrink.drinkId == _selectedRecipeId)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// LocalizationManager에서 로컬라이즈된 문자열을 가져옵니다.
    /// 매니저가 없거나 키가 등록되지 않은 경우 키 값을 그대로 반환합니다.
    /// </summary>
    private string GetLocalizedText(string key)
    {
        if (LocalizationManager.Instance != null)
        {
            string text = LocalizationManager.Instance.GetText(key);
            if (!string.IsNullOrEmpty(text))
                return text;
        }
        // Fallback: 키 자체를 반환
        return key;
    }
}
