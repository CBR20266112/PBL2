using UnityEngine;

/// <summary>
/// 게임 내 재화(돈)의 획득, 소모 및 검증을 전담하는 매니저 클래스입니다.
/// 프로젝트의 유일한 재화 관리 객체로 사용됩니다.
/// 자체 저장 기능을 호출하지 않고, SaveManager 등 외부 세이브 시스템과의 연동을 위한 API만 제공합니다.
/// </summary>
public class MoneyManager : Singleton<MoneyManager>
{
    [Header("설정")]
    [Tooltip("게임 시작 시 지급되는 초기 재화 설정")]
    [SerializeField] private int startingMoney = 10000;

    // 런타임 현재 재화 수량 (Setter는 비공개)
    public int CurrentMoney { get; private set; }

    // 재화 변경 이벤트 (UI 갱신이나 효과음 재생 연동용)
    public delegate void MoneyChangedHandler(int newMoney, int delta);
    public event MoneyChangedHandler OnMoneyChanged;

    protected override void Awake()
    {
        base.Awake();
        
        // 싱글톤 초기화 성공 시에만 시작 자금 설정
        if (Instance == this)
        {
            CurrentMoney = startingMoney;
        }
    }

    /// <summary>
    /// 재화를 안전하게 추가합니다. (예: 음료 판매 수익, 보상 등)
    /// </summary>
    /// <param name="amount">추가할 수량 (양수)</param>
    public void AddMoney(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("[MoneyManager] 0 이하의 재화는 추가할 수 없습니다. 차감은 SpendMoney를 사용하세요.");
            return;
        }

        CurrentMoney += amount;
        Debug.Log($"[MoneyManager] 재화 추가: +{amount}원 (현재: {CurrentMoney}원)");
        OnMoneyChanged?.Invoke(CurrentMoney, amount);
    }

    /// <summary>
    /// 재화를 검사하고 차감합니다. (예: 재료 구매 등)
    /// </summary>
    /// <param name="amount">소모할 수량 (양수)</param>
    /// <returns>소지금이 충분하여 차감 성공 시 true 반환. 부족 시 false 반환 및 차감 진행 안 함.</returns>
    public bool SpendMoney(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("[MoneyManager] 0 이하의 재화는 소모할 수 없습니다.");
            return false;
        }

        if (!CanAfford(amount))
        {
            Debug.LogWarning($"[MoneyManager] 소지금 부족: 요구 {amount}원, 보유 {CurrentMoney}원");
            return false;
        }

        CurrentMoney -= amount;
        Debug.Log($"[MoneyManager] 재화 소모: -{amount}원 (현재: {CurrentMoney}원)");
        OnMoneyChanged?.Invoke(CurrentMoney, -amount);
        return true;
    }

    /// <summary>
    /// 지정한 금액을 지불할 수 있는 잔액이 있는지 확인합니다.
    /// </summary>
    public bool CanAfford(int amount)
    {
        return CurrentMoney >= amount;
    }

    /// <summary>
    /// 세이브 데이터 구조체에 담아 현재 재화 정보를 반환합니다.
    /// SaveManager 등 외부 세이브 시스템이 하루 종료 시 일괄 수집하는 데 사용됩니다.
    /// </summary>
    public MoneySaveData GetSaveData()
    {
        return new MoneySaveData { currentMoney = CurrentMoney };
    }

    /// <summary>
    /// 저장되었던 재화 데이터를 인계받아 런타임 상태를 복원합니다.
    /// </summary>
    public void LoadSaveData(MoneySaveData saveData)
    {
        if (saveData == null) return;
        
        int oldMoney = CurrentMoney;
        CurrentMoney = saveData.currentMoney;
        Debug.Log($"[MoneyManager] 재화 데이터 복구 완료: {CurrentMoney}원");
        OnMoneyChanged?.Invoke(CurrentMoney, CurrentMoney - oldMoney);
    }
}

// ──────────────────────────────────────────────────────────────────────
// 세이브/로드 연동을 위한 데이터 컨테이너 정의
// ──────────────────────────────────────────────────────────────────────

[System.Serializable]
public class MoneySaveData
{
    public int currentMoney;
}
