# Art Pipeline Report

본 보고서는 `10_ART/conceptArt` 폴더 내 비캐릭터 시트 전체에 대해 자동 크롭 및 분류 작업을 수행한 결과를 검증하고 요약한 문서입니다.

## 📊 1. 최종 추출 통계

Cropped 폴더 내 생성된 **총 PNG 수**: **316 개**

### 카테고리별 개수 (Category Distribution)
- **Furniture**: 82 개
- **Environment**: 58 개
- **Tea**: 53 개
- **Decoration**: 41 개
- **UI**: 40 개
- **Effects**: 7 개
- **Background**: 2 개
- **Uncategorized**: 33 개

> [!TIP]
> Uncategorized 폴더에 남아있는 33개의 에셋은 이전에 단일 테스트용으로 추출되었던 임시 에셋(`Asset_7fc5059d...`)이거나 기본 이름 부여 룰에 포함된 잉여 에셋입니다.

## 🛡️ 2. 무결성 검증 (Integrity Check)

전체 PNG 파일에 대해 0-byte, 손상 여부, 헤더 무결성 등을 검증했습니다.
- **0byte 파일**: 0 개 (모두 정상 용량)
- **손상된 파일(열리지 않음)**: 0 개 (모두 정상 PNG 포맷)

## ⚠️ 3. 중복 데이터 검사 (Duplication Check)

### 중복 이름 (Duplicate Names)
- **발견된 중복 파일명**: 0 건
- 모든 에셋이 고유한 파일명과 카테고리 구조를 정상적으로 유지하고 있습니다.

### 중복 내용 (Duplicate MD5 Hashes)
이름(경로)은 다르지만 이미지의 실제 픽셀 데이터(MD5)가 100% 일치하는 에셋들이 다수 발견되었습니다.
주로 이전 테스트에서 `Uncategorized/Asset_...` 로 생성된 항목과 새롭게 정식 이름으로 생성된 항목 간의 중복입니다.

- **발견된 주요 중복 그룹**: 30 건
  - 예시 1: `Decoration_Basket_Woven_01.png` == `Asset_..._01.png`
  - 예시 2: `Furniture_Table_Long_01.png` == `Asset_..._07.png`
  - 예시 3: `Tea_Brazier_Clay_01.png` == `Asset_..._22.png`

> [!WARNING]
> 이전 단일 테스트 시 `Uncategorized` 폴더에 생성된 `Asset_7fc5059d...` 형태의 파일들이 새로 올바른 카테고리에 분류된 파일들과 픽셀(MD5) 단위로 완전히 동일합니다. Unity 프로젝트 반영 전, `Uncategorized` 내의 임시 파일들을 안전하게 삭제하는 것을 권장합니다.

## 💡 종합 결론
- 자동 크롭 스크립트를 통한 **Bounding Box 검출 및 분류 시스템이 매우 성공적으로 동작**했습니다.
- 에셋의 손상 없이 고품질의 투명 배경 PNG가 316장 분리 완료되었습니다.
- 임시 테스트 파일만 삭제하면 즉시 Unity 파이프라인에 통합 가능한 상태입니다.
