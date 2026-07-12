# MASTER ASSET NAMING

> **작성 일자**: 2026-07-12
> **문서 목적**: 추출된 모든 Sprite 이미지가 Unity 프로젝트 내에서 체계적으로 관리되고 Addressables로 쉽게 분류될 수 있도록 하는 최종 명명 규칙 가이드

---

## 1. 글로벌 네이밍 컨벤션

모든 아트 에셋(Sprite, Texture)은 아래의 절대 규칙을 따릅니다.

**포맷**: `[Category]_[Name]_[Variant/Culture]_[Index]`
**예시**: `TeaWare_Teapot_Celadon_01`

- **Category**: 대분류 접두사 (아래 2항 참조)
- **Name**: 에셋의 구체적인 영문 이름 (PascalCase 적용, 예: `WoodTable`, `GreenTea`)
- **Variant/Culture**: 아종, 색상, 문화권 또는 언어 표시 (예: `Red`, `Gold`, `Korea`, `KR`, `EN`)
- **Index**: 동일 유형 식별용 2자리 일련번호 (`01`, `02`...)

---

## 2. 통합 Category 접두사 (Prefix) 구조

Unity 프로젝트 폴더 구조와 매핑되는 11개의 주요 분류입니다.

| 접두사 (Prefix) | 대상 에셋 | 폴더 매핑 예시 |
|-----------------|-----------|---------------|
| `UI_` | 버튼, 탭, 패널, 게이지, 엠블럼, 정보 카드, 말풍선 | `Assets/Sprites/UI/` |
| `Background_` | 풀 씬 배경 아트, 바닥 타일 등 배경 깔개 | `Assets/Sprites/Environment/BG/` |
| `Environment_` | 벽면, 창문, 문, 기둥, 건축용 판자/보 | `Assets/Sprites/Environment/Architecture/` |
| `Furniture_` | 테이블, 의자, 리셉션 데스크, 대형 수납장 | `Assets/Sprites/Decor/Furniture/` |
| `Decoration_` | 액자, 족자, 분재, 쿠션, 칠판 등 인테리어 소품 | `Assets/Sprites/Decor/Props/` |
| `TeaWare_` | 티팟, 찻잔, 차판(트레이), 가이완, 유리 피처 | `Assets/Sprites/TeaSystem/TeaWare/` |
| `TeaDrink_` | 서빙할 준비가 된 완성된 차 음료(잔에 담긴 형태) | `Assets/Sprites/TeaSystem/Drinks/` |
| `Ingredient_` | 차 원재료(찻잎, 말차), 부재료(꿀, 빙당, 우유, 레몬) | `Assets/Sprites/TeaSystem/Ingredients/` |
| `Prop_` | 게임 상호작용 도구 (국자, 차침, 온도계, 모래시계) | `Assets/Sprites/TeaSystem/Tools/` |
| `Effect_` | 수증기, 파티클, 물방울, 메뉴의 반짝임 효과, 조명 등 | `Assets/Sprites/Effects/` |
| `Animation_` | 프레임 애니메이션용 시퀀스 스프라이트 (Brewing 등) | `Assets/Sprites/Animations/` |

---

## 3. 언어 및 문화권 (Variant) 표기 기준

티 카페의 다문화 요소를 식별하기 위한 꼬리표(Variant) 기준입니다.

- **국가/스타일**: `Korea` (한국풍), `Japan` (일본풍), `China` (중국풍), `Vietnam` (베트남풍), `Asian` (공통 동양풍), `Modern` (현대 기기)
- **UI 언어**: 한국어 텍스트 포함 시 `KR`, 영문 텍스트 포함 시 `EN`
- **색상**: `Red`, `Green`, `Gold`, `DarkWood` 등 명시적 색채가 특징인 경우

---

## 4. 카테고리별 대표 네이밍 예시 (Sample List)

추출 예정인 300여 개의 에셋 중 기준이 되는 대표 명명 예시입니다.

### 4.1. UI
- `UI_Button_Action_Brew_01` (직사각형 BREW 버튼)
- `UI_Button_Round_Recipe_01` (원형 레시피 버튼)
- `UI_Tab_Brewing_KR_01` (브루잉 한국어 탭 활성 상태)
- `UI_Card_Order_Empty_01` (주문 패널 베이스)
- `UI_Bar_Satisfaction_High_01` (고객 만족도 높음 게이지)

### 4.2. Environment & Background
- `Background_Floor_Tatami_Japan_01` (다다미 바닥재)
- `Environment_Wall_Shoji_Japan_01` (미닫이 창호 벽면)
- `Environment_Window_Lattice_Korea_01` (한국 전통 격자 창문)
- `Environment_Door_Wood_Open_01` (나무 여닫이문 열림 상태)

### 4.3. Furniture & Decoration
- `Furniture_Table_Low_Korea_01` (좌식 나무 테이블)
- `Furniture_ReceptionDesk_Asian_01` (L자 리셉션 데스크)
- `Decoration_Scroll_Zen_01` (禪 한자 두루마리 족자)
- `Decoration_Cushion_Square_02` (자수 사각 방석)

### 4.4. Tea System (TeaWare, Drink, Ingredient, Prop)
- `TeaWare_Teapot_Celadon_01` (청자 주전자)
- `TeaWare_Cup_Side_Floral_01` (측면 뷰 꽃무늬 찻잔)
- `Ingredient_MatchaPowder_01` (말차 가루 재료)
- `Ingredient_HoneyJar_01` (꿀 항아리 재료)
- `TeaDrink_MatchaLatte_01` (말차 라떼 완성품)
- `Prop_TeaNeedle_01` (차침 도구)
- `Prop_Hourglass_01` (모래시계)

### 4.5. Effect & Animation
- `Effect_Lantern_Paper_01` (종이 등롱 조명)
- `Effect_Steam_White_01` (흰색 수증기 이펙트)
- `Animation_Brewing_TeaPot_Pour_Row01_Frame01` (티팟 따르기 시퀀스 프레임 1)
- `Animation_Brewing_Gaiwan_Open_Row02_Frame08` (가이완 덮개 열기 프레임 8)
