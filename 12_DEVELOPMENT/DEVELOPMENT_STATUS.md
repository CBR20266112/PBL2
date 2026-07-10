# 개발 진행 상황 - v0.3.0 (개발 시작)

## Step 1: 프로젝트 기본 구조 생성 ✅

### 완료 항목

#### A. 폴더 구조
- `Assets/Scripts/` - 모든 C# 스크립트
  - `Core/` - 핵심 시스템 (GameManager, Singleton, Constants)
  - `UI/` - UI 관리 시스템
  - `Game/` - 게임 로직 (나중 추가)
  - `Data/` - 데이터 모델 및 저장소
  - `Systems/` - 게임 시스템 (경제, 진행도 등, 나중 추가)
- `Assets/Resources/` - ScriptableObject, Prefabs, 데이터
- `Assets/Scenes/` - 게임 씬
- `Assets/Sprites/` - 임시 이미지 (나중 추가)
- `Assets/Animations/` - 애니메이션 (나중 추가)
- `ProjectSettings/` - Unity 프로젝트 설정

#### B. 핵심 클래스 구현
- `Singleton<T>` - 싱글톤 기본 클래스
- `GameManager` - 게임 상태 관리 (Title, Tutorial, Playing, Paused, GameOver)
- `UIManager` - UI 패널 표시/숨김 관리
- `PlayerDataManager` - 플레이어 데이터 저장/로드 (JSON + PlayerPrefs)
- `GameConstants` - 게임 전역 상수 (가격, 경험치, 레벨 조건 등)

#### C. 프로젝트 설정
- Unity 6 LTS 프로젝트 설정
- `.gitignore` 생성 (Unity 빌드 파일 제외)
- `package.json` 생성

#### D. 기본 씬
- `Main.unity` - 메인 씬 (Canvas, Camera 포함)

### 파일 목록
```
Assets/Scripts/Core/Singleton.cs
Assets/Scripts/Core/GameManager.cs
Assets/Scripts/Core/GameConstants.cs
Assets/Scripts/Core/MainSceneInitializer.cs
Assets/Scripts/UI/UIManager.cs
Assets/Scripts/UI/PlayerHUD.cs
Assets/Scripts/UI/MainScreenUIBuilder.cs
Assets/Scripts/Data/PlayerDataManager.cs
Assets/Scenes/Main.unity
ProjectSettings/ProjectVersion.txt
Assets/package.json
.gitignore
```

---

## Step 2: 메인 화면 구현 ✅

### 완료 항목

#### A. UI 레이아웃
- 상단 HUD: 레벨, 돈, 경험치 표시
- 중앙: 다방 표시 영역 (placeholder)
- 하단: 네비게이션 바 (손님 대기, 상점, 컬렉션, 설정)

#### B. 스크립트 구현
- `MainScreenUIBuilder` - UI 동적 생성 (Canvas에 자동으로 레이아웃 구성)
- `PlayerHUD` - 플레이어 정보 표시 및 업데이트
- `MainScreenManager` - 메인 화면 버튼 이벤트 처리
- `MainSceneInitializer` - 씬 초기화 및 게임 매니저 연결

#### C. 기능
- 플레이어 레벨, 돈 표시
- 경험치 바 (임시)
- 손님 대기 버튼 (아직 미구현)
- 상점, 컬렉션, 설정 버튼 (아직 미구현)

### 사용 방법
1. `Main.unity` 씬에서 Canvas를 선택
2. Canvas에 `MainScreenUIBuilder` 스크립트 추가
3. 씬 실행 시 자동으로 UI 생성

### 파일 목록
```
Assets/Scripts/Game/MainScreenManager.cs
Assets/Scripts/UI/PlayerHUD.cs
Assets/Scripts/UI/MainScreenUIBuilder.cs
Assets/Scripts/Core/MainSceneInitializer.cs
```

### UI 구조
```
Canvas
├─ Background (크림색 배경)
├─ TopHUD (상단 정보)
│  ├─ LevelText
│  └─ MoneyText
├─ CafeDisplay (중앙 다방 영역)
└─ BottomNavigation (하단 네비게이션)
   ├─ WaitCustomerBtn
   ├─ ShopBtn
   ├─ CollectionBtn
   └─ SettingsBtn
```

---

## 다음 단계
**Step 3: 타이틀 화면 구현**
- Title 씬 생성
- 게임 시작, 이름 설정 팝업
- 게임 시작 → Main 씬 전환

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

**상태**: Step 2 완료, Step 3 준비 중
