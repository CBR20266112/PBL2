using UnityEngine;

/// <summary>
/// 손님 클래스
/// 게임 중 나타나는 손님 인스턴스
/// </summary>
public class Customer : MonoBehaviour
{
    public CustomerData data;
    public int familiarityLevel = 0; // 0~5
    private int _totalVisits = 0;

    public void Initialize(CustomerData customerData)
    {
        data = customerData;
        familiarityLevel = 0;
        _totalVisits = 0;
        Debug.Log($"Customer initialized: {data.customerName}");
    }

    public void OnVisited()
    {
        _totalVisits++;
    }

    public void IncreaseFamiliarity()
    {
        if (familiarityLevel < 5)
        {
            familiarityLevel++;
            Debug.Log($"{data.customerName} familiarity increased to {familiarityLevel}");
        }
    }

    public bool IsSatisfiedWith(string teaName, int temperature, int steepTime)
    {
        // 선호하는 차인지 확인
        bool teaMatches = data.preferredTea == teaName;
        bool tempMatches = data.preferredTemperature == temperature;
        bool timeMatches = data.preferredSteepTime == steepTime;

        // 모두 일치하면 만족 (5별)
        // 2개 일치하면 보통 (3~4별)
        // 1개 이하 일치하면 불만족 (1~2별)

        int matchCount = 0;
        if (teaMatches) matchCount++;
        if (tempMatches) matchCount++;
        if (timeMatches) matchCount++;

        return matchCount >= 2;
    }

    public int GetRating(string teaName, int temperature, int steepTime)
    {
        int matchCount = 0;
        if (data.preferredTea == teaName) matchCount++;
        if (data.preferredTemperature == temperature) matchCount++;
        if (data.preferredSteepTime == steepTime) matchCount++;

        // 일치도에 따라 별점 계산
        return matchCount switch
        {
            3 => 5, // 완벽
            2 => Random.Range(3, 5), // 좋음
            1 => Random.Range(2, 4), // 보통
            _ => Random.Range(1, 3)  // 부족
        };
    }
}
