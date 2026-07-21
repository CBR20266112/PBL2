# CHANGELOG

## v1.5.0 (2026-07-21)

### Added
- RecipeData (ScriptableObject) 생성: recipeId 및 우림 조건 제외. targetDrink(1:1) + IngredientRequirement 리스트(1:N) 구조로 완성품과 원재료 연결
- RecipeDatabase (ScriptableObject) 생성: targetDrink.drinkId 기준 Dictionary 캐싱, OnEnable 안전 초기화, GetRecipe/GetAllRecipes API 제공
- SetupRecipeDatabase (Editor Setup Tool) 생성: `Tools > Setup > Create Recipe Database` 메뉴 제공. 기존 DrinkDatabase/IngredientDatabase 무수정 확장, Shyr Chai(실론+우유+소금) 포함 5종 레시피 자동 링킹, Idempotent 보장

---

## v1.4.0 (2026-07-21)

### Added
- IngredientData (ScriptableObject) 생성: 재재료 고유 ID, 표시 이름, 경제 데이터(buyPrice, unlockLevel), 설명 정의. 공통 자원 정책에 맞춰 국가(CountryType) 필드 제외
- IngredientDatabase (ScriptableObject) 생성: 중앙 관리용 DB. 런타임 캐싱(Dictionary) 최적화 및 OnEnable 안전 초기화 보장
- SetupIngredientDatabase (Editor Setup Tool) 생성: `Tools > Setup > Create Ingredient Database` 메뉴 제공. 멱등성 보장(기존 에셋 변경 없음, 없는 것만 신규 생성), Shyr Chai용 소금/우유를 포함한 7종 초기 원재료 생성 및 DB 연결 자동화

---

## v1.3.2 (2026-07-21)


### Added
- AI_DEVELOPMENT_RULES.md: 에디터 셋업 툴 에셋 수정/덮어쓰기 금지 정책 및 마이그레이션 도구(Tools > Migration) 제공 의무화 규칙 명시

---

## v1.3.1 (2026-07-21)


### Changed
- SetupDrinkDatabase: `InitializeOnLoad + delayCall` 자동 실행 방식 제거
- Setup Tool을 `Tools > Setup > Create Drink Database` 수동 실행 방식으로 변경
- Idempotent 동작 보장: 기존 에셋 보존, 없는 항목만 신규 생성, 여러 번 실행 안전

---

## v1.3.0 (2026-07-21)

### Added
- DrinkData (ScriptableObject) 생성: 음료 기본 정보, 가격/재료비, 해금 조건, 온도/시간 설정 및 다국어 확장을 고려한 displayName 관리 지원
- DrinkDatabase (ScriptableObject) 생성: 런타임 캐시 최적화(Dictionary Pre-warming) 및 OnEnable 이중 안전 초기화 적용
- SetupDrinkDatabase (Editor Menu Tool) 생성: `Tools > Setup > Create Drink Database` 메뉴 제공. 5종 에셋(유자차, 보이차, 말차, 연꽃차, Shyr Chai) 및 폴더/DB 자동 생성 기능 구현

### Changed
- AudioManager.cs 컴파일 에러 수정: SettingsManager.cs와의 호환성을 위한 GetThemeBgmClip() 헬퍼 메서드 추가

---

## v0.3.0 (2026-07-11)

### Added
- MVP 구현 완료: Title → Main → Tutorial → Customer → Kitchen → Rating
- Customer system, brewing mechanics, order and rating flow
- Player save/load, money, experience, and level progression
- Dynamic UI builder implementation for all core screens

### Changed
- 버전업: MVP 단계 구현 완료, dev 브랜치에 커밋

### Documentation
- 개발 상태 파일 업데이트 및 버전 관리 적용

---

## v0.2.0 (2026-07-11)

### Added
- CHANGELOG.md 문서 시작
- GAME_DESIGN_DOCUMENT.md (상세 GDD)
- TEA_DETAIL_SPECS.md (차 상세 스펙)
- CUSTOMER_PROFILES.md (손님 프로필)
- GAME_ECONOMY.md (경제 시스템)
- PROGRESSION_SYSTEM.md (진행도 시스템)
- UI_SPECIFICATIONS.md (UI 상세 스펙)
- CHARACTER_DESIGN_SPECS.md (캐릭터 디자인)

### Changed
- PROJECT_MASTER_v0.2.md 기준으로 상세 기획 시작

### Documentation
- 기획 문서 8개 완성으로 v0.2.0 기획 단계 완료

---

## v0.1.0 (Initial)

### Added
- 프로젝트 초기 기획
- 5개 국가 선정
- 5종 차 선정
- 주인공 캐릭터 기획
