# Asset Analysis — Part 03

> **작업 범위**: `10_ART/conceptArt` 폴더 내 미분석 PNG
> **분석 일자**: 2026-07-12
> **분석 대상**: 이전 Part01/02에서 분석되지 않은 PNG (비캐릭터 기준)

---

## 전체 PNG 처리 현황 (Part01 ~ Part03 누계)

| # | 파일명 (앞 8자) | Part | 처리 결과 |
|---|----------------|------|-----------|
| 1 | `25459862...` | Part01 | ✅ 분석 완료 |
| 2 | `6ec69027...` | Part01 | ⏭️ Character (Skipped) |
| 3 | `714a876a...` | Part01 | ⏭️ Character (Skipped) |
| 4 | `7273a87f...` | Part01 | ✅ 분석 완료 |
| 5 | `7fc5059d...` | Part01 | ✅ 분석 완료 |
| 6 | `adf7adab...` | Part02 | ✅ 분석 완료 |
| 7 | `b61d0da6...` | Part02 | ✅ 분석 완료 |
| 8 | `c06ec44a...` | Part02 | ✅ 분석 완료 |
| 9 | `c2bd230b...` | Part02 | ⏭️ Character (Skipped) |
| 10 | `d1f644d5...` | Part02 | ⏭️ Character (Skipped) |
| 11 | `ee20bee0...` | Part02 | ⏭️ Character (Skipped) |
| 12 | `f6ceccc7...` | Part02 | ✅ 분석 완료 |
| 13 | `f72e7bd5...` | Part02 | ✅ 분석 완료 |
| **14** | **`f837fa6a...`** | **Part03** | **✅ 이번 분석** |
| **15** | **`fed646b9...`** | **Part03** | **✅ 이번 분석** |

> **비고**: 전체 15개 파일 중 비캐릭터 10개, 캐릭터 5개.
> Part03 시점에서 미분석 비캐릭터는 2개입니다.

---

## PNG #14 — `f837fa6a-a6ba-467a-ac58-e9df21f67c26.png`

### 1. 시트 종류

**차 우려내기(Brewing) 애니메이션 시퀀스 시트**

8개 행(Row) × 최대 8개 프레임(Column)으로 구성된 다단계 애니메이션 스프라이트 시트.
각 행은 독립된 브루잉 단계 또는 오브젝트의 상태 변화를 표현함.
투명 배경(체커보드 패턴)이 적용된 전문적 스프라이트 시트.

### 2. 포함된 Asset 종류

| 행 번호 | 오브젝트 종류 | 설명 |
|---------|-------------|------|
| Row 1 | TeaPot + TeaTray 세트 | 주전자로 차를 따르는 8단계 애니메이션 (물 붓기 시퀀스) |
| Row 2 | Gaiwan + TeaTray 세트 | 개완(蓋碗) + 찻잔 받침대 8단계 (뚜껑 열기/닫기 상태 변화) |
| Row 3 | Steam Effect | 수증기(연기) 이펙트 8단계 (투명도/형태 변화, 반투명 파티클) |
| Row 4 | TeaCup (Side View) | 차 담기는 상태 변화 8단계 (빈 컵 → 가득 찬 컵) |
| Row 5 | Honey Jar + Dipper | 꿀 병 + 국자 세트 5단계 (꿀 담기 인터랙션) |
| Row 6 | Creamer Pitcher + Cup | 크리머 주전자로 우유/크림 붓기 5단계 |
| Row 7 | TeaCup (Top View) + Spoon | 차 컵 위에서 스푼으로 젓는 8단계 (탑뷰 시퀀스) |
| Row 8 | TeaCup + Saucer (Isometric) | 찻잔 + 받침 조합 6단계 (꽃 장식 등장, 완성 시퀀스) |

### 3. 예상 Asset 개수

| 항목 | 개수 |
|------|------|
| 애니메이션 프레임 총계 | **57개** (8+8+8+8+5+5+8+6+1 스프라이트) |
| 독립 오브젝트 종류 | **8종** (Row별 1종) |
| Unity Sprite 기준 분리 시 | **~57개 개별 Sprite** |
| 애니메이션 클립 수 | **~8개** (각 Row = 1 AnimationClip) |

### 4. Unity 활용도

| 용도 | 활용 방법 | 우선도 |
|------|----------|--------|
| **2D Animation** | Sprite Animation / Animator Controller | ⭐⭐⭐⭐⭐ |
| **게임플레이 연출** | 차 우려내기 미니게임 핵심 시각 요소 | ⭐⭐⭐⭐⭐ |
| **이펙트 시스템** | Row3(Steam)은 Particle System 또는 UI 애니메이션 | ⭐⭐⭐⭐ |
| **UI 인터랙션** | 재료 추가(꿀, 크림) 시 피드백 애니메이션 | ⭐⭐⭐⭐ |
| **완성 연출** | Row8 찻잔 완성 씬 | ⭐⭐⭐ |

### 5. 자동 크롭 가능 여부

**⚠️ 조건부 자동 가능**

- 배경이 투명(알파 채널 존재)하여 기술적으로 Auto-crop 가능
- 각 행이 균일한 간격으로 정렬되어 있음 (그리드 구조)
- Row별 열(Column) 수가 상이함(5개 ~ 8개) → 행별 개별 처리 필요
- Row 3(Steam)은 반투명 픽셀이 많아 threshold 조정 필요

### 6. 수동 작업이 필요한 이유

1. **행별 프레임 수 불균일**: Row5(5개), Row6(5개), Row8(6개)로 열 개수가 다름 → 단순 균등 분할 불가
2. **Row3 수증기 이펙트**: 반투명 픽셀 경계가 모호 → 자동 경계 탐지 오류 가능성
3. **프레임 번호 레이블**: 각 프레임 위에 숫자(1~8)가 텍스트로 인쇄됨 → 필요시 제거 작업
4. **행 번호 아이콘**: 각 행 왼쪽에 원형 번호(①~⑧)가 존재 → 크롭 경계 설정 시 제외 처리 필요
5. **세트 오브젝트**: Row1, Row2, Row6는 두 오브젝트가 세트로 배치 → 개별 분리 시 별도 마스킹

### 7. 추천 Asset 이름 규칙

```
Animation_Brewing_TeaPot_Pour_Row01_Frame[01~08]
Animation_Brewing_Gaiwan_Open_Row02_Frame[01~08]
Animation_Brewing_Steam_Row03_Frame[01~08]
Animation_Brewing_TeaCup_Fill_Row04_Frame[01~08]
Animation_Brewing_Honey_Add_Row05_Frame[01~05]
Animation_Brewing_Cream_Pour_Row06_Frame[01~05]
Animation_Brewing_TeaCup_Stir_Row07_Frame[01~08]
Animation_Brewing_TeaCup_Finish_Row08_Frame[01~06]
```

**세트 오브젝트 분리 시:**
```
TeaWare_TeaPot_Black_Side_01
TeaWare_TeaTray_Brown_Wood_01
TeaWare_Gaiwan_White_Floral_01
TeaWare_Creamer_White_01
Ingredient_Honey_Jar_Gold_01
Ingredient_Honey_Dipper_01
Effect_Steam_White_Frame[01~08]
```

---

## PNG #15 — `fed646b9-5d72-4cb2-a6ce-09d2f1789e6d.png`

### 1. 시트 종류

**건축 구조 요소 및 인테리어 부품 시트**

카페 실내 공간 구성에 필요한 바닥재, 벽 패널, 창문, 문, 선반 등 건축/인테리어 요소를 모아놓은 모듈식 환경 Asset 시트.
투명 배경(체커보드 패턴)이 적용되어 있으며, 한국/일본/중국 동아시아 스타일이 혼합됨.

### 2. 포함된 Asset 종류

| 행 / 그룹 | 오브젝트 종류 | 수량 | 문화권 |
|-----------|-------------|------|--------|
| **Row 1 — 바닥재** | 바닥 타일 패널 (원근 뷰) | 7개 | 혼합 (나무/다다미/석재/대나무/장식) |
| **Row 2 — 벽 패널** | 슬라이딩 월 패널 / 벽 분리대 | 6개 | 한국/일본 (호지, 격자, 파도, 나무) |
| **Row 3 — 기둥/보** | 기둥, 수평보, 꺾인 보 조각 | 4개 | 한국 목조건축 |
| **Row 4 — 창문** | 창문 프레임 (격자/원형/장호) | 4개 | 한국/중국 |
| **Row 5 — 문** | 단문, 슬라이딩 이중문, 정문(입구) | 3개 | 한국 (한옥 스타일) |
| **Row 6 — 선반/벤치** | 목재 선반(simple), 황금 장식 선반 | 2개 | 한국/중국 |
| **Row 7 — 받침/기단** | 바닥 몰딩/기단 부재 | 4개 | 한국 |

### 3. 예상 Asset 개수

| 항목 | 개수 |
|------|------|
| 개별 오브젝트 총계 | **약 30개** |
| 바닥재 타일 | 7개 |
| 벽 패널 | 6개 |
| 기둥/보 부재 | 4개 |
| 창문 | 4개 |
| 문 | 3개 |
| 선반 | 2개 |
| 기단/몰딩 | 4개 |

### 4. Unity 활용도

| 용도 | 활용 방법 | 우선도 |
|------|----------|--------|
| **카페 배경 구성** | 씬(Scene)의 레이어드 배경 스프라이트 | ⭐⭐⭐⭐⭐ |
| **모듈식 맵 빌딩** | 타일 조합으로 다양한 카페 룸 구성 | ⭐⭐⭐⭐⭐ |
| **인터랙션 오브젝트** | 문 개폐 애니메이션 (SpriteAnimation) | ⭐⭐⭐⭐ |
| **UI 프레임** | 창문/패널을 HUD 패널 프레임으로 활용 | ⭐⭐⭐ |
| **커스터마이제이션** | 플레이어 카페 꾸미기 시스템의 빌딩 부품 | ⭐⭐⭐⭐ |

### 5. 자동 크롭 가능 여부

**✅ 자동 크롭 대부분 가능**

- 투명 배경으로 Alpha-기반 Bounding Box 탐지 용이
- 각 오브젝트가 독립적으로 배치되어 겹침 없음
- 가로/세로 여백이 충분하여 경계 탐지 안정적
- `cv2.findContours` 또는 `PIL.getbbox()` 방식으로 자동화 가능

### 6. 수동 작업이 필요한 이유

1. **비균등 크기**: 문(대형), 바닥타일(중형), 기둥(소형) 등 오브젝트 크기가 매우 다양함
2. **Row3 구조물 밀집**: 기둥과 보(수평재)가 서로 근접 배치 → 개별 경계 탐지 시 하나로 묶일 위험
3. **복합 구조물**: Row5의 정문(입구)은 좌우 패널이 연결된 일체형 → 분리 시 수동 확인 필요
4. **Row1 타일 원근감**: 사다리꼴(원근 뷰) 형태의 바닥 타일 → Unity Sprite로 사용 시 pivot 조정 필요
5. **문화권별 분류 태깅**: 한국/일본/중국 스타일 구분 메타데이터를 수동으로 태깅해야 함

### 7. 추천 Asset 이름 규칙

**바닥재**
```
Floor_Wood_Light_Korea_01
Floor_Wood_Dark_Korea_01
Floor_Wood_Glossy_01
Floor_Tatami_Japan_01
Floor_Stone_Gray_01
Floor_Bamboo_Natural_01
Floor_Ornate_China_01
```

**벽 패널**
```
Wall_Panel_Plain_Korea_01
Wall_Panel_Grid_Korea_01
Wall_Panel_Hanji_Korea_01
Wall_Panel_Wave_Japan_01
Wall_Panel_Cream_01
Wall_Panel_Wood_Dark_01
```

**기둥 / 보**
```
Structure_Pillar_Wood_Korea_01
Structure_Beam_Horizontal_01
Structure_Beam_Corner_01
Structure_Beam_Decorated_01
```

**창문**
```
Window_Grid_Small_Korea_01
Window_Grid_Triple_Korea_01
Window_Shoji_Slide_Japan_01
Window_Round_China_01
```

**문**
```
Door_Hanok_Single_Korea_01
Door_Hanok_Double_Korea_01
Door_Entrance_Ornate_China_01
```

**선반 / 기단**
```
Shelf_Wood_Simple_Korea_01
Shelf_Gold_Ornate_China_01
Base_Floor_Molding_Korea_01
Base_Floor_Molding_Korea_02
Base_Floor_Molding_Korea_03
Base_Floor_Molding_China_01
```

---

## Part03 분석 요약

| 항목 | 내용 |
|------|------|
| 이번 분석 파일 수 | **2개** (비캐릭터) |
| 건너뜀 파일 수 | **0개** (이번 회차 Character 없음) |
| 추출 예상 총 Asset | **~87개** (f837fa6a: ~57개 + fed646b9: ~30개) |
| 자동 크롭 가능 | 조건부 (2개 모두 주의사항 있음) |

