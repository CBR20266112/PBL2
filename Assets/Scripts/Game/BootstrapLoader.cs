using System.Collections;
using UnityEngine;

/// <summary>
/// Bootstrap 씬에서 전역 싱글톤 매니저들의 최소 준비 상태만 확인하고,
/// 곧바로 MainMenu 씬으로 비동기 전환을 수행하는 진입점 로더입니다.
/// 매니저 순서를 하드코딩하지 않고 각 매니저의 Awake -> Start 라이프사이클에 초기화를 위임합니다.
/// </summary>
public class BootstrapLoader : MonoBehaviour
{
    [SerializeField] private string targetSceneName = GameConstants.SCENE_TITLE;
    [SerializeField] private float initialDelay = 0.2f;

    private IEnumerator Start()
    {
        Debug.Log("[BootstrapLoader] 게임 진입점 초기화 완료. MainMenu 씬 전환 준비...");
        
        yield return new WaitForSeconds(initialDelay);

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.RequestTransition(new SceneTransitionRequest(targetSceneName, 0.5f, true));
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(targetSceneName);
        }
    }
}
