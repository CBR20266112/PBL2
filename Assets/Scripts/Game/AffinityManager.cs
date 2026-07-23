using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 친밀도 직렬화용 아이템 구조체
/// </summary>
[System.Serializable]
public struct AffinityItemSaveData
{
    public string customerId;
    public int affinityValue;
}

/// <summary>
/// 친밀도 직렬화용 세이브 컨테이너
/// </summary>
[System.Serializable]
public class AffinitySaveData
{
    public List<AffinityItemSaveData> items = new List<AffinityItemSaveData>();
}

/// <summary>
/// 손님별 친밀도(Affinity) 데이터를 저장하고 관리하는 매니저 클래스입니다.
/// 보상 지급이나 이벤트 실행은 담당하지 않으며 수치 저장/조회 및 세이브 연동만 전담합니다.
/// </summary>
public class AffinityManager : Singleton<AffinityManager>
{
    // 런타임 친밀도 저장소 (customerId -> affinityValue)
    private readonly Dictionary<string, int> _affinityMap = new Dictionary<string, int>();

    // 친밀도 변경 이벤트
    public delegate void AffinityChangedHandler(string customerId, int newAffinity, int delta);
    public event AffinityChangedHandler OnAffinityChanged;

    private void Start()
    {
        // ServingManager 이벤트 구독 (ServingManager가 전달하는 customerId 직접 활용)
        if (ServingManager.Instance != null)
        {
            ServingManager.Instance.OnServeSucceeded += OnServeSucceededHandler;
        }
    }

    private void OnDestroy()
    {
        if (ServingManager.Instance != null)
        {
            ServingManager.Instance.OnServeSucceeded -= OnServeSucceededHandler;
        }
    }

    private void OnServeSucceededHandler(string customerId, string drinkId, int earnedMoney)
    {
        if (!string.IsNullOrEmpty(customerId))
        {
            AddAffinity(customerId, 1); // 서빙 성공 시 친밀도 +1
        }
    }

    /// <summary>
    /// 지정한 손님의 친밀도를 수량만큼 증가시킵니다.
    /// </summary>
    public void AddAffinity(string customerId, int amount)
    {
        if (string.IsNullOrEmpty(customerId) || amount <= 0) return;

        int current = GetAffinity(customerId);
        SetAffinity(customerId, current + amount);
    }

    /// <summary>
    /// 지정한 손님의 친밀도를 수량만큼 감소시킵니다. (최소 0 유지)
    /// </summary>
    public void RemoveAffinity(string customerId, int amount)
    {
        if (string.IsNullOrEmpty(customerId) || amount <= 0) return;

        int current = GetAffinity(customerId);
        SetAffinity(customerId, Mathf.Max(0, current - amount));
    }

    /// <summary>
    /// 지정한 손님의 현재 친밀도 수치를 반환합니다. (기록 없으면 0)
    /// </summary>
    public int GetAffinity(string customerId)
    {
        if (string.IsNullOrEmpty(customerId)) return 0;
        _affinityMap.TryGetValue(customerId, out int value);
        return value;
    }

    /// <summary>
    /// 지정한 손님의 친밀도 수치를 명시적으로 설정합니다.
    /// </summary>
    public void SetAffinity(string customerId, int value)
    {
        if (string.IsNullOrEmpty(customerId)) return;

        int newValue = Mathf.Max(0, value);
        int oldValue = GetAffinity(customerId);
        int delta = newValue - oldValue;

        _affinityMap[customerId] = newValue;
        Debug.Log($"[AffinityManager] 친밀도 변경 - {customerId}: {oldValue} -> {newValue} (변동: {delta})");

        OnAffinityChanged?.Invoke(customerId, newValue, delta);

        // 친밀도 단계 업/이벤트 해금 확인 훅 실행
        ProcessAffinityLevelCheckHook(customerId, newValue);
    }

    /// <summary>
    /// (Hook) 친밀도 상승에 따른 단계(Level) 계산이나 스토리/업적 해금을 체크하는 훅입니다.
    /// </summary>
    private void ProcessAffinityLevelCheckHook(string customerId, int currentAffinity)
    {
        // TODO: 향후 친밀도 랭크/단계 계산 및 스토리 해금 시스템 연동
    }

    // ──────────────────────────────────────────────────────────────────────
    // SaveManager 연동 (GetSaveData / LoadSaveData)
    // ──────────────────────────────────────────────────────────────────────

    public AffinitySaveData GetSaveData()
    {
        AffinitySaveData saveData = new AffinitySaveData();
        foreach (var pair in _affinityMap)
        {
            saveData.items.Add(new AffinityItemSaveData
            {
                customerId = pair.Key,
                affinityValue = pair.Value
            });
        }
        return saveData;
    }

    public void LoadSaveData(AffinitySaveData saveData)
    {
        if (saveData == null || saveData.items == null) return;

        _affinityMap.Clear();
        foreach (var item in saveData.items)
        {
            if (!string.IsNullOrEmpty(item.customerId))
            {
                _affinityMap[item.customerId] = item.affinityValue;
            }
        }
        Debug.Log($"[AffinityManager] 친밀도 데이터 복구 완료 ({_affinityMap.Count}명 기록).");
    }
}
