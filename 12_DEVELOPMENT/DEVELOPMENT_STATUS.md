# 개발 진행 상황 - v0.3.0 (개발 시작)

## Step 1: 프로젝트 기본 구조 생성 ✅

[생략...]

---

## Step 2: 메인 화면 구현 ✅

[생략...]

---

## Step 3: 타이틀 화면 구현 ✅

### 완료 항목

#### A. 타이틀 씬 생성
- `Title.unity` - Canvas와 Camera가 포함된 타이틀 씬

#### B. 스크립트 구현
- `TitleScreenManager` - 타이틀 화면 관리
  - 새 게임 시작
  - 게임 계속하기 (저장된 데이터 있을 경우)
  - 플레이어 이름 설정 팝업
  - 설정 버튼
- `TitleScreenUIBuilder` - 타이틀 UI 동적 생성
  - 타이틀 텍스트
  - 메인 버튼들 (새 게임, 계속하기, 설정)
  - 이름 입력 패널 (모달)
  - 확인/기본값 사용 버튼

#### C. 기능
- **새 게임**: 플레이어 이름 설정 팝업 표시
- **게임 계속**: 저장된 PlayerData로 게임 재개
- **이름 설정**: 입력 또는 기본값(orangeCat) 사용
- **씬 전환**: Title → Main

### UI 구조
```
Title.unity (Canvas)
├─ Background (크림색)
├─ Title (게임명 표시)
├─ MainButtonPanel
│  ├─ StartButton (새 게임)
│  ├─ ContinueButton (계속하기, 저장 데이터 있을 때만)
│  └─ SettingsButton (설정)
└─ NameInputPanel (모달, 초기: 숨김)
   └─ Form
      ├─ Title (캐릭터 이름)
      ├─ InputField (입력)
      ├─ ConfirmButton (확인)
      └─ SkipButton (기본값 사용)
```

### GameManager 수정
- `Start()` → 게임 시작 시 Title 씬 자동 로드

### 파일 목록
```
Assets/Scripts/UI/TitleScreenManager.cs
Assets/Scripts/UI/TitleScreenUIBuilder.cs
Assets/Scenes/Title.unity
Assets/Scripts/Core/GameManager.cs (수정)
```

---

## 다음 단계
**Step 4: 튜토리얼 구현**
- Tutorial 씬 생성 또는 메인에서 인게임 튜토리얼
- 첫 손님 대기 → 주문 → 제조 → 평가까지의 안내
- 건너뛰기 옵션

---

## 기술 스택 최종 확인

- **Engine**: Unity 6 LTS
- **Language**: C#
- **Render**: 2D (Canvas 기반 UI)
- **Input**: Unity Input System (나중 추가)
- **UI**: uGUI (Canvas)
- **Animation**: Animator (나중 추가)
- **Save**: JSON + PlayerPrefs
- **Database**: ScriptableObject (나중 추가)
- **Version Control**: Git + GitHub
- **IDE**: VS Code

---

## 주의사항

1. 모든 기능은 dev 브랜치에서 개발
2. Placeholder 이미지/색상 사용 (디자인은 나중)
3. 구조를 우선 (완성도보다 기능 먼저)
4. 한 번에 한 Step만 구현
5. 각 Step 완료 후 다음 Step 진행

---

## Step 4: 튜토리얼 구현 ✅

### 완료 항목

#### A. 튜토리얼 시스템
- `TutorialManager` - 튜토리얼 진행 관리
  - PlayerPrefs로 완료 여부 저장
  - 단계별 대사 진행
  - 다음/건너뛰기 버튼
- `TutorialUIBuilder` - 튜토리얼 UI 동적 생성
  - 반투명 배경 (포커스)
  - 대화 박스 (하단)
  - 다음/건너뛰기 버튼

#### B. 기능
- **첫 플레이만 표시**: PlayerPrefs.HasKey("TutorialCompleted") 확인
- **5단계 대사**:
  1. 환영 인사
  2. 할머니로부터 물려받은 다방
  3. 손님 주문 → 정확히 만들기
  4. 손님 대기 버튼 누르기
  5. 준비 완료
- **다음/건너뛰기**: 선택 가능
- **완료 후**: GameState.Playing으로 변경

#### C. 통합
- MainScreenUIBuilder에서 TutorialUIBuilder 자동 실행
- Main 씬 실행 시 자동으로 UI 생성

### 파일 목록
```
Assets/Scripts/UI/TutorialManager.cs
Assets/Scripts/UI/TutorialUIBuilder.cs
Assets/Scripts/UI/MainScreenUIBuilder.cs (수정)
```

### 튜토리얼 UI 구조
```
TutorialPanel (반투명 배경)
└─ DialogBox (하단 대화 박스)
   ├─ Text (튜토리얼 메시지)
   ├─ NextButton (다음)
   └─ SkipButton (건너뛰기)
```

---

## Step 5: 손님 시스템 구현 ✅

### 완료 항목

#### A. 손님 데이터 구조
- `CustomerData` - ScriptableObject 기반 손님 데이터
  - 이름, 캐릭터 타입
  - 선호하는 차, 온도, 우림시간
  - 성격, 스토리
- `Customer` - 게임 중 나타나는 손님 인스턴스
  - 친숙도 레벨 (0~5)
  - 방문 횟수
  - 만족도 계산

#### B. 손님 매니저
- `CustomerManager` (싱글톤) - 모든 손님 관리
  - 손님 데이터 로드 (Resources 또는 기본값)
  - 무작위 손님 생성
  - 손님 호출 (대기 큐 → 현재 손님)
  - 손님 제거 (서빙 완료)
- `CustomerDataHelper` - 기본 손님 데이터 생성

#### C. 기본 5가지 손님
1. **Luna** (토끼) - 유자차, 낮은 온도, 짧은 우림
2. **Hyuntae** (사자) - 차이, 높은 온도, 긴 우림
3. **Wei** (판다) - 보이차, 중간 온도, 긴 우림
4. **Sakura** (여우) - 말차, 중간 온도, 짧은 우림
5. **Denu** (독수리) - 차이, 높은 온도, 긴 우림

#### D. 통합
- MainScreenManager: 손님 대기 버튼 → CustomerManager.SpawnRandomCustomer()
- CustomerManager: 게임 시작 시 자동으로 손님 데이터 로드

### 파일 목록
```
Assets/Scripts/Data/CustomerData.cs
Assets/Scripts/Data/CustomerDataHelper.cs
Assets/Scripts/Game/Customer.cs
Assets/Scripts/Game/CustomerManager.cs
Assets/Scripts/Game/MainScreenManager.cs (수정)
```

### 손님 시스템 아키텍처
```
CustomerManager (싱글톤)
├─ _allCustomers (모든 손님 데이터)
├─ _waitingCustomers (대기 중인 손님 큐)
└─ _currentCustomer (현재 서빙 중인 손님)

Customer (게임 중 인스턴스)
├─ data (CustomerData 참조)
├─ familiarityLevel (친숙도)
└─ _totalVisits (방문 횟수)
```

### 다음 단계
**Step 6: 주문 시스템 구현**
- 손님 주문 화면 (선택할 차 표시)
- 손님의 요청사항 표시 (온도, 우림시간)
- 손님 대화 (대사)
- 선택 완료 → 제조 화면으로 이동

---

## Step 6: 주방(차 제조) 시스템 구현 ✅

### 완료 항목

#### A. 제조 데이터 구조
- `BrewingData` - 현재 제조 상태
  - 선택한 차
  - 온도 (0=낮음, 1=중간, 2=높음)
  - 우림시간 (0=짧음, 1=중간, 2=길음)
  - 제조 진행률 (0~1)
  - 품질 (1~5별)

#### B. 제조 매니저
- `BrewingManager` (싱글톤)
  - 제조 데이터 관리
  - 차 선택 (SelectTea)
  - 온도/우림시간 설정 (SetTemperature, SetSteepTime)
  - 제조 시작/진행 (StartBrewing, Update)
  - 진행률 계산
  - 레이블 헬퍼 (온도, 우림시간, 차 이름)

#### C. 주방 UI
- `KitchenUIBuilder` - 주방 화면 동적 생성
  - 온도 선택 버튼 (낮음, 중간, 높음)
  - 우림시간 선택 버튼 (짧음, 중간, 길음)
  - 차 선택 버튼 (유자차, 말차, 보이차, 연꽃차, 차이)
  - 진행률 바
  - 제조 시작 버튼
  - 돌아가기 버튼

#### D. 진행률 표시
- `ProgressBarUpdater` - 실시간 진행률 바 업데이트
  - 너비로 진행률 표시 (0~800px)

#### E. 통합
- MainScreenManager: 손님 대기 버튼 → 주방 화면 진입

### 파일 목록
```
Assets/Scripts/Game/BrewingManager.cs
Assets/Scripts/UI/KitchenUIBuilder.cs
Assets/Scripts/Game/MainScreenManager.cs (수정)
```

### 주방 UI 레이아웃
```
[주방]
 
온도         [낮음]  [중간]  [높음]

우림시간     [짧음]  [중간]  [길음]

차 선택: [유자차] [말차] [보이차] [연꽃차] [차이]

[████████████] (진행률 바)

     [제조 시작]

[돌아가기]
```

### 다음 단계
**Step 7: 손님 등장**
- 손님 시각화 (스프라이트/placeholder)
- 손님 대사 화면
- 손님 선호도 표시
- 손님과 상호작용

---

## Step 7: 손님 등장 구현 ✅

### 완료 항목

#### A. 손님 등장 UI
- `CustomerAppearanceUIBuilder` - 손님 등장 화면 동적 생성
  - 손님 캐릭터 표시 (색상으로 구분)
  - 손님 이름 + 인사말
  - 손님 대사 (스토리 비트)
  - 선호도 표시 (차, 온도, 우림시간, 성격)
  - 주방으로 가기 버튼
  - 거절 버튼

#### B. 캐릭터 색상
- Luna (토끼): 황금색
- Hyuntae (사자): 주황색
- Wei (판다): 연초록색
- Sakura (여우): 분홍색
- Denu (독수리): 베이지색

#### C. 흐름
1. 메인 화면 → 손님 대기 버튼 클릭
2. 손님 등장 화면 (선호도 표시)
3. "주방으로 가기" → 주방 화면으로 이동
4. "거절" → 손님 제거, 메인 화면으로 돌아감

#### D. 통합
- MainScreenManager: 손님 대기 버튼 → 손님 등장 화면
- CustomerAppearanceUIBuilder: 주방으로 가기 → KitchenUIBuilder 호출
- CustomerManager: CallNextCustomer() 실행

### 파일 목록
```
Assets/Scripts/UI/CustomerAppearanceUIBuilder.cs
Assets/Scripts/Game/MainScreenManager.cs (수정)
```

### 손님 등장 UI 레이아웃
```
┌─────────────────────────────────┐
│  [황금색 박스]                   │
│   LUNA                          │
│                                 │
│ "과제 때문에 밤 늦게 방문"      │
│                                 │
│ 선호도                          │
│ [선호하는 차: 유자차]           │
│ [온도: 낮음] [우림시간: 짧음]  │
│ [성격: 수줍음, 예민함]         │
│                                 │
│  [주방으로 가기]  [거절]        │
└─────────────────────────────────┘
```

### 다음 단계
**Step 8: 주문 시스템**
- 손님이 실제로 주문 요청 (차 선택)
- 주방에서 손님 요청사항 표시
- 손님 선택과 플레이어 선택 비교

---

**상태**: Step 7 완료, Step 8 준비 중
