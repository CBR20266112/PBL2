using UnityEngine;

/// <summary>
/// 손님 데이터 생성 헬퍼
/// 테스트용으로 손님 데이터를 프로그래매틱하게 생성
/// </summary>
public class CustomerDataHelper : MonoBehaviour
{
    /// <summary>
    /// 5가지 기본 손님 데이터 생성
    /// </summary>
    public static CustomerData[] CreateDefaultCustomers()
    {
        CustomerData[] customers = new CustomerData[5];

        // 1. Luna (토끼, 낮은 온도, 짧은 우림, 유자차)
        customers[0] = ScriptableObject.CreateInstance<CustomerData>();
        customers[0].customerName = "Luna";
        customers[0].characterType = "luna";
        customers[0].preferredTea = "yuzu";
        customers[0].preferredTemperature = 0; // low
        customers[0].preferredSteepTime = 0; // short
        customers[0].personality = "수줍음, 예민함";
        customers[0].storyBit = "과제 때문에 밤 늦게 방문";

        // 2. Hyuntae (사자, 높은 온도, 긴 우림, 차이)
        customers[1] = ScriptableObject.CreateInstance<CustomerData>();
        customers[1].customerName = "Hyuntae";
        customers[1].characterType = "hyuntae";
        customers[1].preferredTea = "chai";
        customers[1].preferredTemperature = 2; // high
        customers[1].preferredSteepTime = 2; // long
        customers[1].personality = "활발함, 자신감";
        customers[1].storyBit = "회의 사이 잠깐 방문";

        // 3. Wei (판다, 중간 온도, 긴 우림, 보이차)
        customers[2] = ScriptableObject.CreateInstance<CustomerData>();
        customers[2].customerName = "Wei";
        customers[2].characterType = "wei";
        customers[2].preferredTea = "puerh";
        customers[2].preferredTemperature = 1; // medium
        customers[2].preferredSteepTime = 2; // long
        customers[2].personality = "차분함, 건강 관심";
        customers[2].storyBit = "건강식 영양사";

        // 4. Sakura (여우, 중간 온도, 짧은 우림, 말차)
        customers[3] = ScriptableObject.CreateInstance<CustomerData>();
        customers[3].customerName = "Sakura";
        customers[3].characterType = "sakura";
        customers[3].preferredTea = "matcha";
        customers[3].preferredTemperature = 1; // medium
        customers[3].preferredSteepTime = 0; // short
        customers[3].personality = "활기찬, 사교적";
        customers[3].storyBit = "SNS 크리에이터, 사진 촬영";

        // 5. Denu (독수리, 높은 온도, 긴 우림, 차이)
        customers[4] = ScriptableObject.CreateInstance<CustomerData>();
        customers[4].customerName = "Denu";
        customers[4].characterType = "denu";
        customers[4].preferredTea = "chai";
        customers[4].preferredTemperature = 2; // high
        customers[4].preferredSteepTime = 2; // long
        customers[4].personality = "침착함, 지혜로움";
        customers[4].storyBit = "은퇴한 차 수집가, 신비로움";

        return customers;
    }
}
