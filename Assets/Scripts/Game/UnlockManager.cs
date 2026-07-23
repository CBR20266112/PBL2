using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 해금 직렬화용 세이브 컨테이너
/// </summary>
[System.Serializable]
public class UnlockSaveData
{
    public List<string> unlockedIds = new List<string>();
}

/// <summary>
/// 게임 내 콘텐츠(음료, 재료, 가구, 손님 등)의 해금 상태를 저장하고 조회하는 매니저 클래스입니다.
/// 해금 조건 판단이나 보상 지급은 하지 않으며 순수 해금 상태 데이터만 수집/제공합니다.
/// </summary>
public class UnlockManager : Singleton<UnlockManager>
{
    // 런타임 해금 항목 집합 (ID 기반 빠른 O(1) 조회)
    private readonly HashSet<string> _unlockedIds = new HashSet<string>();

    // 해금 상태 변경 이벤트
    public delegate void UnlockStateChangedHandler(string id, bool isUnlocked);
    public event UnlockStateChangedHandler OnUnlockStateChanged;

    /// <summary>
    /// 새 게임(New Game) 시작 시 명시적으로 호출되어 기본 해금 항목(전통 음료 5종 등)을 등록합니다.
    /// Awake()에서 자동 호출하지 않습니다.
    /// </summary>
    public void InitializeDefaultUnlocks()
    {
        _unlockedIds.Clear();
        foreach (var id in UnlockConstants.DEFAULT_UNLOCKED_IDS)
        {
            if (!string.IsNullOrEmpty(id))
            {
                _unlockedIds.Add(id);
                OnUnlockStateChanged?.Invoke(id, true);
            }
        }
        Debug.Log($"[UnlockManager] 기본 해금 항목 {_unlockedIds.Count}개 초기화 완료.");
    }

    /// <summary>
    /// 지정한 ID의 항목을 해금 처리합니다.
    /// </summary>
    public void Unlock(string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        if (_unlockedIds.Add(id))
        {
            Debug.Log($"[UnlockManager] 항목 해금 완료: {id}");
            OnUnlockStateChanged?.Invoke(id, true);
        }
    }

    /// <summary>
    /// 전달받은 ID 목록을 일괄 해금 처리합니다. (디버그 및 보상용 Helper API)
    /// </summary>
    public void UnlockAll(IEnumerable<string> ids)
    {
        if (ids == null) return;
        foreach (var id in ids)
        {
            Unlock(id);
        }
    }

    /// <summary>
    /// 지정한 ID의 항목을 잠금 처리합니다.
    /// </summary>
    public void Lock(string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        if (_unlockedIds.Remove(id))
        {
            Debug.Log($"[UnlockManager] 항목 잠금 처리: {id}");
            OnUnlockStateChanged?.Invoke(id, false);
        }
    }

    /// <summary>
    /// 지정한 ID의 항목이 해금 상태인지 확인합니다.
    /// </summary>
    public bool IsUnlocked(string id)
    {
        if (string.IsNullOrEmpty(id)) return false;
        return _unlockedIds.Contains(id);
    }

    /// <summary>
    /// 현재 해금된 모든 항목의 ID 목록을 반환합니다.
    /// </summary>
    public List<string> GetAllUnlockedIds()
    {
        return new List<string>(_unlockedIds);
    }

    // ──────────────────────────────────────────────────────────────────────
    // SaveManager 연동 (GetSaveData / LoadSaveData)
    // ──────────────────────────────────────────────────────────────────────

    public UnlockSaveData GetSaveData()
    {
        return new UnlockSaveData
        {
            unlockedIds = GetAllUnlockedIds()
        };
    }

    /// <summary>
    /// 저장된 데이터만을 복원합니다. 기본 해금을 임의로 덮어쓰지 않습니다.
    /// </summary>
    public void LoadSaveData(UnlockSaveData saveData)
    {
        if (saveData == null || saveData.unlockedIds == null) return;

        _unlockedIds.Clear();
        foreach (var id in saveData.unlockedIds)
        {
            if (!string.IsNullOrEmpty(id))
            {
                _unlockedIds.Add(id);
            }
        }

        Debug.Log($"[UnlockManager] 해금 데이터 복구 완료 ({_unlockedIds.Count}개 항목 해금 중).");
    }
}
