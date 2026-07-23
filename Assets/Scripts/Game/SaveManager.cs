using UnityEngine;

/// <summary>
/// 최상위 저장 데이터 컨테이너
/// </summary>
[System.Serializable]
public class GameSaveData
{
    public int version = 1;

    // 현재 단계 구현 대상
    public MoneySaveData moneyData = new MoneySaveData();
    public InventorySaveData inventoryData = new InventorySaveData();
    public DaySaveData dayData = new DaySaveData();
    public AffinitySaveData affinityData = new AffinitySaveData();
    public UnlockSaveData unlockData = new UnlockSaveData();
    public FurnitureSaveData furnitureData = new FurnitureSaveData();
    public SettingsSaveData settingsData = new SettingsSaveData();
    public TutorialSaveData tutorialData = new TutorialSaveData();
}

/// <summary>
/// 게임의 유일한 저장/로드 관리자 클래스입니다.
/// 모든 Sub-Manager들의 세이브 데이터를 일괄 직렬화(JSON)하여 저장 매체(PlayerPrefs 등)에 영구 보관합니다.
/// </summary>
public class SaveManager : Singleton<SaveManager>
{
    // 세이브 상수 (추후 SaveConstants 또는 GameConstants 클래스로 이관 가능)
    public const string SAVE_KEY = "PBL2_GameSaveData";
    public const int CURRENT_SAVE_VERSION = 1;

    // 게임 저장 완료 이벤트
    public event System.Action OnGameSaved;

    /// <summary>
    /// 세이브 데이터 존재 여부를 확인합니다.
    /// </summary>
    public bool HasSave()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }

    /// <summary>
    /// 현재 각 매니저의 데이터를 수집하여 JSON으로 직렬화 및 저장합니다.
    /// </summary>
    public void SaveGame()
    {
        GameSaveData saveData = new GameSaveData
        {
            version = CURRENT_SAVE_VERSION
        };

        if (MoneyManager.Instance != null)
            saveData.moneyData = MoneyManager.Instance.GetSaveData();

        if (InventoryManager.Instance != null)
            saveData.inventoryData = InventoryManager.Instance.GetSaveData();

        if (DayManager.Instance != null)
            saveData.dayData = DayManager.Instance.GetSaveData();

        if (AffinityManager.Instance != null)
            saveData.affinityData = AffinityManager.Instance.GetSaveData();

        if (UnlockManager.Instance != null)
            saveData.unlockData = UnlockManager.Instance.GetSaveData();

        if (FurnitureManager.Instance != null)
            saveData.furnitureData = FurnitureManager.Instance.GetSaveData();

        if (SettingsManager.Instance != null)
            saveData.settingsData = SettingsManager.Instance.GetSaveData();

        if (TutorialManager.Instance != null)
            saveData.tutorialData = TutorialManager.Instance.GetSaveData();

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
        
        Debug.Log($"[SaveManager] 게임 자동 저장 완료 (버전: {CURRENT_SAVE_VERSION})");
        OnGameSaved?.Invoke();
    }

    /// <summary>
    /// 저장된 JSON 데이터를 읽어와 각 매니저에 수거 및 데이터 복원을 수행합니다.
    /// </summary>
    public bool LoadGame()
    {
        if (!HasSave())
        {
            Debug.LogWarning("[SaveManager] 저장된 데이터를 찾을 수 없습니다.");
            return false;
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

        if (saveData == null) return false;

        // 세이브 데이터 버전 검사 및 마이그레이션 준비
        if (saveData.version != CURRENT_SAVE_VERSION)
        {
            Debug.LogWarning($"[SaveManager] 저장 버전 불일치 (데이터 버전: {saveData.version}, 현재 버전: {CURRENT_SAVE_VERSION})");
            // TODO: 향후 세이브 데이터 마이그레이션 로직(Migration System)을 여기에 연결합니다.
        }

        if (MoneyManager.Instance != null && saveData.moneyData != null)
            MoneyManager.Instance.LoadSaveData(saveData.moneyData);

        if (InventoryManager.Instance != null && saveData.inventoryData != null)
            InventoryManager.Instance.LoadSaveData(saveData.inventoryData);

        if (DayManager.Instance != null && saveData.dayData != null)
            DayManager.Instance.LoadSaveData(saveData.dayData);

        if (AffinityManager.Instance != null && saveData.affinityData != null)
            AffinityManager.Instance.LoadSaveData(saveData.affinityData);

        if (UnlockManager.Instance != null && saveData.unlockData != null)
            UnlockManager.Instance.LoadSaveData(saveData.unlockData);

        if (FurnitureManager.Instance != null && saveData.furnitureData != null)
            FurnitureManager.Instance.LoadSaveData(saveData.furnitureData);

        if (SettingsManager.Instance != null && saveData.settingsData != null)
            SettingsManager.Instance.LoadSaveData(saveData.settingsData);

        if (TutorialManager.Instance != null && saveData.tutorialData != null)
            TutorialManager.Instance.LoadSaveData(saveData.tutorialData);

        Debug.Log("[SaveManager] 게임 데이터 복원 완료.");
        return true;
    }

    /// <summary>
    /// 저장된 데이터를 삭제합니다.
    /// </summary>
    public void DeleteSave()
    {
        if (HasSave())
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
            PlayerPrefs.Save();
            Debug.Log("[SaveManager] 저장 데이터가 삭제되었습니다.");
        }
    }
}
