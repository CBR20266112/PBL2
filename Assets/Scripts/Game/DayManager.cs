using UnityEngine;

public enum DayState
{
    Ready,       // 영업 준비
    Operating,   // 영업 중
    Settlement   // 영업 결산
}

/// <summary>
/// 게임의 하루 주기(영업 준비, 영업 중, 영업 결산) 상태와 하루 통계를 관리하는 클래스입니다.
/// </summary>
public class DayManager : Singleton<DayManager>
{
    [Header("설정")]
    [Tooltip("시작 일차 설정")]
    [SerializeField] private int startDayNumber = 1;

    // 현재 진행 중인 일차
    public int CurrentDayNumber { get; private set; } = 1;

    // 현재 하루의 상태
    public DayState CurrentState { get; private set; } = DayState.Ready;

    // 하루 통계 데이터
    public int DailyEarnings { get; private set; } = 0;
    public int DailyServedCustomers { get; private set; } = 0;

    // 상태 변경 이벤트 (UI 갱신 또는 외부 매니저 연동용)
    public delegate void DayStateChangedHandler(DayState newState);
    public event DayStateChangedHandler OnDayStateChanged;

    public delegate void DayNumberChangedHandler(int newDayNumber);
    public event DayNumberChangedHandler OnDayNumberChanged;

    public delegate void DailyEarningsChangedHandler(int newEarnings);
    public event DailyEarningsChangedHandler OnDailyEarningsChanged;

    public delegate void DailyServedCustomersChangedHandler(int newCount);
    public event DailyServedCustomersChangedHandler OnDailyServedCustomersChanged;

    protected override void Awake()
    {
        base.Awake();
        if (Instance == this)
        {
            CurrentDayNumber = startDayNumber;
        }
    }

    private void Start()
    {
        // 첫 진입 시 준비 상태 설정 및 상점 활성화
        EnterState(DayState.Ready);
    }

    /// <summary>
    /// 영업을 시작합니다. (Ready -> Operating)
    /// </summary>
    public void StartDay()
    {
        if (CurrentState != DayState.Ready) return;
        EnterState(DayState.Operating);
    }

    /// <summary>
    /// 영업을 종료하고 결산 단계를 시작합니다. (Operating -> Settlement)
    /// </summary>
    public void EndDay()
    {
        if (CurrentState != DayState.Operating) return;
        EnterState(DayState.Settlement);
    }

    /// <summary>
    /// 다음날 영업 준비 단계로 진입하며 일차를 증가시킵니다. (Settlement -> Ready)
    /// </summary>
    public void AdvanceToNextDay()
    {
        if (CurrentState != DayState.Settlement) return;

        CurrentDayNumber++;
        OnDayNumberChanged?.Invoke(CurrentDayNumber);

        EnterState(DayState.Ready);
    }

    /// <summary>
    /// 하루 매출(냥전)을 누적 기록합니다.
    /// </summary>
    public void RecordSale(int amount)
    {
        if (CurrentState != DayState.Operating) return;
        if (amount < 0) return;

        DailyEarnings += amount;
        OnDailyEarningsChanged?.Invoke(DailyEarnings);
    }

    /// <summary>
    /// 하루 중 서빙 성공한 손님 수를 누적 기록합니다.
    /// </summary>
    public void RecordCustomerServed()
    {
        if (CurrentState != DayState.Operating) return;

        DailyServedCustomers++;
        OnDailyServedCustomersChanged?.Invoke(DailyServedCustomers);
    }

    /// <summary>
    /// 상태 변경에 따른 시스템 제어 및 이벤트 처리
    /// </summary>
    private void EnterState(DayState newState)
    {
        CurrentState = newState;

        switch (CurrentState)
        {
            case DayState.Ready:
                // 영업 준비 시 당일 통계 초기화 및 상점 오픈
                DailyEarnings = 0;
                DailyServedCustomers = 0;
                OnDailyEarningsChanged?.Invoke(DailyEarnings);
                OnDailyServedCustomersChanged?.Invoke(DailyServedCustomers);
                
                if (ShopManager.Instance != null)
                {
                    ShopManager.Instance.SetShopOpen(true);
                }
                break;

            case DayState.Operating:
                // 영업 개시 시 상점 마감
                if (ShopManager.Instance != null)
                {
                    ShopManager.Instance.SetShopOpen(false);
                }
                break;

            case DayState.Settlement:
                // 영업 종료 결산 시 세이브 시스템 호출
                TriggerAutoSave();
                break;
        }

        OnDayStateChanged?.Invoke(CurrentState);
    }

    /// <summary>
    /// 결산 시점에 자동 저장을 호출하는 트리거 훅입니다.
    /// </summary>
    private void TriggerAutoSave()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame();
        }
    }

    // ──────────────────────────────────────────────────────────────────────
    // 세이브 데이터 제공 책임
    // ──────────────────────────────────────────────────────────────────────

    public DaySaveData GetSaveData()
    {
        return new DaySaveData { currentDayNumber = CurrentDayNumber };
    }

    public void LoadSaveData(DaySaveData saveData)
    {
        if (saveData == null) return;
        CurrentDayNumber = saveData.currentDayNumber;
        OnDayNumberChanged?.Invoke(CurrentDayNumber);
    }
}

// ──────────────────────────────────────────────────────────────────────
// 세이브/로드 연동을 위한 데이터 컨테이너 정의
// ──────────────────────────────────────────────────────────────────────

[System.Serializable]
public class DaySaveData
{
    public int currentDayNumber;
}
