using UnityEngine;

/// <summary>
/// 게임 전체 상태 관리
/// 씬 전환, 게임 상태, 플레이어 데이터 관리
/// </summary>
public class GameManager : Singleton<GameManager>
{
    // 게임 상태 열거형
    public enum GameState
    {
        Loading,      // 로딩 중
        Title,        // 타이틀 화면
        Tutorial,     // 튜토리얼
        Playing,      // 게임 플레이 중
        Paused,       // 일시정지
        GameOver      // 게임 종료
    }

    private GameState _currentState = GameState.Loading;
    public GameState CurrentState
    {
        get => _currentState;
        set
        {
            if (_currentState != value)
            {
                GameState previousState = _currentState;
                _currentState = value;
                OnGameStateChanged?.Invoke(previousState, _currentState);
                Debug.Log($"GameState changed: {previousState} -> {_currentState}");
            }
        }
    }

    // 이벤트
    public delegate void GameStateDelegate(GameState previousState, GameState newState);
    public event GameStateDelegate OnGameStateChanged;

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
        Debug.Log("GameManager initialized");
    }

    private void Start()
    {
        // 타이틀 화면으로 이동
        CurrentState = GameState.Title;
    }

    public void LoadScene(string sceneName)
    {
        CurrentState = GameState.Loading;
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void PlayGame()
    {
        CurrentState = GameState.Playing;
    }

    public void PauseGame()
    {
        if (CurrentState == GameState.Playing)
        {
            CurrentState = GameState.Paused;
            Time.timeScale = 0f;
        }
    }

    public void ResumeGame()
    {
        if (CurrentState == GameState.Paused)
        {
            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
        }
    }
}
