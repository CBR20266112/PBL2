/// <summary>
/// 씬 전환 진행 상태를 나타내는 열거형입니다.
/// </summary>
public enum SceneTransitionState
{
    Idle = 0,
    FadingIn = 1,
    LoadingScene = 2,
    FadingOut = 3
}
