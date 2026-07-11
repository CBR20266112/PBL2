# AI Asset Import Guide

이 문서는 예전 AI 디자인 워크플로우에 대한 참고 가이드입니다.
현재 게임 전용 디자인 이미지는 `10_ART` 폴더를 사용하며, 최신 워크플로우는 `ART_ASSET_IMPORT_GUIDE.md`를 확인하세요.

## 1. 과거 상태

과거 AI로 생성한 디자인 파일은 다음 폴더에 있었습니다:

`C:\Users\vipgo\Dev\PBL2\13_REFERENCE\AI_Design`

현재 파일은 UUID 형태의 이름을 가진 1024x1024 PNG 이미지입니다.

## 2. 권장 프로젝트 폴더 구조

Unity로 가져오기 전에 아래와 같은 폴더 구조를 사용하는 것이 좋습니다:

- `Assets/Resources/AI_Design/Tilesets`
- `Assets/Resources/AI_Design/Furniture`
- `Assets/Resources/AI_Design/Drinks`
- `Assets/Resources/AI_Design/Ingredients`
- `Assets/Resources/AI_Design/Icons`

이렇게 분류하면 런타임에서 `Resources.Load<Sprite>("AI_Design/..." )`로 쉽게 불러올 수 있습니다.

## 3. 자동 임포트 도구

`Assets/Editor/AIAssetImporter.cs` 에디터 스크립트를 추가했습니다.

### 사용 방법
1. Unity Editor를 실행합니다.
2. 상단 메뉴에서 `AI Design > Import AI Design Assets`를 선택합니다.
3. `13_REFERENCE/AI_Design` 폴더의 PNG 파일을 `Assets/Resources/AI_Design/`로 복사합니다.
4. 복사된 텍스처는 자동으로 `Sprite (2D and UI)`로 설정되고 `Multiple` 모드로 임포트됩니다.
5. 이미지 파일명이 UUID일 경우, 이미 정의된 기본 매핑에 따라 더 안정적인 이름으로 바뀝니다.
6. 임포트가 완료되면 `Assets/Resources/AI_Design/ai_design_manifest.json` 파일이 생성됩니다.

### 주의사항
- 현재 파일 이름이 UUID이므로, 나중에 파일 내용을 확인한 뒤 적절한 이름으로 바꿔주는 것이 필요합니다.
- `AIAssetImporter`는 미리 정의된 매핑을 사용하여 현재 알고 있는 파일에만 안정적인 이름을 할당하며, 나머지는 `AI_Design_Raw_Sheet_*.png` 같은 규칙적 이름으로 저장합니다.
- 한 PNG 파일 내에 여러 자산이 있는 경우 `Sprite Editor`에서 수동으로 `Slice`해야 합니다.

## 4. 네이밍 권장 방식

각 자산은 아래와 같은 규칙으로 이름을 정하면 좋습니다.

### 건축 타일셋
- `Tileset_Floor_Wood.png`
- `Tileset_Wall_Paper.png`

### 가구
- `Furniture_Table_Tea.png`
- `Furniture_Chair_Wood.png`

### 음료
- `Drink_Matcha.png`
- `Drink_JasmineTea.png`

### 재료
- `Ingredient_Honey.png`
- `Ingredient_Omija.png`

## 5. 다음 작업 제안

1. 현재 AI 디자인 이미지 각각의 내용을 확인하여 어떤 자산인지 분류
2. `Assets/Resources/AI_Design/`로 복사 후 적절히 이름 변경
3. `Sprite Editor`를 사용하여 한 이미지에 여러 자산이 담긴 경우 `Slice` 수행
4. UI 및 게임 로직에서 새로운 스프라이트 경로를 연결

## 6. 추가 지원

필요하면 다음 작업을 이어서 진행할 수 있습니다:

- AI 디자인 이미지의 컨셉별 분류 및 네이밍
- sprite slice 정보를 자동으로 생성하는 에디터 유틸리티
- 디자인 파일 기반 `ScriptableObject` 자산 목록 생성
- 현재 코드에 `Resources.Load`를 사용하는 이미지 로딩 루틴 추가
