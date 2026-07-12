using UnityEngine;

/// <summary>
/// 손님 데이터 (ScriptableObject)
/// </summary>
[CreateAssetMenu(fileName = "Customer_", menuName = "Tea Culture Game/Customer")]
public class CustomerData : ScriptableObject
{
    public string customerName;
    public string characterType; // "luna", "hyuntae", "wei", "sakura", "denu"

    [Header("Visual")]
    [Tooltip("손님 캐릭터 스프라이트. Assets/Sprites/Character/Customers/ 에서 연결합니다.")]
    public Sprite characterSprite;
    public string preferredTea; // "yuzu", "matcha", "puerh", "lotus", "chai"
    public int preferredTemperature; // 0=low, 1=medium, 2=high
    public int preferredSteepTime; // 0=short, 1=medium, 2=long
    public string personality; // 성격 설명
    public string storyBit; // 이 손님의 짧은 스토리
}
