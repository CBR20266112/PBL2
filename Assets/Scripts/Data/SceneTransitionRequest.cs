/// <summary>
/// 씬 전환 요청 시 필요한 파라미터를 보관하는 DTO 클래스입니다.
/// </summary>
public class SceneTransitionRequest
{
    public string sceneName;
    public float fadeDuration = 0.5f;
    public bool showLoadingUI = true;

    public SceneTransitionRequest() { }

    public SceneTransitionRequest(string sceneName, float fadeDuration = 0.5f, bool showLoadingUI = true)
    {
        this.sceneName = sceneName;
        this.fadeDuration = fadeDuration;
        this.showLoadingUI = showLoadingUI;
    }
}
