#if UNITY_EDITOR || DEVELOPMENT_BUILD
/// <summary>
/// 디버그 패널에서 실행할 수 있는 명령의 유형을 정의하는 열거형입니다.
/// </summary>
public enum DebugCommand
{
    // Money Category
    AddMoney100 = 1,
    AddMoney1000 = 2,
    ResetMoney = 3,

    // Day Category
    AdvanceDay = 10,
    ForceEndDay = 11,

    // Customer Category
    SpawnCustomer = 20,
    CompleteCurrentOrder = 21,

    // Unlock Category
    UnlockAllRecipes = 30,
    UnlockAllFurniture = 31,
    AddAffinity10 = 32,

    // Save Category
    SaveGame = 40,
    LoadGame = 41,
    DeleteSave = 42,

    // Scene Category
    TransitionToMainGame = 50,
    TransitionToTitle = 51
}
#endif
