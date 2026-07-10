using UnityEngine;

/// <summary>
/// 메인 씬 초기화 스크립트
/// 게임 시작 시 실행되는 첫 씬의 진입점
/// </summary>
public class MainSceneInitializer : MonoBehaviour
{
    private void Awake()
    {
        // 게임 매니저 초기화
        GameManager gameManager = GameManager.Instance;
        gameManager.OnGameStateChanged += HandleGameStateChanged;

        // 플레이어 데이터 초기화
        PlayerDataManager playerDataManager = PlayerDataManager.Instance;

        // UI 매니저 초기화
        UIManager uiManager = UIManager.Instance;

        Debug.Log("MainSceneInitializer: All managers initialized");
    }

    private void Start()
    {
        // 게임 상태를 Playing으로 변경
        GameManager.Instance.CurrentState = GameManager.GameState.Playing;

        Debug.Log($"Game started - Player: {PlayerDataManager.Instance.PlayerName} (Lv.{PlayerDataManager.Instance.CurrentLevel})");
    }

    private void HandleGameStateChanged(GameManager.GameState previousState, GameManager.GameState newState)
    {
        Debug.Log($"Game state: {previousState} -> {newState}");
    }
}
