using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Localization 항목 단일 엔트리
/// </summary>
[System.Serializable]
public class LocalizationEntry
{
    [Tooltip("Localization Key (예: customer.name.sora, drink.name.yuzu)")]
    public string key;

    [Tooltip("한국어 텍스트")]
    public string korean;

    [Tooltip("영어 텍스트")]
    public string english;
}

/// <summary>
/// ScriptableObject 기반 다국어 문자열 데이터베이스입니다.
/// 현재는 단일 List 구조이며, 향후 카테고리(UI, Drink, Customer, Furniture 등) 단위로
/// 여러 개의 LocalizationDatabase 에셋을 분리하여 관리할 수 있도록 확장성을 고려하여 설계되었습니다.
/// </summary>
[CreateAssetMenu(fileName = "LocalizationDatabase", menuName = "Tea Culture Game/Localization Database")]
public class LocalizationDatabase : ScriptableObject
{
    [Tooltip("이 데이터베이스의 카테고리 식별자 (예: UI, Drink, Customer, Furniture 등). 향후 카테고리별 분리 시 활용합니다.")]
    public string category = "General";

    [Tooltip("Localization 엔트리 목록")]
    public List<LocalizationEntry> entries = new List<LocalizationEntry>();
}
