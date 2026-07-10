using UnityEngine;

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
    public BrewingData CurrentBrewingData { get; private set; } = new BrewingData();

    private float _brewingDuration = 5f; // 기본 우림시간 5초
    private float _brewingTimer = 0f;
    private bool _isBrewing = false;

    private void Update()
    {
        if (!_isBrewing) return;

        _brewingTimer += Time.deltaTime;
        CurrentBrewingData.brewingProgress = Mathf.Clamp01(_brewingTimer / _brewingDuration);

        if (_brewingTimer >= _brewingDuration)
        {
            _isBrewing = false;
            _brewingTimer = 0f;
            Debug.Log($"Brewing complete! Quality: {CurrentBrewingData.brewingQuality}");
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
