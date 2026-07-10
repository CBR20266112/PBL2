using UnityEngine;

/// <summary>
/// 플레이어 데이터 저장소
/// JSON + PlayerPrefs로 저장/로드
/// </summary>
public class PlayerDataManager : Singleton<PlayerDataManager>
{
    private PlayerData _playerData;

    public string PlayerName { get; set; } = "orangeCat";
    public int CurrentLevel { get; set; } = 1;
    public int CurrentMoney { get; set; } = 10000;
    public int CurrentExp { get; set; } = 0;

    protected override void Awake()
    {
        base.Awake();
        LoadPlayerData();
    }

    /// <summary>
    /// 플레이어 데이터 로드
    /// </summary>
    public void LoadPlayerData()
    {
        if (PlayerPrefs.HasKey("PlayerData"))
        {
            string json = PlayerPrefs.GetString("PlayerData");
            _playerData = JsonUtility.FromJson<PlayerData>(json);

            PlayerName = _playerData.playerName;
            CurrentLevel = _playerData.level;
            CurrentMoney = _playerData.money;
            CurrentExp = _playerData.experience;

            Debug.Log($"PlayerData loaded: {PlayerName} (Lv.{CurrentLevel})");
        }
        else
        {
            Debug.Log("No saved PlayerData found. Using default values.");
        }
    }

    /// <summary>
    /// 플레이어 데이터 저장
    /// </summary>
    public void SavePlayerData()
    {
        _playerData = new PlayerData
        {
            playerName = PlayerName,
            level = CurrentLevel,
            money = CurrentMoney,
            experience = CurrentExp
        };

        string json = JsonUtility.ToJson(_playerData);
        PlayerPrefs.SetString("PlayerData", json);
        PlayerPrefs.Save();

        Debug.Log("PlayerData saved");
    }

    /// <summary>
    /// 플레이어 이름 설정
    /// </summary>
    public void SetPlayerName(string name)
    {
        PlayerName = string.IsNullOrEmpty(name) ? "orangeCat" : name;
        SavePlayerData();
    }

    /// <summary>
    /// 돈 추가
    /// </summary>
    public void AddMoney(int amount)
    {
        CurrentMoney += amount;
        SavePlayerData();
    }

    /// <summary>
    /// 경험치 추가
    /// </summary>
    public void AddExp(int amount)
    {
        CurrentExp += amount;
        CheckLevelUp();
        SavePlayerData();
    }

    /// <summary>
    /// 레벨업 확인
    /// </summary>
    private void CheckLevelUp()
    {
        int expPerLevel = 500 + (CurrentLevel - 1) * 200;
        if (CurrentExp >= expPerLevel)
        {
            CurrentExp -= expPerLevel;
            CurrentLevel++;
            Debug.Log($"Level up! Now Lv.{CurrentLevel}");
        }
    }
}

[System.Serializable]
public class PlayerData
{
    public string playerName = "orangeCat";
    public int level = 1;
    public int money = 10000;
    public int experience = 0;
}
