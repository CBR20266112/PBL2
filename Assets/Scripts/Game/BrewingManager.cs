using UnityEngine;

// DrinkDatabase, RecipeDatabase는 Inspector에서 연결합니다.

/// <summary>
/// 차 제조 데이터
/// 온도, 우림시간, 재료 등
/// </summary>
public class BrewingData
{
    public string selectedTea = "yuzu"; // 선택한 차
    public int temperature = 1; // 0=low, 1=medium, 2=high
    public int steepTime = 1; // 0=short, 1=medium, 2=long
    public float brewingProgress = 0f; // 0~1
    public int brewingQuality = 3; // 1~5 별점 (기본값 3)

    public void Reset()
    {
        selectedTea = "yuzu";
        temperature = 1;
        steepTime = 1;
        brewingProgress = 0f;
        brewingQuality = 3;
    }
}

/// <summary>
/// 차 제조 매니저
/// </summary>
public class BrewingManager : Singleton<BrewingManager>
{
    // ──────────────────────────────────────────────────────────────────────
    // 데이터베이스 연결 (Inspector에서 에셋을 드래그 앤 드롭으로 연결)
    // ──────────────────────────────────────────────────────────────────────
    [Header("데이터베이스 연결")]
    [Tooltip("DrinkDatabase 에셋을 연결합니다.")]
    [SerializeField] private DrinkDatabase _drinkDatabase;

    [Tooltip("RecipeDatabase 에셋을 연결합니다.")]
    [SerializeField] private RecipeDatabase _recipeDatabase;

    // ──────────────────────────────────────────────────────────────────────
    // 상태
    // ──────────────────────────────────────────────────────────────────────
    public BrewingData CurrentBrewingData { get; private set; } = new BrewingData();

    // 제조 완료 이벤트
    public delegate void BrewingCompleteHandler();
    public event BrewingCompleteHandler OnBrewingComplete;

    // 제조 진행도 변경 이벤트
    public event System.Action<float> OnBrewingProgressChanged;

    private float _brewingDuration = 5f; // 기본 우림시간 5초
    private float _brewingTimer = 0f;
    private bool _isBrewing = false;

    private void Update()
    {
        if (!_isBrewing) return;

        _brewingTimer += Time.deltaTime;
        CurrentBrewingData.brewingProgress = Mathf.Clamp01(_brewingTimer / _brewingDuration);
        OnBrewingProgressChanged?.Invoke(CurrentBrewingData.brewingProgress);

        if (_brewingTimer >= _brewingDuration)
        {
            _isBrewing = false;
            _brewingTimer = 0f;
            
            // 평가 계산
            CalculateQuality();
            
            Debug.Log($"Brewing complete! Quality: {CurrentBrewingData.brewingQuality}");
            
            // 제조 완료 이벤트 발생
            OnBrewingComplete?.Invoke();
        }
    }

    /// <summary>
    /// 차 제조 시작
    /// </summary>
    public void StartBrewing()
    {
        _brewingTimer = 0f;
        _isBrewing = true;
        Debug.Log($"Brewing started: {CurrentBrewingData.selectedTea} at {CurrentBrewingData.temperature} temp, {CurrentBrewingData.steepTime} steep");
    }

    /// <summary>
    /// 차 제조 중인가
    /// </summary>
    public bool IsBrewing()
    {
        return _isBrewing;
    }

    /// <summary>
    /// 현재 선택된 차의 레시피가 존재하고 재료 재고가 충분한지 검증합니다.
    /// 데이터베이스나 인벤토리 매니저가 연결되지 않은 경우에는 항상 true를 반환하여 기존 동작을 유지합니다.
    /// </summary>
    /// <param name="failReason">제조 불가 시 사유. 성공하면 빈 문자열.</param>
    /// <returns>제조 가능 여부</returns>
    public bool CanBrew(out string failReason)
    {
        if (_recipeDatabase == null)
        {
            // DB가 연결되지 않은 경우 기존 동작과 동일하게 항상 허용
            failReason = string.Empty;
            return true;
        }

        RecipeData recipe = _recipeDatabase.GetRecipe(CurrentBrewingData.selectedTea);
        if (recipe == null)
        {
            failReason = $"'{CurrentBrewingData.selectedTea}'에 해당하는 레시피가 등록되어 있지 않습니다.";
            return false;
        }

        // 인벤토리 매니저를 통한 요구 재료 재고 검증
        if (InventoryManager.Instance != null)
        {
            foreach (var req in recipe.requiredIngredients)
            {
                if (req.ingredient == null) continue;
                if (!InventoryManager.Instance.HasEnoughIngredient(req.ingredient.ingredientId, req.amount))
                {
                    failReason = $"재료 부족: {req.ingredient.displayName} (필요: {req.amount}개)";
                    return false;
                }
            }
        }

        failReason = string.Empty;
        return true;
    }

    /// <summary>
    /// 레시피 유효성을 검증한 뒤 제조를 시작합니다.
    /// 레시피가 없으면 제조를 시작하지 않고 false를 반환합니다.
    /// 기존 StartBrewing()은 수정하지 않습니다.
    /// </summary>
    /// <returns>제조 시작 성공 여부</returns>
    public bool StartBrewingWithValidation()
    {
        if (!CanBrew(out string failReason))
        {
            Debug.LogWarning($"[BrewingManager] 제조 불가: {failReason}");
            return false;
        }

        StartBrewing();
        return true;
    }

    /// <summary>
    /// 차 선택
    /// </summary>
    public void SelectTea(string teaName)
    {
        CurrentBrewingData.selectedTea = teaName;
        Debug.Log($"Tea selected: {teaName}");
    }

    /// <summary>
    /// 온도 설정
    /// </summary>
    public void SetTemperature(int temp)
    {
        CurrentBrewingData.temperature = Mathf.Clamp(temp, 0, 2);
        Debug.Log($"Temperature set to: {GetTemperatureLabel(CurrentBrewingData.temperature)}");
    }

    /// <summary>
    /// 우림시간 설정
    /// </summary>
    public void SetSteepTime(int time)
    {
        CurrentBrewingData.steepTime = Mathf.Clamp(time, 0, 2);
        Debug.Log($"Steep time set to: {GetSteepTimeLabel(CurrentBrewingData.steepTime)}");
    }

    /// <summary>
    /// 품질 설정
    /// </summary>
    public void SetQuality(int quality)
    {
        CurrentBrewingData.brewingQuality = Mathf.Clamp(quality, 1, 5);
    }

    /// <summary>
    /// 평가 계산 (손님 선호도와 플레이어 선택 비교)
    /// </summary>
    private void CalculateQuality()
    {
        Customer customer = CustomerManager.Instance.GetCurrentCustomer();
        if (customer == null)
        {
            CurrentBrewingData.brewingQuality = 3;
            return;
        }

        CurrentBrewingData.brewingQuality = customer.GetRating(
            CurrentBrewingData.selectedTea,
            CurrentBrewingData.temperature,
            CurrentBrewingData.steepTime
        );
    }

    /// <summary>
    /// 온도 레이블
    /// </summary>
    public static string GetTemperatureLabel(int temp)
    {
        return temp switch
        {
            0 => "낮음",
            1 => "중간",
            2 => "높음",
            _ => "중간"
        };
    }

    /// <summary>
    /// 우림시간 레이블
    /// </summary>
    public static string GetSteepTimeLabel(int time)
    {
        return time switch
        {
            0 => "짧음",
            1 => "중간",
            2 => "길음",
            _ => "중간"
        };
    }

    /// <summary>
    /// 차 이름 레이블
    /// </summary>
    public static string GetTeaLabel(string teaName)
    {
        return teaName switch
        {
            "yuzu" => "유자차",
            "matcha" => "말차",
            "puerh" => "보이차",
            "lotus" => "연꽃차",
            "chai" => "차이",
            _ => "알 수 없는 차"
        };
    }

    /// <summary>
    /// 제조 초기화
    /// </summary>
    public void ResetBrewing()
    {
        _isBrewing = false;
        _brewingTimer = 0f;
        CurrentBrewingData.Reset();
    }
}
