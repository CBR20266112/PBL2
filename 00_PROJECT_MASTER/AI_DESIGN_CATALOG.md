# AI Design Asset Catalog

이 문서는 `13_REFERENCE/AI_Design`에서 가져온 AI 디자인 이미지를 Unity 프로젝트 `Assets/Resources/AI_Design/`로 정리하고 관리하기 위한 카탈로그입니다.

## 현재 정리된 파일 목록

| 파일 이름 | 현재 경로 | 예상 분류 | 비고 |
|---|---|---|---|
| UI_Reference_GameScreen.png | Assets/Resources/AI_Design/ | UI 레퍼런스 | 전체 게임 화면 또는 UI 컨셉 아트 |
| AI_Design_Raw_Sheet_01.png | Assets/Resources/AI_Design/ | 원본 자산 시트 | 내용 확인 후 정확한 분류 필요 |
| Furniture_Sheet_KoreanTeaHouse.png | Assets/Resources/AI_Design/ | 가구 시트 | 티하우스 가구와 소품 모음 |
| AI_Design_Raw_Sheet_02.png | Assets/Resources/AI_Design/ | 원본 자산 시트 | 내용 확인 후 정확한 분류 필요 |
| AI_Design_Raw_Sheet_03.png | Assets/Resources/AI_Design/ | 원본 자산 시트 | 내용 확인 후 정확한 분류 필요 |
| AI_Design_Raw_Sheet_04.png | Assets/Resources/AI_Design/ | 원본 자산 시트 | 내용 확인 후 정확한 분류 필요 |
| AI_Design_Raw_Sheet_05.png | Assets/Resources/AI_Design/ | 원본 자산 시트 | 내용 확인 후 정확한 분류 필요 |
| AI_Design_Raw_Sheet_06.png | Assets/Resources/AI_Design/ | 원본 자산 시트 | 내용 확인 후 정확한 분류 필요 |
| AI_Design_Raw_Sheet_07.png | Assets/Resources/AI_Design/ | 원본 자산 시트 | 내용 확인 후 정확한 분류 필요 |
| AI_Design_Raw_Sheet_08.png | Assets/Resources/AI_Design/ | 원본 자산 시트 | 내용 확인 후 정확한 분류 필요 |
| AI_Design_Raw_Sheet_09.png | Assets/Resources/AI_Design/ | 원본 자산 시트 | 내용 확인 후 정확한 분류 필요 |
| AI_Design_Raw_Sheet_10.png | Assets/Resources/AI_Design/ | 원본 자산 시트 | 내용 확인 후 정확한 분류 필요 |
| AI_Design_Raw_Sheet_11.png | Assets/Resources/AI_Design/ | 원본 자산 시트 | 내용 확인 후 정확한 분류 필요 |
| AI_Design_Raw_Sheet_12.png | Assets/Resources/AI_Design/ | 원본 자산 시트 | 내용 확인 후 정확한 분류 필요 |
| AI_Design_Transparent_Sheet.png | Assets/Resources/AI_Design/ | 투명 배경 시트 | 알파 채널이 있는 아이콘/스크린샷 가능성 |

## 관리 정보

- `Assets/Resources/AI_Design/ai_design_manifest.json` 파일이 생성되어 있습니다.
- 매니페스트는 원본 파일명과 가져온 파일명, 분류, 비고를 저장하여 추후 재분류 및 자동화에 사용합니다.

## 다음 단계

1. 각 파일을 Unity에서 확인하여 정확한 내용과 용도를 확정합니다.
2. 다음과 같은 네이밍 규칙으로 세분화합니다.
   - `Tileset_*`
   - `Furniture_*`
   - `Drink_*`
   - `Ingredient_*`
   - `Icon_*`
   - `UI_Reference_*`
3. 한 이미지 파일에 여러 자산이 포함된 경우, Unity `Sprite Editor`의 `Slice` 기능을 사용하여 개별 스프라이트를 분리합니다.
4. 분류가 확정되면 `ai_design_manifest.json`을 업데이트하여 추적합니다.

## Unity에서 사용할 때

- `Assets/Resources/AI_Design/`에 들어 있는 파일은 런타임 `Resources.Load<Sprite>("AI_Design/<name>")`로 불러올 수 있습니다.
- `SettingsUIBuilder`와 `AudioManager`에 이미 사운드 설정/테마가 추가되어 있으므로, 그래픽 자산이 준비되면 바로 UI에 연결할 수 있습니다.
