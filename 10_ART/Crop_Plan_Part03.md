# Crop Plan — Part 03

> **작업 범위**: `10_ART/conceptArt` 내 미분석 PNG 2개
> **작성 일자**: 2026-07-12
> **도구**: Python (Pillow / OpenCV)

---

## PNG #14 — `f837fa6a-a6ba-467a-ac58-e9df21f67c26.png`
### 브루잉 애니메이션 시퀀스 시트

#### 기본 정보

| 항목 | 내용 |
|------|------|
| 파일명 | `f837fa6a-a6ba-467a-ac58-e9df21f67c26.png` |
| 시트 구조 | 8행(Row) × 최대 8열(Col), 불균일 |
| 배경 | 투명 (알파 채널 존재) |
| 크롭 난이도 | ⚠️ 중간 (행별 열 수 불일치) |

#### 크롭 전략

**방법: 행(Row) 단위 수동 분할 + 프레임 단위 자동 크롭**

```
전략:
1단계 - 시트를 8개 Row로 수동 분할 (Y 좌표 기준)
2단계 - 각 Row 내부에서 Alpha Bounding Box로 개별 프레임 자동 감지
3단계 - Row3(Steam)은 threshold를 낮춰(alpha > 10) 반투명 픽셀까지 포함
4단계 - 행 번호 아이콘(①~⑧)은 크롭 영역에서 제외 (왼쪽 여백 처리)
5단계 - 프레임 번호 레이블(1~8 텍스트)은 원본에서 제거 X, 메타데이터로만 기록
```

#### Row별 크롭 계획

| Row | 오브젝트 | 프레임 수 | 크롭 방식 | 주의사항 |
|-----|---------|----------|----------|---------|
| Row 1 | TeaPot + TeaTray | 8 | Alpha BBox | 물 붓기 시 투명 픽셀 포함 주의 |
| Row 2 | Gaiwan + TeaTray | 8 | Alpha BBox | 뚜껑 개폐로 높이 변동 |
| Row 3 | Steam Effect | 8 | Alpha BBox (threshold=10) | 반투명 픽셀 다수, 낮은 임계값 필요 |
| Row 4 | TeaCup (Side) | 8 | Alpha BBox | 비교적 단순, 자동 가능 |
| Row 5 | Honey Jar + Dipper | 5 | Alpha BBox | 열 5개, 나머지 공간 빈 칸 |
| Row 6 | Creamer + Cup | 5 | Alpha BBox | 세트 오브젝트, 필요시 2개 분리 |
| Row 7 | TeaCup (Top) + Spoon | 8 | Alpha BBox | 탑뷰, 경계 단순 |
| Row 8 | TeaCup + Saucer | 6 | Alpha BBox | 꽃 장식 주의 |

#### 출력 디렉토리 구조 (제안)

```
Assets/
└── Sprites/
    └── Animation/
        └── Brewing/
            ├── Row01_TeaPot_Pour/
            │   ├── Animation_Brewing_TeaPot_Pour_Row01_Frame01.png
            │   └── ... Frame08.png
            ├── Row02_Gaiwan_Open/
            ├── Row03_Steam/
            ├── Row04_TeaCup_Fill/
            ├── Row05_Honey_Add/
            ├── Row06_Cream_Pour/
            ├── Row07_TeaCup_Stir/
            └── Row08_TeaCup_Finish/
```

#### 자동화 코드 접근법 (Python 개요)

```python
# Row 경계를 수동으로 지정 후, 각 Row 내부는 Alpha BBox 자동화
ROW_Y_RANGES = {
    "row01": (y1_01, y2_01),
    "row02": (y1_02, y2_02),
    # ... 총 8개 Row
}
ROW_FRAME_COUNT = {
    "row01": 8, "row02": 8, "row03": 8, "row04": 8,
    "row05": 5, "row06": 5, "row07": 8, "row08": 6
}
STEAM_ROW_ALPHA_THRESHOLD = 10   # Row03 전용
DEFAULT_ALPHA_THRESHOLD = 30
```

#### 우선순위

| 순위 | Row | 이유 |
|------|-----|------|
| 1순위 | Row1 (TeaPot) | 게임 핵심 브루잉 연출 |
| 2순위 | Row4 (TeaCup Fill) | 차 완성도 피드백 |
| 3순위 | Row3 (Steam) | 분위기 이펙트 |
| 4순위 | Row8 (Finish) | 완성 연출 |
| 5순위 | Row2, 5, 6, 7 | 추가 인터랙션 |

---

## PNG #15 — `fed646b9-5d72-4cb2-a6ce-09d2f1789e6d.png`
### 건축 구조/인테리어 부품 시트

#### 기본 정보

| 항목 | 내용 |
|------|------|
| 파일명 | `fed646b9-5d72-4cb2-a6ce-09d2f1789e6d.png` |
| 시트 구조 | 7개 그룹 (Row 구분), 불균일 배치 |
| 배경 | 투명 (알파 채널 존재) |
| 크롭 난이도 | ✅ 낮음 (오브젝트 간격 충분) |

#### 크롭 전략

**방법: Alpha Bounding Box 전체 자동 크롭**

```
전략:
1단계 - 전체 시트에서 connected component 분석으로 개별 오브젝트 감지
2단계 - 각 오브젝트별 Alpha BBox 추출 (padding 8~16px)
3단계 - 크기별 자동 분류 (large/medium/small)
4단계 - Row3(기둥/보)는 근접 오브젝트 병합 방지를 위해 수동 확인
5단계 - Row5(입구 정문)는 일체형이므로 하나의 오브젝트로 처리
```

#### 그룹별 크롭 계획

| 그룹 | 오브젝트 종류 | 수량 | 크롭 방식 | 주의사항 |
|------|-------------|------|----------|---------|
| Row 1 | 바닥재 타일 | 7 | 자동 BBox | 사다리꼴 형태, Unity Pivot 후보정 필요 |
| Row 2 | 벽 패널 | 6 | 자동 BBox | 단순 사각형, 쉬움 |
| Row 3 | 기둥/보 | 4 | 반자동 | 근접 배치, 수동 Y 구분 필요 |
| Row 4 | 창문 | 4 | 자동 BBox | 원형 창문 경계 주의 |
| Row 5 | 문 | 3 | 반자동 | 정문(입구)은 분리 여부 결정 필요 |
| Row 6 | 선반 | 2 | 자동 BBox | 가로 길이 차이 큼 |
| Row 7 | 기단/몰딩 | 4 | 자동 BBox | 납작한 형태, threshold 주의 |

#### 출력 디렉토리 구조 (제안)

```
Assets/
└── Sprites/
    └── Environment/
        └── Architectural/
            ├── Floor/
            │   ├── Floor_Wood_Light_Korea_01.png
            │   └── ... (7개)
            ├── Wall/
            │   └── ... (6개)
            ├── Structure/
            │   └── ... (4개)
            ├── Window/
            │   └── ... (4개)
            ├── Door/
            │   └── ... (3개)
            ├── Shelf/
            │   └── ... (2개)
            └── Base/
                └── ... (4개)
```

#### 자동화 코드 접근법 (Python 개요)

```python
from PIL import Image
import numpy as np
from scipy import ndimage

# Alpha BBox 자동 크롭
img = Image.open("fed646b9-5d72-4cb2-a6ce-09d2f1789e6d.png").convert("RGBA")
alpha = np.array(img)[:, :, 3]

# Connected Component 분석
labeled, num_features = ndimage.label(alpha > 30)
for i in range(1, num_features + 1):
    component = np.where(labeled == i)
    y_min, y_max = component[0].min(), component[0].max()
    x_min, x_max = component[1].min(), component[1].max()
    # 패딩 추가 후 크롭
    PADDING = 10
    crop = img.crop((x_min-PADDING, y_min-PADDING,
                     x_max+PADDING, y_max+PADDING))
    crop.save(f"asset_{i:03d}.png")
```

#### 수동 확인 필요 항목

| 항목 | 이유 | 처리 방법 |
|------|------|---------|
| 기둥 + 보 (Row3) | 근접 배치로 병합될 위험 | Y 좌표 수동 분리 |
| 입구 정문 (Row5) | 일체형 vs 분리 결정 | 기획자 확인 후 처리 |
| 바닥타일 Pivot | 원근 사다리꼴 형태 | Unity에서 Custom Pivot 설정 |

#### 우선순위

| 순위 | 그룹 | 이유 |
|------|-----|------|
| 1순위 | 바닥재 (Row1) | 씬 구성 필수 |
| 2순위 | 벽 패널 (Row2) | 공간 구분 필수 |
| 3순위 | 문 (Row5) | 인터랙션 오브젝트 |
| 4순위 | 창문 (Row4) | 분위기 연출 |
| 5순위 | 선반, 기단, 기둥 | 세부 장식 |

---

## Part03 크롭 계획 요약

| PNG | 크롭 방식 | 예상 산출물 | 자동화율 |
|-----|---------|-----------|---------|
| `f837fa6a...` | 행 수동 분할 + 프레임 자동 BBox | ~57개 PNG | **약 70%** |
| `fed646b9...` | Connected Component 전체 자동 | ~30개 PNG | **약 85%** |

> **도구 권장**: Python 3.x + Pillow + NumPy + SciPy (`ndimage.label`)
> **예상 총 소요 시간**: 자동화 스크립트 작성 2~3시간 + 수동 검수 1시간

