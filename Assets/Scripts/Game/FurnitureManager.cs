using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 가구 상태 변경 유형 열거형
/// </summary>
public enum FurnitureStateChangeType
{
    Owned,       // 가구 획득
    Placed,      // 가구 배치
    Removed,     // 가구 배치 해제
    Activated,   // 가구 활성화
    Deactivated  // 가구 비활성화
}

/// <summary>
/// 가구 직렬화용 세이브 컨테이너
/// </summary>
[System.Serializable]
public class FurnitureSaveData
{
    public List<string> ownedFurnitureIds = new List<string>();
    public List<string> placedFurnitureIds = new List<string>();
    public List<string> activeFurnitureIds = new List<string>();
}

/// <summary>
/// 카페 가구의 보유(Owned), 배치(Placed), 활성화(Active) 상태 관리를 전담하는 매니저 클래스입니다.
/// 가구 효과 계산, UI 렌더링, 구매/해금 검증은 하지 않고 순수 가구 상태 데이터만 제공합니다.
/// </summary>
public class FurnitureManager : Singleton<FurnitureManager>
{
    // 가구 보유 상태
    private readonly HashSet<string> _ownedFurnitureIds = new HashSet<string>();

    // 가구 배치 상태 (placed ⊂ owned)
    private readonly HashSet<string> _placedFurnitureIds = new HashSet<string>();

    // 가구 활성화 상태 (active ⊂ placed ⊂ owned)
    private readonly HashSet<string> _activeFurnitureIds = new HashSet<string>();

    // 가구 상태 변경 이벤트 (변경 유형 타입 포함)
    public delegate void FurnitureStateChangedHandler(string furnitureId, FurnitureStateChangeType changeType);
    public event FurnitureStateChangedHandler OnFurnitureStateChanged;

    // 슬롯 배치 상태 (slotId -> furnitureId)
    private readonly Dictionary<string, string> _slotToFurnitureMap = new Dictionary<string, string>();

    /// <summary>
    /// 지정한 가구를 지정한 슬롯에 배치 가능한지 여부를 확인합니다.
    /// </summary>
    public bool CanPlaceFurniture(string furnitureId, string slotId)
    {
        if (string.IsNullOrEmpty(furnitureId) || string.IsNullOrEmpty(slotId)) return false;
        if (!HasFurniture(furnitureId)) return false;
        if (IsPlaced(furnitureId)) return false; // 이미 배치되어 있는 경우

        return true;
    }

    /// <summary>
    /// 지정한 가구를 지정한 슬롯에서 배치 해제 가능한지 여부를 확인합니다.
    /// </summary>
    public bool CanRemoveFurniture(string furnitureId, string slotId)
    {
        if (string.IsNullOrEmpty(furnitureId)) return false;
        return IsPlaced(furnitureId);
    }

    /// <summary>
    /// 지정한 가구를 보유 목록에 추가합니다. (구매 성공 시 ShopManager 등에서 호출)
    /// </summary>
    public void OwnFurniture(string furnitureId)
    {
        if (string.IsNullOrEmpty(furnitureId)) return;

        if (_ownedFurnitureIds.Add(furnitureId))
        {
            Debug.Log($"[FurnitureManager] 가구 획득: {furnitureId}");
            OnFurnitureStateChanged?.Invoke(furnitureId, FurnitureStateChangeType.Owned);
        }
    }

    /// <summary>
    /// 지정한 가구를 보유하고 있는지 확인합니다.
    /// </summary>
    public bool HasFurniture(string furnitureId)
    {
        if (string.IsNullOrEmpty(furnitureId)) return false;
        return _ownedFurnitureIds.Contains(furnitureId);
    }

    /// <summary>
    /// 보유한 가구를 카페에 배치 처리합니다. (자동으로 Active 상태를 부여하지 않습니다)
    /// </summary>
    public void PlaceFurniture(string furnitureId)
    {
        if (!HasFurniture(furnitureId))
        {
            Debug.LogWarning($"[FurnitureManager] 보유하지 않은 가구({furnitureId})는 배치할 수 없습니다.");
            return;
        }

        if (_placedFurnitureIds.Add(furnitureId))
        {
            Debug.Log($"[FurnitureManager] 가구 배치 완료: {furnitureId}");
            OnFurnitureStateChanged?.Invoke(furnitureId, FurnitureStateChangeType.Placed);
        }
    }

    /// <summary>
    /// 배치된 가구를 배치 해제(제거) 처리합니다. (배치 해제 시 활성 상태도 해제됩니다)
    /// </summary>
    public void RemoveFurniture(string furnitureId)
    {
        if (string.IsNullOrEmpty(furnitureId)) return;

        if (_placedFurnitureIds.Remove(furnitureId))
        {
            _activeFurnitureIds.Remove(furnitureId);
            Debug.Log($"[FurnitureManager] 가구 배치 해제: {furnitureId}");
            OnFurnitureStateChanged?.Invoke(furnitureId, FurnitureStateChangeType.Removed);
        }
    }

    /// <summary>
    /// 지정한 가구가 카페에 배치되어 있는지 확인합니다.
    /// </summary>
    public bool IsPlaced(string furnitureId)
    {
        if (string.IsNullOrEmpty(furnitureId)) return false;
        return _placedFurnitureIds.Contains(furnitureId);
    }

    /// <summary>
    /// 배치된 가구의 활성화/비활성화 상태를 변경합니다. (IsPlaced 상태에서만 허용)
    /// </summary>
    public void SetActive(string furnitureId, bool active)
    {
        if (!IsPlaced(furnitureId))
        {
            Debug.LogWarning($"[FurnitureManager] 배치되지 않은 가구({furnitureId})의 활성 상태는 변경할 수 없습니다.");
            return;
        }

        bool wasActive = IsActive(furnitureId);
        if (active)
        {
            if (_activeFurnitureIds.Add(furnitureId) && !wasActive)
            {
                Debug.Log($"[FurnitureManager] 가구 활성화: {furnitureId}");
                OnFurnitureStateChanged?.Invoke(furnitureId, FurnitureStateChangeType.Activated);
            }
        }
        else
        {
            if (_activeFurnitureIds.Remove(furnitureId) && wasActive)
            {
                Debug.Log($"[FurnitureManager] 가구 비활성화: {furnitureId}");
                OnFurnitureStateChanged?.Invoke(furnitureId, FurnitureStateChangeType.Deactivated);
            }
        }
    }

    /// <summary>
    /// 지정한 가구가 현재 활성화 상태인지 확인합니다.
    /// </summary>
    public bool IsActive(string furnitureId)
    {
        if (string.IsNullOrEmpty(furnitureId)) return false;
        return _activeFurnitureIds.Contains(furnitureId);
    }

    /// <summary>
    /// 보유 중인 모든 가구 ID 목록을 반환합니다.
    /// </summary>
    public List<string> GetOwnedFurnitureIds()
    {
        return new List<string>(_ownedFurnitureIds);
    }

    /// <summary>
    /// 배치되어 있는 모든 가구 ID 목록을 반환합니다.
    /// </summary>
    public List<string> GetPlacedFurnitureIds()
    {
        return new List<string>(_placedFurnitureIds);
    }

    // ──────────────────────────────────────────────────────────────────────
    // SaveManager 연동 (GetSaveData / LoadSaveData)
    // ──────────────────────────────────────────────────────────────────────

    public FurnitureSaveData GetSaveData()
    {
        return new FurnitureSaveData
        {
            ownedFurnitureIds = GetOwnedFurnitureIds(),
            placedFurnitureIds = GetPlacedFurnitureIds(),
            activeFurnitureIds = new List<string>(_activeFurnitureIds)
        };
    }

    /// <summary>
    /// 데이터 무결성 검증 (active ⊂ placed ⊂ owned)을 수행하며 데이터를 복원합니다.
    /// </summary>
    public void LoadSaveData(FurnitureSaveData saveData)
    {
        if (saveData == null) return;

        _ownedFurnitureIds.Clear();
        _placedFurnitureIds.Clear();
        _activeFurnitureIds.Clear();

        // 1. 보유 데이터 복원
        if (saveData.ownedFurnitureIds != null)
        {
            foreach (var id in saveData.ownedFurnitureIds)
            {
                if (!string.IsNullOrEmpty(id)) _ownedFurnitureIds.Add(id);
            }
        }

        // 2. 무결성 검증 1단계: placed ⊂ owned (보유한 가구만 배치 허용)
        if (saveData.placedFurnitureIds != null)
        {
            foreach (var id in saveData.placedFurnitureIds)
            {
                if (!string.IsNullOrEmpty(id) && _ownedFurnitureIds.Contains(id))
                {
                    _placedFurnitureIds.Add(id);
                }
            }
        }

        // 3. 무결성 검증 2단계: active ⊂ placed (배치된 가구만 활성화 허용)
        if (saveData.activeFurnitureIds != null)
        {
            foreach (var id in saveData.activeFurnitureIds)
            {
                if (!string.IsNullOrEmpty(id) && _placedFurnitureIds.Contains(id))
                {
                    _activeFurnitureIds.Add(id);
                }
            }
        }

        Debug.Log($"[FurnitureManager] 가구 데이터 복구 완료 (보유: {_ownedFurnitureIds.Count}, 배치: {_placedFurnitureIds.Count}, 활성: {_activeFurnitureIds.Count}).");
    }
}
