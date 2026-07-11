# Art Asset Import Guide

이 가이드는 게임 전용 이미지 자산을 Unity 프로젝트에 안전하게 가져오기 위한 최신 워크플로우를 설명합니다.

## 1. 현재 상태

게임 전용 아트 자산은 이제 다음 폴더에서 관리합니다:

- `C:\Users\vipgo\Dev\PBL2\10_ART`

특히, 컨셉 아트 파일은 다음 경로에 넣습니다:

- `C:\Users\vipgo\Dev\PBL2\10_ART\conceptArt`

이 폴더에는 현재 게임에 사용할 디자인 시트가 들어 있습니다.

## 2. Unity 프로젝트 내 대상 위치

최종적으로 Unity에서 사용할 아트 자산은 아래 위치로 복사합니다:

- `Assets/Resources/Art`

이 위치에 들어간 PNG/JPG 파일은 `Resources.Load`로 게임 런타임에 쉽게 불러올 수 있습니다.

## 3. 자동 임포트 도구

`Assets/Editor/AIAssetImporter.cs` 스크립트는 현재 다음을 수행합니다:

1. `10_ART/conceptArt`에서 `.png` 및 `.jpg` 파일을 찾습니다.
2. `Assets/Resources/Art`로 복사합니다.
3. 복사한 텍스처를 `Sprite` 타입으로, `Multiple` 모드로 자동 설정합니다.
4. `Assets/Resources/Art/art_design_manifest.json`에 소스 파일 이름과 임포트된 안정적 이름을 기록합니다.

### 사용 방법

1. Unity Editor를 실행합니다.
2. 상단 메뉴에서 `Art > Import 10_ART Concept Art`를 선택합니다.
3. `Assets/Resources/Art`에 파일이 복사되고 `art_design_manifest.json`이 생성됩니다.
4. 필요할 경우 `Sprite Editor`에서 각 시트를 수동으로 슬라이스합니다.

## 4. 파일 네이밍

현재는 UUID 파일을 안정적인 순차 이름으로 바꾸어 파일을 관리합니다:

- `Art_Concept_01.png`
- `Art_Concept_02.png`
- ...

향후에는 파일 내용을 확인한 뒤 다음과 같은 명명 규칙을 적용하세요:

- `Tileset_Floor_Wood.png`
- `Furniture_Table_Tea.png`
- `Drink_Matcha.png`
- `Ingredient_Omija.png`

## 5. ART_BIBLE_v1.0 문서 활용

`10_ART/ART_BIBLE_v1.0.docx`는 게임 아트의 싱글 소스 오브 트루스입니다.

이 문서에서 정의한 규칙은 다음과 같습니다:

- 장르: 따뜻한 동양 전통 티 하우스 타이쿤
- 스타일: 부드러운 수채화, 따뜻한 조명, 둥근 실루엣
- UI: Good Coffee 스타일의 가독성을 유지하면서 차 문화 아이덴티티를 반영
- 국가별 대표 색상, 동물, 악기, 주요 차 종류를 정의
- 한국 테마 기본값, 국가별 사운드 테마와 랜덤 모드를 지원

## 6. 다음 작업

1. 현재 `Assets/Resources/Art/Art_Concept_*.png` 파일을 열어 어떤 자산인지 분류합니다.
2. 필요하면 `Assets/Resources/Art` 하위에 `Tilesets`, `Furniture`, `Drinks`, `Ingredients`, `UI` 폴더를 만듭니다.
3. `Sprite Editor`로 각 시트를 슬라이스하고 개별 스프라이트에 이름을 지정합니다.
4. 게임 UI와 로직에서 새 스프라이트 경로를 연결합니다.

## 7. 배포 노트

게임의 실제 실행 링크를 만들려면 Unity WebGL 빌드가 필요합니다.
현재는 `Assets/Editor/WebGLBuilder.cs`를 통해 `docs` 폴더로 WebGL 빌드 출력을 준비할 수 있습니다.

실제 HTTPS 배포 링크는 깃허브 Pages 설정이 활성화된 경우 `https://<username>.github.io/PBL2` 형태로 제공될 수 있습니다.
