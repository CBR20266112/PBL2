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

**상태**: Step 3 완료, Step 4 준비 중
