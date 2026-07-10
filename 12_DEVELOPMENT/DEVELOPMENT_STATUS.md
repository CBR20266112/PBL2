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

**상태**: Step 4 완료, Step 5 준비 중
