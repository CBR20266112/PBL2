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
Assets/Scripts/UI/UIManager.cs
Assets/Scripts/Data/PlayerDataManager.cs
Assets/Scenes/Main.unity
ProjectSettings/ProjectVersion.txt
Assets/package.json
.gitignore
```

### 아키텍처 개요

```
GameManager (싱글톤)
├─ 게임 상태 관리 (Loading, Title, Tutorial, Playing, Paused, GameOver)
├─ 이벤트 시스템 (OnGameStateChanged)
└─ 씬 로드 제어

UIManager (싱글톤)
├─ Canvas 참조
├─ 패널 활성화/비활성화
└─ UI 중앙 관리

PlayerDataManager (싱글톤)
├─ PlayerData (이름, 레벨, 돈, 경험치)
├─ JSON 저장/로드
├─ PlayerPrefs 사용
└─ 레벨업 자동 계산

GameConstants
└─ 모든 게임 상수 정의
```

### 다음 단계
**Step 2: 메인 화면 구현**
- Title 씬 생성
- 플레이 버튼, 설정 버튼 UI
- 게임 시작 로직

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
2. Placeholder 이미지 사용 (디자인은 나중)
3. 구조를 우선 (완성도보다 기능 먼저)
4. 한 번에 한 Step만 구현
5. 각 Step 완료 후 다음 Step 진행

---

**상태**: Step 1 완료, Step 2 준비 중
