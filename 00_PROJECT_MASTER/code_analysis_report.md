# Unity 코드 분석 보고서
## Tea Cafe — Resources.Load 의존성 & Sprite 이관 영향도 분석

> 분석 일자: 2026-07-12  
> 분석 대상: `Assets/Scripts/`, `Assets/Editor/`  
> 상태: 분석 전용 — 코드 수정 없음

---

## 1. Resources.Load() 사용 현황

### 1-A. 실제 `Resources.Load<T>` 호출 목록

| 스크립트 | 메서드 | 경로 문자열 | 타입 |
|---|---|---|---|
| `Scripts/Audio/AudioManager.cs` L115 | `LoadAudioClip()` | `"Audio/Music/" + clipName` | `AudioClip` |
| `Scripts/Audio/AudioManager.cs` L115 | `LoadAudioClip()` | `"Audio/SFX/" + themedClipName` | `AudioClip` |
| `Scripts/Game/CustomerManager.cs` L23 | `LoadAllCustomers()` | `"ScriptableObjects/Customers"` | `CustomerData` (ScriptableObject) |

### 1-B. `Resources.GetBuiltinResource<T>` 호출 목록
> 이것은 **Unity 엔진 내장 리소스**에 접근하는 것으로, `Resources` 폴더와 무관하며 문제 없음.

| 스크립트 | 내용 | 비고 |
|---|---|---|
| `Scripts/UI/FontHelper.cs` L11 | `GetBuiltinResource<Font>("LegacyRuntime.ttf")` | ✅ 엔진 내장 폰트, 무관 |
| `Scripts/UI/FontHelper.cs` L23 | `GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd")` | ✅ 엔진 내장 UI 스프라이트, 무관 |

---

## 2. Sprite / Texture2D 문자열 경로 로드 현황

### 결론: **현재 게임 런타임 코드에서 Sprite 또는 Texture2D를 문자열 경로로 동적 로드하는 코드는 존재하지 않음.**

- `Sprite` 타입은 `FontHelper.cs`에서만 사용되지만, 이는 Unity 내장 UI 스프라이트를 가져오는 용도임.
- `Texture2D`는 전체 코드베이스에서 **한 건도 사용되지 않음**.
- 현재 손님 캐릭터 표시(`CustomerAppearanceUIBuilder.cs`)는 이미지 없이 **단순 색상(Color)만** 사용 중. (미완성 상태)

---

## 3. Assets/Resources/Art 실제 참조 현황

### 결론: **런타임 코드에서 `Resources/Art`를 직접 참조하는 스크립트는 0개.**

| 항목 | 내용 |
|---|---|
| `Assets/Resources/Art/` 내 파일 | `Art_Concept_01.png` ~ `Art_Concept_18.png` (18개) + `art_design_manifest.json` |
| 런타임에서 이 경로를 사용하는 스크립트 | **없음** |
| 이 폴더를 참조하는 코드 | `Assets/Editor/AIAssetImporter.cs` (에디터 전용, `#if UNITY_EDITOR` 조건부 컴파일) |

즉, `Resources/Art`에 들어 있는 18개 이미지는 현재 **게임 내 어디에서도 실제로 사용되지 않는 상태**임.  
`AIAssetImporter.cs`는 에디터 도구이므로 런타임 빌드에 포함되지 않음.

---

## 4. ScriptableObject 직접 참조 방식으로 전환 시 수정 대상 목록

### 4-A. 스프라이트 연결을 위해 **수정해야 하는** 스크립트

| 스크립트 | 경로 | 수정 이유 | 영향 범위 |
|---|---|---|---|
| `CustomerData.cs` | `Scripts/Data/` | `Sprite characterSprite` 필드 추가 필요 | 손님 비주얼 전체 |
| `CustomerAppearanceUIBuilder.cs` | `Scripts/UI/` | 색상 대신 `customer.data.characterSprite`를 `Image.sprite`에 적용 | 손님 등장 화면 |
| `CustomerDataHelper.cs` | `Scripts/Data/` | 런타임 생성(Fallback)에 sprite 초기화 추가 필요 | 폴백 손님 데이터 |

### 4-B. 차(Tea) 에셋 연결을 위해 **향후 신규 생성이 필요한** ScriptableObject

현재 차(Tea) 관련 ScriptableObject가 **존재하지 않음**. 추후 아래 항목 신규 생성 필요:

| 신규 클래스 | 역할 | 포함할 Sprite 필드 |
|---|---|---|
| `TeaData.cs` (신규) | 차 레시피, 문화 정보 저장 | `teaIcon: Sprite` |
| `FurnitureData.cs` (신규) | 가구 배치 정보 | `furnitureSprite: Sprite` |
| `DecorationData.cs` (신규) | 장식 아이템 | `decorationSprite: Sprite` |

### 4-C. **수정 불필요** 스크립트

| 스크립트 | 이유 |
|---|---|
| `AudioManager.cs` | 오디오 전용, 이미지와 무관. Resources.Load<AudioClip>은 그대로 유지가 합리적. |
| `PlayerDataManager.cs` | JSON/PlayerPrefs 저장, 이미지와 무관 |
| `GameManager.cs` | 씬 전환 및 상태 관리, 이미지와 무관 |
| `FontHelper.cs` | GetBuiltinResource 사용, 문제 없음 |
| `KitchenUIBuilder.cs` | 현재 텍스트/색상만 사용, Tea 에셋 연결 시 TeaData ScriptableObject와 연동 예정 |

---

## 5. 변경 난이도 및 예상 영향도

### 전체 평가: ★★☆☆☆ (낮음) — 영향 범위가 좁고 명확

| 항목 | 평가 | 근거 |
|---|---|---|
| **전체 변경 난이도** | 🟢 낮음 | 현재 Sprite 사용 코드가 거의 없음. 구조 수정보다 필드 추가 수준 |
| **CustomerData 수정** | 🟢 매우 낮음 | `public Sprite characterSprite;` 한 줄 추가 |
| **CustomerAppearanceUIBuilder 수정** | 🟡 보통 | 색상 → 스프라이트 교체, null 처리 필요 |
| **AudioManager 변경 필요 여부** | ✅ 불필요 | 오디오 Resources.Load는 유지해도 무방 (파일 수 적고 클립은 바뀌지 않음) |
| **CustomerManager 변경** | 🟡 보통 | Resources.LoadAll → SerializedField List로 전환 권장 |
| **신규 ScriptableObject 생성** | 🟡 보통 | 차, 가구, 장식 등 데이터 클래스 신규 설계 필요 |

### 위험 요소
- `CustomerManager.cs`가 `Resources.LoadAll<CustomerData>("ScriptableObjects/Customers")` 방식을 사용 중인데, 현재 `Resources/ScriptableObjects/` 폴더가 **비어 있음**. 즉 매번 Fallback(`CustomerDataHelper`)으로 동작 중. 이 구조 자체가 이미 불안정한 상태.

---

## 6. 기존 SaveData / JSON 저장 방식과의 충돌 여부

### 결론: **충돌 없음. 완전히 독립적.**

| 저장 시스템 | 방식 | Sprite 영향 |
|---|---|---|
| `PlayerDataManager` | `PlayerPrefs` + `JsonUtility` | `PlayerData` 클래스에 `Sprite` 필드 없음 → 충돌 없음 |
| `ManifestEntry` (AIAssetImporter) | JSON 파일 (`art_design_manifest.json`) | 에디터 전용, 런타임 저장과 무관 |

**이유:** `JsonUtility`는 Unity 직렬화 규칙을 따르며, `Sprite`는 직렬화 대상이 아님. ScriptableObject의 `Sprite` 필드는 **에셋 참조(GUID)**로 관리되므로 런타임 JSON 저장과 레이어가 다름. 전혀 충돌하지 않음.

---

## 7. 종합 요약

```
현재 상태 한 줄 요약:
"Sprite를 실제로 사용하는 런타임 코드가 없고,
 Resources/Art는 코드로 참조되지 않는 빈 자산 공간이다."
```

| 항목 | 현황 |
|---|---|
| Resources.Load 호출 수 | 3건 (AudioClip 2, CustomerData 1) |
| 문자열 경로로 Sprite 로드 | 0건 |
| Resources/Art 를 참조하는 런타임 코드 | 0건 |
| 현재 손님 비주얼 | 색상(Color)만 사용 (이미지 없음) |
| SaveData와 충돌 여부 | 없음 |
| 전환 난이도 | 낮음 (핵심 수정 3~4개 스크립트) |

### 권장 순서 (실행 시)

1. `CustomerData.cs` — `Sprite characterSprite` 필드 추가
2. `CustomerManager.cs` — `Resources.LoadAll` → Inspector 기반 List로 전환
3. `CustomerDataHelper.cs` — 폴백 처리 정리 (또는 폐기)
4. `CustomerAppearanceUIBuilder.cs` — 색상 → 실제 스프라이트 렌더링으로 교체
5. `TeaData.cs` 등 신규 ScriptableObject 설계 및 생성
6. `AIAssetImporter.cs` — 대상 경로를 `Resources/Art` → `Sprites/` 로 수정 (에디터 툴)
