using System;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
/// <summary>
/// 디버그 UI 패널의 버튼 및 토글 이벤트를 제어하는 View 클래스입니다.
/// 버튼별 개별 이벤트 대신 Action<DebugCommand> 단일 이벤트만 노출하여 Controller로 명령을 위임합니다.
/// Manager 클래스를 직접 참조하지 않으며 6가지 카테고리(Money, Day, Customer, Unlock, Save, Scene)로 그룹화되어 관리됩니다.
/// </summary>
public class DebugView : MonoBehaviour
{
    [Header("패널 제어")]
    [SerializeField] private GameObject debugPanel;
    [SerializeField] private Button btnTogglePanel;

    [Header("1. Money Category Buttons")]
    [SerializeField] private Button btnAddMoney100;
    [SerializeField] private Button btnAddMoney1000;
    [SerializeField] private Button btnResetMoney;

    [Header("2. Day Category Buttons")]
    [SerializeField] private Button btnAdvanceDay;
    [SerializeField] private Button btnForceEndDay;

    [Header("3. Customer Category Buttons")]
    [SerializeField] private Button btnSpawnCustomer;
    [SerializeField] private Button btnCompleteOrder;

    [Header("4. Unlock Category Buttons")]
    [SerializeField] private Button btnUnlockAllRecipes;
    [SerializeField] private Button btnUnlockAllFurniture;
    [SerializeField] private Button btnAddAffinity;

    [Header("5. Save Category Buttons")]
    [SerializeField] private Button btnSaveGame;
    [SerializeField] private Button btnLoadGame;
    [SerializeField] private Button btnDeleteSave;

    [Header("6. Scene Category Buttons")]
    [SerializeField] private Button btnTransitionMain;
    [SerializeField] private Button btnTransitionTitle;

    /// <summary>
    /// 디버그 명령어 단일 이벤트 릴레이 (DebugCommand enum 매개변수)
    /// </summary>
    public event Action<DebugCommand> OnCommandExecuted;

    private void Awake()
    {
        if (btnTogglePanel != null)
            btnTogglePanel.onClick.AddListener(TogglePanel);

        // 1. Money Category
        BindButton(btnAddMoney100, DebugCommand.AddMoney100);
        BindButton(btnAddMoney1000, DebugCommand.AddMoney1000);
        BindButton(btnResetMoney, DebugCommand.ResetMoney);

        // 2. Day Category
        BindButton(btnAdvanceDay, DebugCommand.AdvanceDay);
        BindButton(btnForceEndDay, DebugCommand.ForceEndDay);

        // 3. Customer Category
        BindButton(btnSpawnCustomer, DebugCommand.SpawnCustomer);
        BindButton(btnCompleteOrder, DebugCommand.CompleteCurrentOrder);

        // 4. Unlock Category
        BindButton(btnUnlockAllRecipes, DebugCommand.UnlockAllRecipes);
        BindButton(btnUnlockAllFurniture, DebugCommand.UnlockAllFurniture);
        BindButton(btnAddAffinity, DebugCommand.AddAffinity10);

        // 5. Save Category
        BindButton(btnSaveGame, DebugCommand.SaveGame);
        BindButton(btnLoadGame, DebugCommand.LoadGame);
        BindButton(btnDeleteSave, DebugCommand.DeleteSave);

        // 6. Scene Category
        BindButton(btnTransitionMain, DebugCommand.TransitionToMainGame);
        BindButton(btnTransitionTitle, DebugCommand.TransitionToTitle);
    }

    private void BindButton(Button btn, DebugCommand command)
    {
        if (btn != null)
        {
            btn.onClick.AddListener(() => OnCommandExecuted?.Invoke(command));
        }
    }

    /// <summary>
    /// 패널 표시/숨김을 토글합니다.
    /// </summary>
    public void TogglePanel()
    {
        if (debugPanel != null)
        {
            debugPanel.SetActive(!debugPanel.activeSelf);
        }
    }

    /// <summary>
    /// 패널의 표시 상태를 설정합니다.
    /// </summary>
    public void SetPanelActive(bool active)
    {
        if (debugPanel != null)
        {
            debugPanel.SetActive(active);
        }
    }
}
#endif
