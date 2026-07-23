# 콘텐츠 제작 및 밸런스 구축 보고서 (CONTENT_CREATION_REPORT)

새로운 Framework, Manager, Controller 또는 공용 시스템을 추가하지 않고, 기존에 완성된 ScriptableObject 구조(`RecipeData`, `DrinkData`, `IngredientData`, `CustomerData`)만을 활용하여 초반/중반/후반 티어별 콘텐츠 제작 및 경제/진행 밸런스 구축을 완료했습니다.

---

## 1. 재료 (Ingredients) 8종 명세

| 재료 ID | 재료명 | 구매가 (cost) | 판매가 (sellPrice) | 기본 해금 | 티어 |
| :--- | :--- | :---: | :---: | :---: | :---: |
| `Ing_Yuzu` | 유자청 | 10냥 | 5냥 | ✅ 해금 | 초반 |
| `Ing_Matcha` | 말차가루 | 15냥 | 7냥 | ✅ 해금 | 초반 |
| `Ing_Honey` | 토종꿀 | 12냥 | 6냥 | ✅ 해금 | 초반 |
| `Ing_Puerh` | 보이차엽 | 25냥 | 12냥 | 잠금 | 중반 |
| `Ing_Ceylon` | 홍차엽 | 30냥 | 15냥 | 잠금 | 중반 |
| `Ing_Milk` | 신선한 우유 | 20냥 | 10냥 | 잠금 | 중반 |
| `Ing_Lotus` | 백연꽃잎 | 50냥 | 25냥 | 잠금 | 후반 |
| `Ing_Ginseng` | 6년근 산삼 | 80냥 | 40냥 | 잠금 | 후반 |

---

## 2. 음료 (Drinks) & 레시피 (Recipes) 8종 밸런스 명세

| 음료/레시피 ID | 차 이름 | 필요 재료 | 총 원가 | 판매가 | 순이익 | 제조시간 | 티어 |
| :--- | :--- | :--- | :---: | :---: | :---: | :---: | :---: |
| `Recipe_YuzuTea` | 유자차 | 유자청 x2 | 20냥 | 35냥 | **+15냥** | 3.0초 | 초반 |
| `Recipe_MatchaLatte` | 말차라떼 | 말차가루 x1, 우유 x1 | 35냥 | 45냥 | **+10냥** | 3.5초 | 초반 |
| `Recipe_HoneyYuzu` | 꿀유자차 | 유자청 x1, 토종꿀 x1 | 22냥 | 50냥 | **+28냥** | 4.0초 | 초반 |
| `Recipe_PuerhTea` | 보이차 | 보이차엽 x1 | 25냥 | 70냥 | **+45냥** | 4.5초 | 중반 |
| `Recipe_MilkTea` | 밀크티 | 홍차엽 x1, 우유 x2 | 70냥 | 80냥 | **+10냥** | 5.0초 | 중반 |
| `Recipe_LotusTea` | 연화차 | 백연꽃잎 x1, 토종꿀 x1 | 62냥 | 120냥 | **+58냥** | 6.0초 | 후반 |
| `Recipe_GinsengTea` | 인삼차 | 6년근 산삼 x1 | 80냥 | 200냥 | **+120냥** | 7.0초 | 후반 |
| `Recipe_RoyalBlend` | 황실특제차 | 산삼 x1, 연꽃잎 x1, 꿀 x1 | 142냥 | 350냥 | **+208냥** | 8.5초 | 후반 |

---

## 3. 손님 (Customers) 5종 명세

| 손님 ID | 손님 이름 | 선호 음료 (Favorite Drink) | 등장 조건 및 특징 |
| :--- | :--- | :--- | :--- |
| `Cust_Villager` | 마을주민 | 유자차 (`Drink_YuzuTea`) | 영업 시작 시 기본 방문 (초반) |
| `Cust_Scholar` | 이선비 | 보이차 (`Drink_PuerhTea`) | 보이차 해금 시 방명록 등장 (중반) |
| `Cust_Merchant` | 김상인 | 밀크티 (`Drink_MilkTea`) | 냥전 소지량 100냥 이상 시 방문 (중반) |
| `Cust_Artist` | 박화공 | 연화차 (`Drink_LotusTea`) | 화전 3일차 이후 방문 (후반) |
| `Cust_Noble` | 최양반 | 황실특제차 (`Drink_RoyalBlend`) | 최고급 음료 주문 (후반 고수익) |

---

## 4. 원클릭 에디터 구축 툴 (`SetupContentDatabase`)

- 유니티 상단 메뉴 `Tools > Setup > Build Full Content Database` 등록 완료.
- 클릭 시 모든 `ScriptableObject` 에셋이 `Resources/ScriptableObjects/` 하위 폴더에 자동 배치되고 Database에 동적 등록됩니다.

---

## 5. Save/Load 직렬화 검증

- **JSON 직렬화 호환성**: 신규 재료/레시피/손님 ID는 기존 `SaveManager`, `InventoryManager`, `UnlockManager`의 `string ID` 저장 구조와 100% 완벽 호환됩니다.
