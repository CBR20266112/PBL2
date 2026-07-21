using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 DrinkData를 중앙에서 관리하는 데이터베이스 (ScriptableObject).
/// ThemeDatabase와 동일한 패턴을 따릅니다.
///
/// - Inspector에서 List 편집 가능
/// - 런타임에 Dictionary로 Pre-warming하여 O(1) 조회 지원
/// - OnEnable()과 Initialize() 양쪽에서 안전하게 초기화
/// </summary>
[CreateAssetMenu(fileName = "DrinkDatabase", menuName = "Tea Culture Game/Drink Database")]
public class DrinkDatabase : ScriptableObject
{
    [Tooltip("등록된 모든 음료 데이터 목록. Setup Tool이 자동으로 채워줍니다.")]
    public List<DrinkData> drinks = new List<DrinkData>();

    private Dictionary<string, DrinkData> _drinkDict;

    // ──────────────────────────────────────────────────────────────────────
    // 초기화
    // ──────────────────────────────────────────────────────────────────────

    /// <summary>
    /// ScriptableObject가 활성화될 때 자동으로 딕셔너리를 초기화합니다.
    /// 에디터 재직렬화(Domain Reload) 이후에도 안전하게 복구됩니다.
    /// </summary>
    private void OnEnable()
    {
        Initialize();
    }

    /// <summary>
    /// 런타임 검색용 딕셔너리를 빌드합니다.
    /// AudioManager.Awake()처럼 Pre-warming이 필요한 경우 명시적으로 호출합니다.
    /// </summary>
    public void Initialize()
    {
        _drinkDict = new Dictionary<string, DrinkData>();
        foreach (var drink in drinks)
        {
            if (drink == null) continue;
            if (string.IsNullOrEmpty(drink.drinkId)) continue;
            if (_drinkDict.ContainsKey(drink.drinkId))
            {
                Debug.LogWarning($"[DrinkDatabase] 중복된 drinkId 발견: '{drink.drinkId}'. 첫 번째 항목만 등록됩니다.");
                continue;
            }
            _drinkDict.Add(drink.drinkId, drink);
        }
    }

    // ──────────────────────────────────────────────────────────────────────
    // 조회 API
    // ──────────────────────────────────────────────────────────────────────

    /// <summary>
    /// drinkId로 DrinkData를 조회합니다. 존재하지 않으면 null을 반환합니다.
    /// </summary>
    public DrinkData GetDrink(string drinkId)
    {
        EnsureInitialized();
        if (_drinkDict.TryGetValue(drinkId, out DrinkData data))
            return data;

        Debug.LogWarning($"[DrinkDatabase] drinkId '{drinkId}'를 찾을 수 없습니다.");
        return null;
    }

    /// <summary>
    /// 플레이어 레벨 기준으로 해금된 음료 목록만 반환합니다.
    /// </summary>
    public List<DrinkData> GetUnlockedDrinks(int playerLevel)
    {
        EnsureInitialized();
        var result = new List<DrinkData>();
        foreach (var drink in drinks)
        {
            if (drink != null && playerLevel >= drink.unlockLevel)
                result.Add(drink);
        }
        return result;
    }

    /// <summary>
    /// 특정 음료가 플레이어 레벨 기준으로 해금되었는지 확인합니다.
    /// </summary>
    public bool IsUnlocked(string drinkId, int playerLevel)
    {
        var drink = GetDrink(drinkId);
        return drink != null && playerLevel >= drink.unlockLevel;
    }

    /// <summary>
    /// 등록된 전체 음료 목록을 반환합니다. (읽기 전용)
    /// </summary>
    public IReadOnlyList<DrinkData> GetAllDrinks()
    {
        return drinks.AsReadOnly();
    }

    // ──────────────────────────────────────────────────────────────────────
    // 내부 유틸
    // ──────────────────────────────────────────────────────────────────────

    /// <summary>
    /// 딕셔너리가 null인 경우 안전하게 재초기화합니다.
    /// OnEnable 이후 Domain Reload 등으로 dict가 소실될 경우 대비합니다.
    /// </summary>
    private void EnsureInitialized()
    {
        if (_drinkDict == null)
            Initialize();
    }
}
