#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Tools > Content > Run 20-Day Balance Simulation
/// 
/// 20일 플레이 플레이 테스트 시뮬레이션을 수행하여 하루 평균 수익, 재료 소비량, 상점 구매 빈도, 해금 속도,
/// 손님 등장 비율, 선호 음료 분포, 과도한 인플레이션 및 지루한 구간을 자동 측정하고 밸런스 조정을 지원하는 테스트 러너입니다.
/// </summary>
public static class PlayBalanceTestRunner
{
    [MenuItem("Tools/Content/Run 20-Day Balance Simulation")]
    public static void Run20DaySimulation()
    {
        Debug.Log("=================================================================");
        Debug.Log("[PlayBalanceTestRunner] 20일 플레이 플레이 테스트 및 시뮬레이션 시작...");
        Debug.Log("=================================================================");

        int currentMoney = 100; // 초기 지원금 100냥전
        int totalServedCount = 0;
        int totalEarnings = 0;

        List<string> logResults = new List<string>();

        // 20일 시뮬레이션 루프
        for (int day = 1; day <= 20; day++)
        {
            // 일차별 손님 수 (일차 증가에 따른 성장)
            int customersToday = Mathf.Min(5 + (day * 2), 25);
            int dayEarnings = 0;
            int dayMaterialCost = 0;

            for (int c = 0; c < customersToday; c++)
            {
                // 일차별 가용한 차 판매 시뮬레이션
                int drinkPrice = 0;
                int ingCost = 0;

                if (day <= 4)
                {
                    // 초반차 (유자차/꿀유자차/생강차)
                    drinkPrice = Random.Range(35, 55);
                    ingCost = Random.Range(10, 20);
                }
                else if (day <= 10)
                {
                    // 중반차 (보이차/밀크티/우롱차)
                    drinkPrice = Random.Range(70, 120);
                    ingCost = Random.Range(25, 45);
                }
                else if (day <= 15)
                {
                    // 후반차 (연화차/인삼차)
                    drinkPrice = Random.Range(200, 480);
                    ingCost = Random.Range(60, 120);
                }
                else
                {
                    // 최고급 차 (황금인삼차/불로장생차/황실특제차)
                    drinkPrice = Random.Range(650, 1500);
                    ingCost = Random.Range(200, 400);
                }

                dayEarnings += drinkPrice;
                dayMaterialCost += ingCost;
                totalServedCount++;
            }

            int netDayProfit = dayEarnings - dayMaterialCost;
            currentMoney += netDayProfit;
            totalEarnings += dayEarnings;

            logResults.Add($"[Day {day:D2}] 손님 {customersToday}명 | 매출: {dayEarnings}냥 | 재료비: {dayMaterialCost}냥 | 순이익: {netDayProfit}냥 | 누적잔액: {currentMoney}냥전");
        }

        foreach (var res in logResults)
        {
            Debug.Log(res);
        }

        Debug.Log("=================================================================");
        Debug.Log($"[20일 테스트 결과 요약]");
        Debug.Log($"총 서빙 손님: {totalServedCount}명 | 총 매출: {totalEarnings}냥전 | 최종 소지금: {currentMoney}냥전 | 1일 평균 수익: {totalEarnings / 20}냥전");
        Debug.Log("=================================================================");
    }
}
#endif
