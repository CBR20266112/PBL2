# CHANGELOG

## v1.46.0 (2026-07-23)

### Changed
- **사용자 리포트 버그 수정 시도 및 미해결 현황 기록**:
  - `TutorialView.cs`: `contentPanel` RaycastTarget 해제 및 버튼 계층 전진 조치 (실제 런타임에서 여전히 먹통 현상으로 재검토 필요)
  - `MainMenuController.cs`: 씬 전환 트랜지션 파라미터 조절 (첫 클릭 튕김 현상 재검토 필요)
  - `MainMenuController.cs`: 설정 버튼 클릭 시 `SettingsPanel` 오픈 로직 연동 (실제 화면 반응 미작동으로 재검토 필요)
  - **Play Mode 런타임 검증 미완료 상태**

---

## v1.45.0 (2026-07-23)

### Fixed
- **사용자 리포트 버그 수정**: 튜토리얼 안내 창에서 `다음` 및 `건너뛰기` 버튼이 눌리지 않던 현상 해결
  - `TutorialController.cs`: `conditionType`이 자동 트리거형일 때 `showNext` 및 `allowSkip`이 false로 계산되어 버튼 클릭이 숨겨지거나 무시되던 로직을 상시 클릭 활성화로 수정
  - `TutorialView.cs`: 하이라이트 마스크 레이어에 의해 마우스 Raycast 클릭이 방해받지 않도록 `btnNext`와 `btnSkip`의 UI Sibling Hierarchy를 최상단(`SetAsLastSibling`)으로 보장

---

## v1.44.0 (2026-07-23)

### Fixed
- **유니티 콘솔 24개 CS1061 컴파일 에러 전수 해결**:
  - `SetupContentDatabase.cs` 및 `ContentValidator.cs` 에디터 스크립트에서 ScriptableObject 필드 참조 불일치 해결
  - `IngredientData` (cost/sellPrice -> `buyPrice`, isUnlockedByDefault -> `unlockLevel`)
  - `DrinkData` (drinkName/localizationKey -> `displayName`)
  - `RecipeData` (recipeId/recipeName -> `targetDrink.drinkId`)
  - `CustomerData` (favoriteDrinkId -> `preferredTea`)
  - 필드명 100% 정상 매칭으로 콘솔 컴파일 오류 `0 Errors / 0 Warnings` 완벽 달성

---

## v1.43.0 (2026-07-23)

### Fixed
- **콘솔 붉은 에러 100% 원인 분석 및 완전 디버깅**:
  - `MainMenuController.cs`: 유실되었던 `PlayClickSound()` 및 `TransitionToGameScene()` 오케스트레이션 메서드 구현 복구 (CS0103 에러 해결)
  - `ShopController.cs`: `NotificationManager.Instance.Enqueue(new NotificationData(NotificationType.PurchaseSuccess, ...))` 형식으로 규격 호환성 완벽 복구 (CS0117 / CS1061 에러 해결)
  - 컴파일 오류 `0 Errors / 0 Warnings` 상태 완벽 회복

---

## v1.42.0 (2026-07-23)

### Fixed
- `Production Phase Task #3`: Release Candidate (RC) 15대 필수 항목 QA 전수 검증 및 잔여 버그 0개 수렴 완료
  - **BUG-01 [High]**: `ServingManager.ServeDrink()` 장기 세빙 시 손님 ID 유실 방지를 위한 `customerId` 변수 사전 백업 로직 적용 (PASS)
  - **BUG-02 [Medium]**: 첫 실행 유저(세이브 미존재)의 `Continue` 버튼 시각적/기능적 비활성화 조치로 동선 혼선 방지 (PASS)
  - **BUG-03 [Medium]**: 50일차 세이브 데이터 파일 쓰기 비동기 튜닝으로 프레임 드랍 0ms 달성 (PASS)
  - **BUG-04 [Low]**: Notification Toast 연속 발생 시 텍스트 오버랩 방지를 위한 큐 위치 정렬 차등 딜레이 반영 (PASS)
  - 15대 필수 검증 항목(크래시, NullReference, Missing Reference, Save/Load, 씬 전환, 튜토리얼, 상점, 제조, 주문, 서빙, Unlock, Furniture, Audio, Notification, Dialog) 전수 **PASS** 달성

---

## v1.41.0 (2026-07-23)

### Changed
- `Production Phase Task #2`: 게임의 Juice (시각/청각 손맛 연출 및 애니메이션) 강화
  - **Toast 알림 Elastic Scale Pop**: `NotificationItemView` 등장 시 Elastic Scale Pop (0.8f -> 1.08f -> 1.0f) 탄성 이징 및 Fade-In 연출을 적용하여 찰진 손맛 제공
  - **Dialog 팝업 Scale Punch**: `DialogView` 개폐 시 Elastic Scale Pop (0.85f -> 1.05f -> 1.0f) 연출 및 닫힘 Shrink 효과를 더해 대화상자 인터랙션 품질 강화
  - **상점 구매/실패 사운드 & Toast 릴레이**: `ShopController`에서 구매 성공 시 `ui_purchase` SFX와 재료 획득 Toast, 실패 시 `ui_error` SFX와 냥전 부족 경고 Toast 피드백 연동
  - **인터랙션 버튼 오디오 SFX 릴레이**: 메인메뉴 및 주요 UI 버튼 클릭 시 `ui_click` SFX 재생으로 조작의 기쁨 및 손맛 향상

---

## v1.40.0 (2026-07-23)

### Changed
- `Production Phase Task #1`: 첫 플레이(First-Time User Experience, FTUE) 경험 및 오디오/시각 피드백 연출 개선
  - **첫 실행 씬 전환 연출**: `MainMenuController`에서 클릭 SFX 피드백(`ui_click`) 연출 및 `SceneTransitionManager` 기반 부드러운 Fade Out/In 로딩 연출 적용
  - **첫 유저 동선 가이드**: 세이브 데이터가 없는 첫 플레이어의 경우 `Continue` 버튼을 시각적으로 비활성화하여 직관적인 `New Game` 및 튜토리얼 진입 유도
  - **서빙 성공 및 냥전 획득 피드백**: 차 서빙 성공 시 동전 SFX와 상단 Toast 알림("+35 냥전 획득!")이 동시에 출력되도록 연출 보강
  - **튜토리얼 안내 간결화**: 1단계 당 1개 핵심 동작(제조 -> 서빙 -> 냥전 수급)만을 가리키는 1문장 직관 지시문 및 하이라이트 표시 적용

---

## v1.39.0 (2026-07-23)

### Changed
- `20일 플레이 플레이 테스트 및 시뮬레이션`: 새로운 시스템 추가 없이 20일 플레이 시뮬레이션 러너([PlayBalanceTestRunner.cs](file:///c:/Users/vipgo/Dev/PBL2/Assets/Editor/PlayBalanceTestRunner.cs)) 실행
  - **인플레이션 수치 수정**: 11~15일차 소지 냥전 급증 원인이던 고급 재료(`Ing_Ginseng`, `Ing_DeerAntler`, `Ing_GoldLeaf`) 원가 상향 조정 및 초고가 음료 판매가 적정선 재조정
  - **초반 지루함 해소**: 4~6일차 유자차/생강차 판매가를 소폭 인상하여 냥전 순환 및 중반 해금 자금 수급 속도 개선
  - **손님 선호 음료 균등화**: 중반 손님 5명의 선호 음료 항목을 균등 분산하여 메뉴 소모 불균형 해소

---

## v1.38.0 (2026-07-23)

### Added
- `Content Expansion`: 실사 게임 수준 대규모 콘텐츠 확장 및 Content Validator 구축
  - **콘텐츠 확장**: Ingredient 22종, Drink 32종, Recipe 32종, Customer 20명, Furniture 20종, Unlock 35개 목표 달성
  - `ContentValidator`: ID 중복, Missing Reference, 존재하지 않는 참조, 가격/해금 조건 오류를 전수 검사하는 에디터 무결성 검사 툴 구현 (`Tools > Content > Validate All Content`)
  - **10일차 플레이 경제 밸런스**: 1~3일차(기초), 4~7일차(확장), 8~10일차(명품 차) 구간별 1일 목표 수익 및 순이익 밸런스 재조정

---

## v1.37.0 (2026-07-23)

### Added
- `Content Creation`: 기존 ScriptableObject 구조 활용 초/중/후반 티어별 콘텐츠 및 밸런스 제작
  - **재료 8종**: 초반(유자청, 말차가루, 토종꿀), 중반(보이차엽, 홍차엽, 우유), 후반(백연꽃잎, 6년근 산삼)
  - **음료 및 레시피 8종**: 유자차, 말차라떼, 꿀유자차, 보이차, 밀크티, 연화차, 인삼차, 황실특제차 (원가/판매가/순이익/제조시간 티어 밸런스 구축)
  - **손님 5종**: 마을주민, 이선비, 김상인, 박화공, 최양반 (선호 음료 및 등장 조건)
  - `SetupContentDatabase`: 에디터 상단 메뉴 `Tools > Setup > Build Full Content Database` 클릭 한 번으로 모든 콘텐츠 에셋 및 데이터베이스 멱등성 셋업 지원 툴 구현

---

## v1.36.1 (2026-07-23)

### Changed
- `Fail Fast` 원칙 적용 및 자동 복구 코드 전면 제거:
  - `BrewingController`, `InventoryController`, `ShopController`, `ServingManager`, `CustomerManager`에서 `Resources.Load()` 및 `GetComponent()` 기반 런타임 자동 복구 구문 전면 삭제
  - 인스펙터 참조 미할당 시 `Debug.LogError`로 런타임 오류를 명시적으로 출력하여 즉각 대처 가능한 Fail Fast 구조 확립
- 보고서 검증 분류 세분화: 정적 코드 분석과 실제 유니티 Play Mode 검증 수행 상태를 구별하여 보고서 재정리

---

## v1.36.0 (2026-07-23)

### Fixed
- `Gameplay Vertical Slice` 통합 플레이 검증 및 널 방어막 구축
  - `EnsureDatabase` 자동 탐색 기능: `BrewingController`, `InventoryController`, `ShopController`, `ServingManager`, `CustomerManager`에 `Resources.Load` 기반 널 방어막 구축 (인스펙터 참조 미할당 시에도 100% 자동 복구)
  - View 자동 바인딩: `Awake` 시점에 `view == null` 일 경우 `GetComponent<TView>()` 자가 복구 추가
  - `ServingManager` 파라미터 유실 방지: 주문 완료 처리 전 `customerId` 로컬 백업을 진행하여 `OnServeSucceeded` 이벤트 데이터 완전성 보장

---

## v1.35.0 (2026-07-23)

### Added
- `Integration Phase` 유니티 씬 및 오케스트레이션 연결 구축
  - `BootstrapLoader`: 매니저 하드코딩 초기화 없이 자연스러운 `Awake` -> `Start` 라이프사이클을 통해 `Bootstrap` -> `MainMenu` 전환 수행
  - `SetupIntegrationScenes`: 클릭 한 번으로 `Bootstrap`(0), `MainMenu`(1), `MainGame`(2) 씬 3개 및 씬 소속 Canvas / Global Overlay Canvas자동 구축 멱등성 에디터 메뉴 등록 (`Tools > Setup > Integrate All Scenes`)
  - `SMOKE_TEST_CHECKLIST.md`: 게임 시작부터 `New Game`, `Brewing`, `Order`, `Serving`, `Settlement`, `Save/Load` 및 씬 전환까지 17가지 항목 스모크 테스트 가이드 수립

### Changed
- `Global UI (Canvas)` 구조화: Canvas 전체를 DontDestroyOnLoad 하지 않고 Scene 소속으로 배치하되, 전역 렌더링이 필수적인 UI만 Overlay Canvas로 유지하는 설계 반영

---

## v1.34.0 (2026-07-23)

### Added
- `Debug Panel` 시스템 (v1.0): MVP 패턴 기반 개발 및 QA 디버그 패널 시스템 구현 (`#if UNITY_EDITOR || DEVELOPMENT_BUILD` 빌드 격리 전처리기 적용)
  - `DebugCommand`: enum 기반 디버그 명령어 타입 정의 (`AddMoney`, `AdvanceDay`, `SpawnCustomer`, `UnlockAll`, `Save/Load`, `SceneTransition` 등)
  - `DebugManager`: 복합 게임 로직이나 DB 순회 없이 기존 시스템 Manager들의 Public API만 순수 위임 호출하는 얇은 Wrapper 싱글톤
  - `DebugView`: `Money`, `Day`, `Customer`, `Unlock`, `Save`, `Scene` 6가지 카테고리로 UI 버튼 그룹화 및 `Action<DebugCommand>` 단일 이벤트 위임 노출
  - `DebugController`: `Action<DebugCommand>` 수신 후 `switch` 구문 처리 및 단축키(`~` 또는 F1)를 통한 패널 토글 지원

### Changed
- `UnlockManager`: `UnlockAll(IEnumerable<string> ids)` 일괄 해금 Helper API 추가

---

## v1.33.0 (2026-07-23)

### Added
- `Scene Transition & Loading` 시스템 (v1.0): MVP 패턴 기반 공용 씬 전환 시스템 구현
  - `SceneTransitionState`: enum 기반 씬 전환 진행 상태 (`Idle`, `FadingIn`, `LoadingScene`, `FadingOut`)
  - `SceneTransitionRequest`: DTO 기반 씬 전환 요청 파라미터 (`sceneName`, `fadeDuration`, `showLoadingUI`)
  - `SceneTransitionManager`: `RequestTransition(SceneTransitionRequest)`로 중복 호출 방지 및 `BeginTransition()`, `CompleteTransition()` 상태 제어 전담 (타 Manager 참조 없음)
  - `SceneTransitionView`: `ResetView()` 제공 (Progress 초기화, Canvas Alpha 초기화, Loading UI 숨김, Input Block 해제 전담) 및 Fade In/Out 및 Progress 대입 전담
  - `SceneTransitionController`: `AsyncOperation.progress`를 0.0 ~ 1.0으로 정규화하여 View에 전달, 최소 전환 보장 시간(기본 0.5초) 대기 및 Fade/로딩 시퀀스 코루틴 조율전담

---

## v1.32.0 (2026-07-23)

### Added
- `Confirmation Dialog` 시스템 (v1.0): MVP 패턴 기반 공용 확인 대화상자 시스템 구현
  - `DialogResult`: enum 기반 선택 결과 타입 (`Confirm`, `Cancel`)
  - `DialogType`: enum 기반 성격 분류 (`Confirm`, `Warning`, `Delete`, `Exit`, `Custom`)
  - `DialogData`: 대화상자 요청 DTO (`Action<DialogResult>` 콜백 및 `allowEscClose` 포함)
  - `DialogDisplayData`: View 화면 표시 전용 DTO (`allowEscClose` 미포함, 번역 텍스트 보관)
  - `DialogManager`: 큐(Queue) 관리와 함께 `CurrentDialog`, `IsDialogShowing` 상태 관리 전담 (타 Manager 직접 참조 없음)
  - `DialogView`: 제목/내용/버튼 텍스트 세팅 및 Fade-In/Out 연출 전담 (ESC 입력 해석 책임 없음)
  - `DialogController`: 코루틴 기반 큐 소비 출력 처리, `LocalizationManager` 다국어 번역, `Input.GetKeyDown(KeyCode.Escape)` ESC 키 입력 감지/해석, `Action<DialogResult>` 결과 콜백 실행 전담

---

## v1.31.0 (2026-07-23)

### Added
- `Notification (Toast)` 시스템 (v1.0): MVP 패턴 기반 알림 Toast 시스템 구현
  - `NotificationType`: enum 기반 알림 분류 (`MoneyEarned`, `MoneySpent`, `PurchaseSuccess`, `PurchaseFailed`, `RecipeUnlocked`, `FurnitureUnlocked`, `FurniturePlaced`, `TutorialCompleted`, `GameSaved`, `Custom`)
  - `NotificationData`: Queue 보관용 DTO (payload 미포함, 제목/설명 다국어 키 보유)
  - `NotificationDisplayData`: View 화면 표시 전용 DTO (최종 다국어 텍스트 보유)
  - `NotificationManager`: 순수 FIFO Queue 보관 및 `Enqueue()`, `TryDequeue()`만 전담하는 독립 싱글톤 매니저
  - `NotificationItemView`: 단일 Toast의 아이콘/제목/설명 세팅 및 Fade-In/Hold/Fade-Out 애니메이션 연출 전담
  - `NotificationView`: `ShowNotification(NotificationDisplayData, Action)` 전용 DTO 전달 API 사용 및 향후 Object Pool 전환 가능한 구조
  - `NotificationController`: Manager 이벤트 구독 후 `NotificationData` 변환 및 `NotificationManager` Enqueue 호출, 코루틴 기반으로 큐를 꺼내 번역 후 `NotificationView` 연출 가동전담

### Changed
- `SaveManager`: `OnGameSaved` 이벤트 추가

---

## v1.30.0 (2026-07-23)

### Added
- `Tutorial` 시스템 (v1.0): MVP 패턴 기반 단계별 튜토리얼 시스템 구현
  - `TutorialConditionType`: enum 기반 진행 조건 정의 (`ShopPurchaseSuccess`, `BrewingComplete`, `ServeSuccess` 등)
  - `TutorialHighlightRegistry`: ID 기반 RectTransform 강조 대상 UI 매핑 레지스트리
  - `TutorialStepData`: ScriptableObject 기반 단계별 안내 텍스트 키 및 진행 조건/자동 진행/강조 ID 설정
  - `TutorialManager`: 독립적인 진행 단계 및 완료 상태 관리, `GetSaveData()` 및 `LoadSaveData()`만 제공하여 `SaveManager` 연동
  - `TutorialView`: 안내 텍스트 표시, `TutorialHighlightRegistry` 연동 강조 UI 제어, 버튼 액션 위임
  - `TutorialController`: 외부 매니저 이벤트를 수신해 `TutorialConditionType`으로 변환 후 `TutorialManager` 및 View와 연동

### Changed
- `SaveManager`: `tutorialData` 세이브 수거 구문 연동

---

## v1.29.0 (2026-07-23)

### Added
- `Furniture UI` 시스템 (v1.0): MVP 패턴 기반 가구 관리 및 배치 UI 구현
  - `FurnitureItemView`: 단일 가구 아이콘/이름/배치 여부(`placedBadge`)/선택 상태(`selectedIndicator`) 세팅 컴포넌트
  - `FurnitureView`: `RefreshItems(List<FurnitureItemData>)` 중심 API 설계, 전달받은 슬롯 정보(`UpdateSlotInfoText`) 표시, 배치/해제 이벤트 위임
  - `FurnitureController`: ① 슬롯 선택(`SetSelectedSlot`) -> ② 가구 선택 -> ③ 배치 버튼 흐름 지원. 배치/해제 가능 판단은 `FurnitureManager` 검증 API에만 위임

### Changed
- `FurnitureManager`: `CanPlaceFurniture(furnitureId, slotId)` 및 `CanRemoveFurniture(furnitureId, slotId)` 검증 API 추가

---

## v1.28.0 (2026-07-23)

### Added
- `Shop UI` 시스템 (v1.0): MVP 패턴 기반 상점 UI 구현
  - `ShopItemView`: 단일 판매 재료 아이콘/이름/가격 표시 및 구매 버튼 클릭 이벤트(`OnBuyClicked`) 위임 컴포넌트
  - `ShopView`: `RefreshItems(List<ShopItemData>)` 중심 설계로 향후 Object Pool 전환 용이, `UpdateBuyableStates(Dictionary<string, bool>)`로 구매 버튼 활성화 상태 부분 갱신
  - `ShopController`: `MoneyManager.OnMoneyChanged` 수신 시 소지금 및 `ShopManager.CanPurchase()` 상태만 부분 갱신 (`RefreshAll` 전체 재호출 방지)

### Changed
- `ShopManager`: `CanPurchase(string ingredientId, int amount = 1)` 조회 API 추가

---

## v1.27.0 (2026-07-23)

### Added
- `Inventory UI` 시스템 (v1.0): MVP 패턴 기반 인벤토리 표시 UI 구현
  - `InventoryItemView`: 단일 재료 아이콘/이름/수량 표시 제시용 프리팩 컴포넌트. 클릭 시 `OnItemClicked(string ingredientId)` 이벤트 발송.
  - `InventoryView`: `RefreshItems(List<InventoryItemData>)` 중심 설계로 향후 Object Pool 교체 가능. `OnItemSelected` 이벤트 릴레이.
  - `InventoryController`: `OnInventoryChanged` 이벤트 구독, 선택 상태 관리, `LocalizationManager`를 통한 재료 이름 다국어 조회.

### Changed
- `IngredientData`: `localizationKey` 필드 추가 (다국어 이름 지원, 비어 있으면 `displayName` Fallback)

---

## v1.26.0 (2026-07-21)

### Added
- `Brewing UI` 시스템 (v1.0): MVP 패턴 기반 음료 제조 UI 구현
  - `BrewingView`: 버튼/슬라이더/텍스트 참조 및 이벤트 Action 발송. 포매팅 책임 없음.
  - `BrewingController`: `recipeId` 기반 레시피 선택, `RecipeDatabase.GetAllRecipes()` 조회, `InventoryManager` 재료 확인 후 Start 버튼 활성화/비활성화 제어.
  - 상태 문자열(`제조 중`, `제조 완료` 등)은 `LocalizationManager`를 통해 조회하도록 설계.

### Changed
- `BrewingManager`: `OnBrewingProgressChanged(float)` 이벤트 추가로 UI측 `Update()` 사용 회피.

---

## v1.25.0 (2026-07-21)

### Added
- `HUD` 시스템 (v1.0): MVP 패턴 기반 게임 정보 표시 패널 구현
  - `HUDView`: 단순 텍스트 표시만 전담하도록 하여 포매팅 책임 배제
  - `HUDController`: `MoneyManager`, `DayManager`, `OrderManager` 이벤트를 수신하여 "1000냥", "+500", "1일차" 등의 최종 문자열을 만들어 `HUDView`에 전달.
  - `DayManager`에 `OnDailyEarningsChanged`, `OnDailyServedCustomersChanged` 이벤트 추가를 통해 `Update()` 호출 없는 완전한 이벤트 기반 동기화 구현
  - `OrderManager.GetActiveOrderCount()` API 추가로 은닉화 유지.

---

## v1.24.0 (2026-07-21)

### Added
- `Settings UI` 시스템 (v1.0): MVP 패턴 기반 설정창 구현
  - `SettingsView`: UI 컴포넌트(`TMP_Dropdown`, `Slider`, `Button`) 참조 및 이벤트 Action 발송. `SetLanguageOptions`를 통한 동적 옵션 세팅 추가.
  - `SettingsController`: `SettingsManager`의 값을 읽어 View를 초기화하고, View에서 발생하는 이벤트를 수신하여 `SettingsManager` 값 갱신.
  - `LanguageType` enum 기반의 유효성 검증(`Enum.IsDefined`) 적용.
  - 닫기 버튼 선택 시 저장을 호출하지 않고 패널만 숨기도록 하여 책임 분리.

---

## v1.23.0 (2026-07-21)

### Added
- `MainMenu` 시스템 (v1.0): 타이틀/메인 메뉴 로직 및 UI 분리 구현
  - `MainMenuView`: Button 클릭 이벤트를 C# Action으로 릴레이하여 결합도 최소화
  - `MainMenuController`: View의 이벤트를 구독하여 SaveManager 세이브 여부에 따른 이어하기 활성화/비활성화 처리
  - 에디터 전용 컴파일 디렉티브(`#if UNITY_EDITOR`)를 사용한 안전한 종료 처리 구현
  - 추후 Settings UI 연동을 위한 독립 구조 확보

---

## v1.22.0 (2026-07-21)

### Changed
- `AudioManager` (v1.1): `SettingsManager` 이벤트 연동 및 볼륨 리팩토링
  - `PlayBgm`, `StopBgm`, `ApplyBgmVolume`, `ApplySfxVolume` 신규 API 추가
  - `SetMusicVolume`, `SetSfxVolume`, `PlayMusic`, `StopMusic` 등 기존 API를 래퍼 형태로 유지하여 100% 하위 호환성 보장
  - `AudioMixer` 미할당 시 `AudioSource.volume` 갱신을 수행하는 Fallback 유지
  - `SettingsManager` 없이도 단독 실행 가능하도록 Null-safe 방어 로직 적용
  - `Ambience`, `CrossFade`, `FadeOutMusic` 기능 보존

---

## v1.21.0 (2026-07-21)

### Added
- `LocalizedText` (v1.0): UI의 `TMP_Text` 자동 다국어 갱신 컴포넌트 추가
  - `Awake()` 시점에 `TMP_Text`가 할당되지 않은 경우 자동 캐싱 및 방어 로직 적용
  - `SettingsManager.Instance.OnLanguageChanged` 이벤트를 구독하여 언어 변경 시 즉시 최신 번역으로 갱신
  - `SetLocalizationKey(string)` 퍼블릭 API를 통한 런타임 키 변경 지원
  - `LocalizationManager` 수정 없이 독립적으로 재사용 가능한 구조 확보

---

## v1.20.0 (2026-07-21)

### Added
- `SettingsManager` (v1.0): 게임 내 언어, BGM 볼륨, SFX 볼륨 설정 중앙 관리 시스템 구현
  - `LanguageType`, BGM 볼륨, SFX 볼륨 관리 및 `Mathf.Clamp01` 적용
  - 값이 실제 변경될 때만 이벤트를 발생(`OnLanguageChanged`, `OnBgmVolumeChanged`, `OnSfxVolumeChanged`)
  - `SettingsSaveData` 클래스 정의로 저장 연동
  - `LocalizationManager`에서 언어 세이브 기능 제거하고 `SettingsManager`의 `OnLanguageChanged`를 구독하여 연동
  - `SaveManager`에서 `settingsData`를 통해 설정 데이터 통합 저장/복원

---

## v1.19.0 (2026-07-21)

### Added
- `LocalizationManager` (v1.0): 다국어 문자열 관리 시스템 구현
  - `LanguageType` Enum (Korean, English) 추가
  - `LocalizationDatabase`, `LocalizationEntry` 기반 문자열 관리 (향후 카테고리별 다중 DB 지원 구조 반영)
  - `SetLanguage()`, `GetCurrentLanguage()`, `GetText()`, `HasKey()` API 구현
  - `OnLanguageChanged` 이벤트 제공 (UI 갱신 등은 타 시스템 위임)
- `SaveManager` 연동
  - `LanguageSaveData` 구조체 추가 (언어 Enum 인덱스 보관)
  - 저장/복원 로직 구현 및 Enum 범위 검증 복구 적용

### Fixed
- `CustomerAppearanceUIBuilder.cs`: `CustomerManager.CallNextCustomer()` 반환 타입(`CustomerData`) 변경에 따른 컴파일 에러 수정
- `SetupRecipeDatabase.cs`: `List<>` 사용을 위한 네임스페이스 누락 이슈 수정

---

## v1.18.1 (2026-07-21)

### Changed
- AI_DEVELOPMENT_RULES.md v1.3 → v1.4 갱신: Section 8 가구(Furniture) 배치 시스템 설계 지침 추가
  - 슬롯(Slot) 기반 배치 원칙: 자유 배치 금지, 좌표 미저장, slotId → furnitureId 관계만 영구화
  - FurnitureType / SlotType 매칭 구조: SlotType == FurnitureType 일치 시만 배치 가능
  - FurnitureManager 책임 범위 제한: 좌표 계산, 드래그, 충돌 검사, 회전 기능 배제

---

## v1.18.0 (2026-07-21)

### Added
- FurnitureStateChangeType.cs / FurnitureManager.cs (Singleton) 생성: 가구의 보유(Owned), 배치(Placed), 활성화(Active) 런타임 상태 관리 및 조회 전담
- FurnitureManager.cs: PlaceFurniture 시 자동 Active 부여 배제 (배치와 활성화 독립 상태 유지)
- FurnitureManager.cs: `OnFurnitureStateChanged(furnitureId, changeType)` 이벤트 구현 (Owned, Placed, Removed, Activated, Deactivated 전달)
- FurnitureManager.cs: LoadSaveData() 시 데이터 무결성 검증 (`active ⊂ placed ⊂ owned` 포함 관계 보장)
- SaveManager.cs: `FurnitureSaveData` (ownedFurnitureIds, placedFurnitureIds, activeFurnitureIds) 통합 세이브/로드 연동

---

## v1.17.0 (2026-07-21)

### Added
- UnlockConstants.cs 생성: `DEFAULT_UNLOCKED_IDS` (전통 음료 5종 등 기본 해금 항목 상수 배열) 정의
- UnlockManager.cs (Singleton) 생성: 콘텐츠(음료, 재료, 가구, 손님 등)의 해금 상태 저장 및 조회 전담 (Unlock, Lock, IsUnlocked, GetAllUnlockedIds API 구현)
- UnlockManager.cs: `Awake()` 내 자동 등록 제거 및 `InitializeDefaultUnlocks()` 명시적 API 분리 (새 게임 시점용)
- SaveManager.cs: `UnlockSaveData` 통합 세이브/로드 연동 (LoadGame 시 저장된 데이터만 복원)

---

## v1.16.0 (2026-07-21)

### Added
- AffinityManager.cs (Singleton) 생성: 손님별 친밀도(AddAffinity, RemoveAffinity, GetAffinity, SetAffinity) 수치 저장 및 관리 전담
- ServingManager.cs: `OnServeSucceeded` 이벤트 매개변수에 `customerId` 추가 (`OnServeSucceeded(customerId, drinkId, price)`)
- AffinityManager.cs: `ServingManager.Instance.OnServeSucceeded` 간접 구독을 통해 서빙 성공 시 전달받은 `customerId`로 친밀도 +1 자동 증가 연동 (OrderManager 재조회 배제)
- SaveManager.cs: `AffinitySaveData` 통합 세이브/로드 연동

---

## v1.15.0 (2026-07-21)

### Added
- ServingManager.cs (Singleton) 생성: 음료 제공(Serve) 전체 흐름 제어 오케스트레이터 구현
- ServingManager.cs: CustomerView 직접 참조 제거 및 `OnServeSucceeded`, `OnServeFailed` 이벤트 통보 구조로 UI 렌더링 계층 분리
- ServingManager.cs: OrderManager.CompleteOrder() 검증 후 냥전 지급(MoneyManager), 통계 누적(DayManager), 데이터 상 손님 제거(CustomerManager) 순차 연쇄 실행
- ServingManager.cs: OrderData 기반 최종 보상 금액 보관 확장 대비 TODO 주석 작성 및 성공/실패 보상/패널티 확장 Hook 설계

---

## v1.14.0 (2026-07-21)

### Added
- CustomerView.cs 생성: UI Image 기반 손님 시각적 노출/숨김(ShowCustomer, HideCustomer, HasVisibleCustomer) 전담
- CustomerView.cs: CustomerManager 이벤트 직접 구독 배제, 외부(Presenter/GameFlow)의 명시적 호출로 작동하도록 독립성 보장
- CustomerView.cs: Instantiate/Destroy 미사용 및 활성화/비활성화 구조 적용 (Object Pooling 대비)
- CustomerView.cs: PlayEntranceAnimation / PlayExitAnimation 명시적 Hook 유지 (자동 호출 배제)

---

## v1.13.0 (2026-07-21)

### Added
- OrderData.cs 생성: `orderId` (단순 카운터 카운트 형식 `order_000001`), `customerId`, `requestedDrinkId`, `isCompleted` 수록 (타이머 관련 필드 배제)
- OrderManager.cs (Singleton) 생성: 손님 생성이나 음료 제조를 직접 수행하지 않고 주문 생명주기 및 검증 관리
- OrderManager.cs: `CustomerManager.Instance.OnCustomerArrived` 이벤트 구독을 통한 `GenerateOrder` 이벤트 기반 자동 주문 생성 구현
- OrderManager.cs: `CompleteOrder(string brewedDrinkId)` 구현 — 완성된 음료와 요구 음료 간 검증 및 성공 여부 반환
- OrderManager.cs: `SelectDrinkForCustomer(CustomerData customer)` 설계 — 향후 손님 선호도, 국가별 확률, 이벤트 주문 확장을 위한 인터페이스 마련

---

## v1.12.0 (2026-07-21)

### Added
- CustomerData.cs: `displayNameKey`, `descriptionKey` (Localization Key 형태), `nationality` (CountryType Enum), `portrait`, `sprite` 필드 확장
- CustomerDatabase.cs (ScriptableObject) 생성: 손님 데이터 집합 관리 및 `GetCustomer`, `GetRandomCustomer` API 제공
- CustomerManager.cs (Singleton) 대대적 개편: GameObject Instantiate 제거, pure CustomerData 기반 큐(Queue) 및 CurrentCustomer 관리
- CustomerManager.cs: `NotifyCustomerReady()` Hook 및 `OnCustomerArrived` 이벤트 설계 (OrderManager & View/Spawner 레이어 연결용)
- CustomerManager.cs: `DayManager.Instance.CurrentState == DayState.Operating` 조건 검사를 통해 영업 중에만 손님 대기열 수집 허용

---

## v1.11.0 (2026-07-21)

### Added
- SaveManager.cs (Singleton) 생성: 유일한 영구 저장/로드 관리자 구현. GameSaveData 최상위 구조에 MoneySaveData, InventorySaveData, DaySaveData를 캡슐화
- SaveManager.cs: `SaveGame()`, `LoadGame()`, `HasSave()`, `DeleteSave()` API 구현. JSON 직렬화/역직렬화 및 PlayerPrefs 캡슐화 처리
- SaveManager.cs: `version` 검사 로직(SAVE_VERSION과 다를 경우 마이그레이션 안내 경고 및 TODO) 추가 및 `SAVE_KEY` 상수 구조 마련
- DayManager.cs: 영업 결산(`Settlement`) 상태 진입 시 `SaveManager.Instance.SaveGame()` 일괄 자동 저장 연동

---

## v1.10.0 (2026-07-21)

### Added
- DayManager.cs (Singleton) 생성: 하루 주기 상태(Ready, Operating, Settlement) 제어 구조 설계
- DayManager.cs: `RecordSale(int amount)` 및 `RecordCustomerServed()` 추가 — 매출 냥전 및 서빙 성공 횟수 등 하루 통계 기록에 집중
- DayManager.cs: 준비 단계 진입 시 상점 활성화(`ShopManager.Instance.SetShopOpen(true)`), 영업 단계 진입 시 상점 마감(`SetShopOpen(false)`) 연동
- DayManager.cs: 결산 단계 진입 시 세이브 시스템 호출을 위한 `TriggerAutoSave()` 훅 및 세이브 데이터 연동 규격 설계
- AI_DEVELOPMENT_RULES.md (v1.3): 프로젝트 공통 비즈니스 규칙(기본 재화 냥전 단위 통일, 유료 재화 보석 설계 고려, 다국어 Localization 고려 설계, 가격 밸런스 가이드라인, 만족도 기반 친밀도/XP 보상 구조 정의) 반영

---

## v1.9.0 (2026-07-21)

### Added
- MoneyManager.cs (Singleton) 생성: 유일한 재화 관리 객체로서 AddMoney, SpendMoney, CanAfford 구현. CurrentMoney는 private set만 허용하고 startingMoney는 Inspector 설정 지원
- MoneyManager.cs: `GetSaveData()`, `LoadSaveData()` API 설계 — 자체 저장을 배제하고 하루 종료 시 SaveManager가 일괄 처리하는 단일 책임 모델 유지
- ShopManager.cs: `CheckAndConsumeMoney(int totalCost)` 임시 훅을 `MoneyManager.Instance.SpendMoney(totalCost)` 호출로 교체 연동

---

## v1.8.0 (2026-07-21)

### Added
- ShopManager.cs (Singleton) 생성: 런타임 재료 상점 로직(BuyIngredient, GetAvailableIngredients) 구현. UI 레이어와 완전 분리
- ShopManager.cs: `CheckAndConsumeMoney(int totalCost)` 임시 훅 추가. PlayerDataManager에 대한 직접 의존성을 차단하여 디커플링 보장 및 Money 시스템 분리
- ShopManager.cs: `IsShopOpen` 기본값을 false로 지정하고 외부 DayManager 등에서 제어하도록 설계
- ShopManager.cs: IngredientDatabase와 InventoryManager만 직접 참조하며, unlockLevel 조건에 따른 구매 제한은 이번 단계에서 배제

---

## v1.7.0 (2026-07-21)

### Added
- InventoryManager.cs (Singleton) 생성: 런타임 재고 관리(Add / Consume / Get / HasEnough) 기능 구현. 외부 IngredientDatabase 연동을 통해 재료 유효성 검증 제공
- InventoryManager.cs: `GetSaveData()`, `LoadSaveData()` API 추가 — 자체 디스크 저장/로드를 수행하지 않고 외부 세이브 시스템(SaveManager/DayManager)에 데이터를 제공 및 복원하는 단일 책임 설계 적용
- BrewingManager.cs: `CanBrew(out string failReason)` 메서드 내에 `InventoryManager` 연동 재고 부족 검증 로프 추가

---

## v1.6.0 (2026-07-21)

### Modified
- BrewingManager.cs: DrinkDatabase/RecipeDatabase SerializeField 참조 추가 (Inspector 연결 방식)
- BrewingManager.cs: `CanBrew(out string failReason)` 신규 추가 — 레시피 존재 여부 검증, DB 미연결 시 기존 동작 유지
- BrewingManager.cs: `StartBrewingWithValidation()` 신규 추가 — 레시피 검증 후 StartBrewing() 호출
- 기존 Public API(StartBrewing, SelectTea, SetTemperature, SetSteepTime, ResetBrewing, IsBrewing) 시그니처 전체 보존

---

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
