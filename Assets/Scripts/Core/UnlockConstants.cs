using UnityEngine;

/// <summary>
/// 해금 시스템 관련 상수 정의 클래스
/// </summary>
public static class UnlockConstants
{
    /// <summary>
    /// 새 게임 시작 시 제공되는 기본 해금 항목 ID 목록 (전통 음료 5종 등)
    /// </summary>
    public static readonly string[] DEFAULT_UNLOCKED_IDS = new string[]
    {
        "yuzu",
        "matcha",
        "puerh",
        "lotus",
        "shyr_chai"
    };
}
