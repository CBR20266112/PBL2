#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Tools > Setup > Integrate All Scenes
/// 
/// 유니티 조작 없이 클릭 한 번으로 Bootstrap, MainMenu, MainGame 씬 3개와
/// 씬 소속 Canvas, Global Overlay Canvas, Manager, UI Controller 계층 구조를 자동으로 멱등성 있게 구축합니다.
/// </summary>
public static class SetupIntegrationScenes
{
    private const string SCENES_FOLDER = "Assets/Scenes";

    [MenuItem("Tools/Setup/Integrate All Scenes")]
    public static void SetupAllScenes()
    {
        EnsureFolder(SCENES_FOLDER);

        SetupBootstrapScene();
        SetupMainMenuScene();
        SetupMainGameScene();

        UpdateBuildSettings();

        Debug.Log("=================================================================");
        Debug.Log("[SetupIntegrationScenes] Bootstrap, MainMenu, MainGame 씬 통합 구성 완료!");
        Debug.Log("Build Settings 씬 등록: 0. Bootstrap / 1. MainMenu / 2. MainGame");
        Debug.Log("=================================================================");
    }

    private static void SetupBootstrapScene()
    {
        string scenePath = $"{SCENES_FOLDER}/Bootstrap.unity";
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // 1. Global Managers
        GameObject globalManagers = new GameObject("[GlobalManagers]");
        globalManagers.AddComponent<SaveManager>();
        globalManagers.AddComponent<SettingsManager>();
        globalManagers.AddComponent<LocalizationManager>();
        globalManagers.AddComponent<AudioManager>();
        globalManagers.AddComponent<SceneTransitionManager>();
        globalManagers.AddComponent<NotificationManager>();
        globalManagers.AddComponent<DialogManager>();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        globalManagers.AddComponent<DebugManager>();
#endif

        // 2. Global Overlay Canvas (전역 Overlay UI만 탑재)
        GameObject overlayCanvasObj = CreateCanvasObject("[Global_Overlay_Canvas]", RenderMode.ScreenSpaceOverlay, 100);
        overlayCanvasObj.AddComponent<SceneTransitionView>();
        overlayCanvasObj.AddComponent<SceneTransitionController>();

        overlayCanvasObj.AddComponent<NotificationView>();
        overlayCanvasObj.AddComponent<NotificationController>();

        overlayCanvasObj.AddComponent<DialogView>();
        overlayCanvasObj.AddComponent<DialogController>();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        overlayCanvasObj.AddComponent<DebugView>();
        overlayCanvasObj.AddComponent<DebugController>();
#endif

        // 3. Loader
        GameObject loaderObj = new GameObject("[BootstrapLoader]");
        loaderObj.AddComponent<BootstrapLoader>();

        // EventSystem
        CreateEventSystem();

        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log($"[SetupIntegrationScenes] Bootstrap 씬 구축 완료: {scenePath}");
    }

    private static void SetupMainMenuScene()
    {
        string scenePath = $"{SCENES_FOLDER}/MainMenu.unity";
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Scene 소속 Canvas (요구사항 1 반영: Canvas 전체를 DontDestroyOnLoad 하지 않고 Scene 소속)
        GameObject sceneCanvas = CreateCanvasObject("[MainMenu_UI_Canvas]", RenderMode.ScreenSpaceOverlay, 10);
        sceneCanvas.AddComponent<MainMenuView>();
        sceneCanvas.AddComponent<MainMenuController>();

        sceneCanvas.AddComponent<SettingsView>();
        sceneCanvas.AddComponent<SettingsController>();

        CreateEventSystem();

        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log($"[SetupIntegrationScenes] MainMenu 씬 구축 완료: {scenePath}");
    }

    private static void SetupMainGameScene()
    {
        string scenePath = $"{SCENES_FOLDER}/MainGame.unity";
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // 1. InGame Managers
        GameObject inGameManagers = new GameObject("[InGameManagers]");
        inGameManagers.AddComponent<DayManager>();
        inGameManagers.AddComponent<MoneyManager>();
        inGameManagers.AddComponent<InventoryManager>();
        inGameManagers.AddComponent<ShopManager>();
        inGameManagers.AddComponent<BrewingManager>();
        inGameManagers.AddComponent<CustomerManager>();
        inGameManagers.AddComponent<OrderManager>();
        inGameManagers.AddComponent<ServingManager>();
        inGameManagers.AddComponent<AffinityManager>();
        inGameManagers.AddComponent<UnlockManager>();
        inGameManagers.AddComponent<FurnitureManager>();
        inGameManagers.AddComponent<TutorialManager>();

        // 2. Scene 소속 UI Canvas
        GameObject sceneCanvas = CreateCanvasObject("[InGame_UI_Canvas]", RenderMode.ScreenSpaceOverlay, 10);
        sceneCanvas.AddComponent<HUDView>();
        sceneCanvas.AddComponent<HUDController>();

        sceneCanvas.AddComponent<BrewingView>();
        sceneCanvas.AddComponent<BrewingController>();

        sceneCanvas.AddComponent<InventoryView>();
        sceneCanvas.AddComponent<InventoryController>();

        sceneCanvas.AddComponent<ShopView>();
        sceneCanvas.AddComponent<ShopController>();

        sceneCanvas.AddComponent<FurnitureView>();
        sceneCanvas.AddComponent<FurnitureController>();

        sceneCanvas.AddComponent<TutorialView>();
        sceneCanvas.AddComponent<TutorialController>();
        sceneCanvas.AddComponent<TutorialHighlightRegistry>();

        CreateEventSystem();

        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log($"[SetupIntegrationScenes] MainGame 씬 구축 완료: {scenePath}");
    }

    private static GameObject CreateCanvasObject(string name, RenderMode renderMode, int sortOrder)
    {
        GameObject canvasObj = new GameObject(name);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = renderMode;
        canvas.sortingOrder = sortOrder;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();
        return canvasObj;
    }

    private static void CreateEventSystem()
    {
        if (Object.FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<EventSystem>();
            esObj.AddComponent<StandaloneInputModule>();
        }
    }

    private static void UpdateBuildSettings()
    {
        EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene($"{SCENES_FOLDER}/Bootstrap.unity", true),
            new EditorBuildSettingsScene($"{SCENES_FOLDER}/MainMenu.unity", true),
            new EditorBuildSettingsScene($"{SCENES_FOLDER}/MainGame.unity", true)
        };
        EditorBuildSettings.scenes = scenes;
    }

    private static void EnsureFolder(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string parent = System.IO.Path.GetDirectoryName(path).Replace('\\', '/');
            string folderName = System.IO.Path.GetFileName(path);
            AssetDatabase.CreateFolder(parent, folderName);
        }
    }
}
#endif
