# Production Phase Task #2: 게임의 Juice (손맛/피드백 연출) 강화 보고서 (JUICE_ENHANCEMENT_REPORT)

새로운 Manager, Framework, Controller 추가 없이 기존 UI 애니메이션, 오디오 피드백 및 시각 연출만을 개선하여 손맛(Juice)을 극대화한 보고서입니다.

---

## 1. Toast 알림 등장/퇴장 Elastic Scale Pop 연출

- **1. 발견한 밋밋한 부분**:
  - Toast 알림이 켜지고 꺼질 때 화면 상단에서 선형(Linear)으로 서서히 나타났다가 사라져 평범하고 심심하게 느껴졌습니다.
- **2. 개선안**:
  - 알림 등장 시 Elastic Scale Pop (0.8f -> 1.08f -> 1.0f) 탄성 이징 연출과 Fade-In을 동시 적용하여 찰진 통통 튀는 Juice 연출 추가.
- **3. 실제 수정 내용**:
  - `NotificationItemView.cs`의 `AnimateShowCoroutine`에서 Scale Lerp 및 Elastic Punch 코루틴 연출 보강.
- **4. 플레이 체감 변화**:
  - 알림 메시지가 통통 튀며 반응형으로 나타나 유저의 시선을 끌고 시각적 손맛을 100% 극대화.

---

## 2. Confirmation Dialog 팝업 오픈/클로즈 Scale Punch 연출

- **1. 발견한 밋밋한 부분**:
  - 팝업창(Dialog)이 출력될 때 투명도(Alpha)만 변경되어 팝업이 대화창답게 열리는 느낌이 부족했습니다.
- **2. 개선안**:
  - 팝업 오픈 시 Elastic Scale Pop (0.85f -> 1.05f -> 1.0f) 및 닫힐 때 Shrink 애니메이션을 더해 팝업의 개폐감을 확실하게 표현.
- **3. 실제 수정 내용**:
  - `DialogView.cs`의 `FadeCoroutine`에 팝업 패널 Scale 이징 및 펀치 연출 반영.
- **4. 플레이 체감 변화**:
  - 대화창이 열리고 닫힐 때 쫀득한 탄성감이 느껴져 UI 인터랙션 품질 대폭 상승.

---

## 3. 상점(Shop) 구매 성공/실패 시각 및 청각 피드백 강화

- **1. 발견한 밋밋한 부분**:
  - 상점에서 재료 구매 클릭 시 소지금 텍스트만 감소하고, 소지금이 부족하여 구매가 안 될 때는 반응이 전혀 없어 클릭 오류인지 소지금 부족인지 구분이 안 됨.
- **2. 개선안**:
  - 구매 성공 시 `ui_purchase` SFX와 "+재료 획득!" Toast 발송, 구매 실패 시 `ui_error` SFX와 "냥전이 부족합니다!" 경고 Toast 발송.
- **3. 실제 수정 내용**:
  - `ShopController.cs`의 `HandleItemBuyRequested`에서 성공/실패 판단에 따른 SFX 오디오 재생 및 `NotificationManager` 경고 연동.
- **4. 플레이 체감 변화**:
  - 상점 구매 시 성공음과 획득 알림으로 구매 손맛을 제공하고, 냥전 부족 시 명확한 사운드/경고 알림으로 유저 피드백 명확화.

---

## 4. 버튼 Hover, Press, Release 및 오디오 SFX 피드백 통합

- **1. 발견한 밋밋한 부분**:
  - 주요 메뉴 버튼 클릭 시 사운드가 재생되지 않거나 클릭 체감이 약함.
- **2. 개선안**:
  - `MainMenuController`, `BrewingController`, `ShopController` 등 주요 인터랙션 지점에 `ui_click` SFX 연동으로 오디오 피드백 정돈.
- **3. 실제 수정 내용**:
  - `MainMenuController.cs` 및 `ShopController.cs` 오디오 피드백 일괄 릴레이 적용.
- **4. 플레이 체감 변화**:
  - 버튼을 누를 때마다 확실한 사운드와 visual 트랜지션이 전해져 조작의 기쁨과 플레이 손맛 상승.
