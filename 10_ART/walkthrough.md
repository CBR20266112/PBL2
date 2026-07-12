# 자동 크롭 환경 설정 완료 (Walkthrough)

지시하신 조건에 맞추어 `MASTER_CROP_PLAN.md`와 `MASTER_ASSET_NAMING.md`를 기반으로 한 **Python 크롭 스크립트 작성 및 디렉토리 구조 생성**을 모두 완료했습니다. 실제 이미지는 아직 수정하거나 자르지 않았습니다.

---

## 1. 생성된 폴더 구조

작업이 진행될 `10_ART` 내부에 다음 구조를 구성했습니다:

```text
10_ART/
├── conceptArt/               # (기존) 절대 수정하지 않는 원본 폴더
├── Cropped/                  # (신규) 크롭된 에셋이 저장될 최상위 폴더
│   └── logs/                 # (신규) 작업 로그 및 스킵 내역 저장
└── scripts/
    └── crop_sprites.py       # (신규) 자동 크롭 Python 스크립트
```

## 2. Python 스크립트 (`crop_sprites.py`) 핵심 기능 요약

- **안전성 (Read-Only)**: 원본 `conceptArt` 안의 이미지는 불러오기(`Image.open`)만 수행하여 훼손 위험을 원천 차단했습니다.
- **알파 채널 감지 (OpenCV)**: 투명 배경(Alpha 채널)을 분석하여 객체들의 Bounding Box를 정확히 감지해냅니다.
- **노이즈 필터 및 패딩**: 아주 작은 점(15x15 픽셀 이하 기본값)은 무시(`skipped.txt` 기록)하고, 추출 시 여백(padding=2)을 주어 잘림 현상을 방지합니다.
- **중복 방지 (MD5 Hash)**: 이미 추출된 에셋과 픽셀 값이 100% 동일한 중복 객체를 발견하면 건너뜁니다.
- **파일명 매핑 지원**: 추후 `naming_map.json`을 통해 MASTER_ASSET_NAMING의 정확한 이름을 주입할 수 있도록 설계했습니다 (기본값은 `Asset_[원본이름]_[순번].png` 적용).
- **작업 기록**: `Cropped/logs/` 내부에 `log.txt`와 `skipped.txt`를 자동으로 남겨 관리 효율을 높였습니다.

## 3. 다음 단계 (직접 실행 시 가이드)

현재 터미널 환경에 `opencv-python` 등의 라이브러리가 설치되어 있지 않으므로, 다음 명령어를 통해 의존성을 설치하신 후 스크립트를 실행하실 수 있습니다.

> [!TIP]
> **스크립트 실행 방법 (Windows PowerShell 기준)**
> 
> ```powershell
> # 1. 필수 라이브러리 설치
> pip install pillow numpy opencv-python
> 
> # 2. 10_ART 폴더로 이동
> cd C:\Users\vipgo\Dev\PBL2\10_ART
> 
> # 3. 저장 없이 안전하게 테스트만 해보기 (Dry Run)
> python scripts/crop_sprites.py --input conceptArt --output Cropped --dry-run
> 
> # 4. 실제 크롭 실행
> python scripts/crop_sprites.py --input conceptArt --output Cropped
> ```
