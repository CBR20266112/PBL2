using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
/// <summary>
/// DebugView의 단일 Action<DebugCommand> 이벤트를 수신하여 switch 구문으로 분기하고,
/// DebugManager의 API를 호출하는 Controller 클래스입니다.
/// 키보드 단축키(BackQuote `~` 또는 F1) 입력 감지를 통해 패널 토글도 처리합니다.
/// 릴리즈 빌드 시 전처리기(#if UNITY_EDITOR || DEVELOPMENT_BUILD)에 의해 완전히 제외됩니다.
/// </summary>
public class DebugController : MonoBehaviour
{
    [SerializeField] private DebugView view;
    [SerializeField] private RecipeDatabase recipeDatabase;

    private void OnEnable()
    {
        if (view != null)
        {
            view.OnCommandExecuted += HandleCommandExecuted;
        }
    }

    private void OnDisable()
    {
        if (view != null)
        {
            view.OnCommandExecuted -= HandleCommandExecuted;
        }
    }

    private void Update()
    {
        // 백쿼트 `~` 키 또는 F1 키로 디버그 패널 토글
        if (Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.F1))
        {
            if (view != null)
            {
                view.TogglePanel();
            }
        }
    }

    /// <summary>
    /// DebugView에서 올라온 DebugCommand 수신 및 switch 분기 처리 (수정 요구사항 1번)
    /// </summary>
    private void HandleCommandExecuted(DebugCommand command)
    {
        if (DebugManager.Instance == null) return;

        Debug.Log($"[DebugController] 디버그 명령 처리: {command}");

        switch (command)
        {
            // 1. Money Category
            case DebugCommand.AddMoney100:
                DebugManager.Instance.AddMoney(100);
                break;
            case DebugCommand.AddMoney1000:
                DebugManager.Instance.AddMoney(1000);
                break;
            case DebugCommand.ResetMoney:
                DebugManager.Instance.ResetMoney();
                break;

            // 2. Day Category
            case DebugCommand.AdvanceDay:
                DebugManager.Instance.AdvanceDay();
                break;
            case DebugCommand.ForceEndDay:
                DebugManager.Instance.ForceEndDay();
                break;

            // 3. Customer Category
            case DebugCommand.SpawnCustomer:
                DebugManager.Instance.SpawnCustomer();
                break;
            case DebugCommand.CompleteCurrentOrder:
                DebugManager.Instance.CompleteCurrentOrder();
                break;

            // 4. Unlock Category
            case DebugCommand.UnlockAllRecipes:
                ExecuteUnlockAllRecipes();
                break;
            case DebugCommand.UnlockAllFurniture:
                ExecuteUnlockAllFurniture();
                break;
            case DebugCommand.AddAffinity10:
                DebugManager.Instance.AddAffinity("customer_default", 10);
                break;

            // 5. Save Category
            case DebugCommand.SaveGame:
                DebugManager.Instance.SaveGame();
                break;
            case DebugCommand.LoadGame:
                DebugManager.Instance.LoadGame();
                break;
            case DebugCommand.DeleteSave:
                DebugManager.Instance.DeleteSave();
                break;

            // 6. Scene Category
            case DebugCommand.TransitionToMainGame:
                DebugManager.Instance.RequestSceneTransition(new SceneTransitionRequest("MainGame"));
                break;
            case DebugCommand.TransitionToTitle:
                DebugManager.Instance.RequestSceneTransition(new SceneTransitionRequest("Title"));
                break;
        }
    }

    private void ExecuteUnlockAllRecipes()
    {
        if (recipeDatabase == null) return;

        var recipes = recipeDatabase.GetAllRecipes();
        if (recipes == null) return;

        List<string> drinkIds = new List<string>();
        foreach (var r in recipes)
        {
            if (r != null && r.targetDrink != null && !string.IsNullOrEmpty(r.targetDrink.drinkId))
            {
                drinkIds.Add(r.targetDrink.drinkId);
            }
        }

        DebugManager.Instance.UnlockAll(drinkIds);
    }

    private void ExecuteUnlockAllFurniture()
    {
        // 가구 ID 목록을 DebugManager.Instance.UnlockAll / OwnFurniture로 전달
        string[] sampleFurniture = new string[] { "furniture_table_01", "furniture_chair_01", "furniture_shelf_01" };
        DebugManager.Instance.UnlockAll(sampleFurniture);

        foreach (var id in sampleFurniture)
        {
            DebugManager.Instance.OwnFurniture(id);
        }
    }
}
#endif
