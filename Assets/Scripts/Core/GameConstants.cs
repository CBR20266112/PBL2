/// <summary>
/// 게임 전역 상수 및 설정값
/// </summary>
public static class GameConstants
{
    // 초기 설정값
    public const int INITIAL_MONEY = 10000;
    public const int INITIAL_LEVEL = 1;
    public const int INITIAL_EXP = 0;

    // 경험치 관련
    public const int BASE_EXP_PER_LEVEL = 500;
    public const float EXP_GROWTH_RATE = 0.4f; // 레벨마다 40% 증가

    // 차 가격
    public const int PRICE_YUZU = 4000;
    public const int PRICE_MATCHA = 5000;
    public const int PRICE_PUERH = 7000;
    public const int PRICE_LOTUS = 5500;
    public const int PRICE_CHAI = 6000;

    // 차 재료비
    public const int COST_YUZU = 1500;
    public const int COST_MATCHA = 2000;
    public const int COST_PUERH = 2500;
    public const int COST_LOTUS = 1800;
    public const int COST_CHAI = 2200;

    // 레벨별 해금 조건
    public const int LEVEL_TEA_UNLOCK_PUERH = 5;   // 보이차
    public const int LEVEL_TEA_UNLOCK_LOTUS = 5;   // 연꽃차
    public const int LEVEL_TEA_UNLOCK_CHAI = 10;   // 차이

    // 씬 이름
    public const string SCENE_TITLE = "Title";
    public const string SCENE_TUTORIAL = "Tutorial";
    public const string SCENE_MAIN = "Main";
    public const string SCENE_KITCHEN = "Kitchen";
}
