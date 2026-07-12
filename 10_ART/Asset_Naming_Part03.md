# Asset Naming — Part 03

> **작업 범위**: `10_ART/conceptArt` 내 미분석 PNG 2개
> **작성 일자**: 2026-07-12

---

## 네이밍 규칙 기본 원칙

```
[Category]_[Name]_[Variant/Culture]_[Index]

- Category   : Asset 대분류 (Animation / TeaWare / Ingredient / Effect / Floor / Wall / Window / Door / Shelf / Structure / Base)
- Name       : 오브젝트 이름 (영문, PascalCase)
- Variant    : 색상, 문화권, 스타일 등 (Korea / Japan / China / Vietnam, 또는 색상명)
- Index      : 01 ~ 99 (동일 종류 복수 시)
```

---

## PNG #14 — `f837fa6a` — 브루잉 애니메이션 시퀀스 시트

### 네이밍 방식

애니메이션 프레임은 **2가지 레벨**로 네이밍:

1. **애니메이션 클립 레벨** (Row 전체를 묶는 Clip 이름)
2. **개별 프레임 레벨** (각 Sprite 파일 이름)

---

### 애니메이션 클립 이름 (8개)

| Row | AnimationClip 이름 | 설명 |
|-----|-------------------|------|
| Row 1 | `Anim_Brewing_TeaPot_Pour` | 주전자 물 붓기 (8프레임) |
| Row 2 | `Anim_Brewing_Gaiwan_Open` | 개완 뚜껑 조작 (8프레임) |
| Row 3 | `Anim_Brewing_Steam_Rise` | 수증기 상승 이펙트 (8프레임) |
| Row 4 | `Anim_Brewing_TeaCup_Fill` | 찻잔 채워지기 (8프레임) |
| Row 5 | `Anim_Brewing_Honey_Add` | 꿀 추가 (5프레임) |
| Row 6 | `Anim_Brewing_Cream_Pour` | 크림 붓기 (5프레임) |
| Row 7 | `Anim_Brewing_TeaCup_Stir` | 찻잔 젓기 탑뷰 (8프레임) |
| Row 8 | `Anim_Brewing_TeaCup_Finish` | 차 완성 연출 (6프레임) |

---

### 개별 Sprite 파일 이름 — 전체 목록

#### Row 1 — TeaPot + TeaTray (8프레임)
```
Animation_Brewing_TeaPot_Pour_Row01_Frame01.png
Animation_Brewing_TeaPot_Pour_Row01_Frame02.png
Animation_Brewing_TeaPot_Pour_Row01_Frame03.png
Animation_Brewing_TeaPot_Pour_Row01_Frame04.png
Animation_Brewing_TeaPot_Pour_Row01_Frame05.png
Animation_Brewing_TeaPot_Pour_Row01_Frame06.png
Animation_Brewing_TeaPot_Pour_Row01_Frame07.png
Animation_Brewing_TeaPot_Pour_Row01_Frame08.png
```

#### Row 2 — Gaiwan + TeaTray (8프레임)
```
Animation_Brewing_Gaiwan_Open_Row02_Frame01.png
Animation_Brewing_Gaiwan_Open_Row02_Frame02.png
Animation_Brewing_Gaiwan_Open_Row02_Frame03.png
Animation_Brewing_Gaiwan_Open_Row02_Frame04.png
Animation_Brewing_Gaiwan_Open_Row02_Frame05.png
Animation_Brewing_Gaiwan_Open_Row02_Frame06.png
Animation_Brewing_Gaiwan_Open_Row02_Frame07.png
Animation_Brewing_Gaiwan_Open_Row02_Frame08.png
```

#### Row 3 — Steam Effect (8프레임)
```
Effect_Steam_Rise_Frame01.png
Effect_Steam_Rise_Frame02.png
Effect_Steam_Rise_Frame03.png
Effect_Steam_Rise_Frame04.png
Effect_Steam_Rise_Frame05.png
Effect_Steam_Rise_Frame06.png
Effect_Steam_Rise_Frame07.png
Effect_Steam_Rise_Frame08.png
```

#### Row 4 — TeaCup Fill (Side View) (8프레임)
```
Animation_Brewing_TeaCup_Fill_Row04_Frame01.png
Animation_Brewing_TeaCup_Fill_Row04_Frame02.png
Animation_Brewing_TeaCup_Fill_Row04_Frame03.png
Animation_Brewing_TeaCup_Fill_Row04_Frame04.png
Animation_Brewing_TeaCup_Fill_Row04_Frame05.png
Animation_Brewing_TeaCup_Fill_Row04_Frame06.png
Animation_Brewing_TeaCup_Fill_Row04_Frame07.png
Animation_Brewing_TeaCup_Fill_Row04_Frame08.png
```

#### Row 5 — Honey Add (5프레임)
```
Animation_Brewing_Honey_Add_Row05_Frame01.png
Animation_Brewing_Honey_Add_Row05_Frame02.png
Animation_Brewing_Honey_Add_Row05_Frame03.png
Animation_Brewing_Honey_Add_Row05_Frame04.png
Animation_Brewing_Honey_Add_Row05_Frame05.png
```

#### Row 6 — Cream Pour (5프레임)
```
Animation_Brewing_Cream_Pour_Row06_Frame01.png
Animation_Brewing_Cream_Pour_Row06_Frame02.png
Animation_Brewing_Cream_Pour_Row06_Frame03.png
Animation_Brewing_Cream_Pour_Row06_Frame04.png
Animation_Brewing_Cream_Pour_Row06_Frame05.png
```

#### Row 7 — TeaCup Stir (Top View) (8프레임)
```
Animation_Brewing_TeaCup_Stir_Row07_Frame01.png
Animation_Brewing_TeaCup_Stir_Row07_Frame02.png
Animation_Brewing_TeaCup_Stir_Row07_Frame03.png
Animation_Brewing_TeaCup_Stir_Row07_Frame04.png
Animation_Brewing_TeaCup_Stir_Row07_Frame05.png
Animation_Brewing_TeaCup_Stir_Row07_Frame06.png
Animation_Brewing_TeaCup_Stir_Row07_Frame07.png
Animation_Brewing_TeaCup_Stir_Row07_Frame08.png
```

#### Row 8 — TeaCup Finish (Isometric) (6프레임)
```
Animation_Brewing_TeaCup_Finish_Row08_Frame01.png
Animation_Brewing_TeaCup_Finish_Row08_Frame02.png
Animation_Brewing_TeaCup_Finish_Row08_Frame03.png
Animation_Brewing_TeaCup_Finish_Row08_Frame04.png
Animation_Brewing_TeaCup_Finish_Row08_Frame05.png
Animation_Brewing_TeaCup_Finish_Row08_Frame06.png
```

---

### 독립 오브젝트 이름 (세트 분리 시)

| 오브젝트 | Asset 이름 | 설명 |
|---------|-----------|------|
| 검은 주전자 | `TeaWare_TeaPot_Black_Side_01` | Row1 주전자 정지 이미지 |
| 나무 찻잔 받침 | `TeaWare_TeaTray_Wood_Brown_01` | Row1/2 공통 받침 |
| 개완 (덮개 포함) | `TeaWare_Gaiwan_White_Floral_01` | Row2 개완 정지 이미지 |
| 찻잔 (측면, 비어있음) | `TeaWare_Cup_Side_Empty_Green_01` | Row4 Frame01 기준 |
| 꿀 병 | `Ingredient_Honey_Jar_Gold_01` | Row5 꿀 병 |
| 꿀 국자 | `Ingredient_Honey_Dipper_Wood_01` | Row5 국자 |
| 크리머 주전자 | `TeaWare_Creamer_White_01` | Row6 크리머 |
| 찻잔 (탑뷰) | `TeaWare_Cup_Top_Floral_01` | Row7 탑뷰 |
| 티스푼 | `TeaWare_Spoon_Wood_01` | Row7 스푼 |
| 찻잔 + 받침 세트 | `TeaWare_Cup_Saucer_Set_01` | Row8 완성형 세트 |

---

## PNG #15 — `fed646b9` — 건축 구조/인테리어 부품 시트

### 바닥재 (Floor) — 7개

| # | Asset 이름 | 설명 |
|---|-----------|------|
| 1 | `Floor_Wood_Light_Korea_01` | 밝은 나무 바닥 (원근 뷰) |
| 2 | `Floor_Wood_Dark_Korea_01` | 어두운 나무 바닥 |
| 3 | `Floor_Wood_Glossy_01` | 광택 나무 바닥 |
| 4 | `Floor_Tatami_Japan_01` | 다다미 바닥 (녹색 격자) |
| 5 | `Floor_Stone_Gray_01` | 회색 석재 바닥 |
| 6 | `Floor_Bamboo_Natural_01` | 대나무 바닥 |
| 7 | `Floor_Ornate_China_01` | 장식 문양 바닥 (중국식) |

### 벽 패널 (Wall) — 6개

| # | Asset 이름 | 설명 |
|---|-----------|------|
| 1 | `Wall_Panel_Plain_Hanji_01` | 단색 한지 패널 |
| 2 | `Wall_Panel_Grid_Korea_01` | 격자 한지 패널 |
| 3 | `Wall_Panel_Solid_Korea_01` | 민무늬 벽 패널 |
| 4 | `Wall_Panel_Wave_Japan_01` | 파도 문양 패널 (일본식) |
| 5 | `Wall_Panel_Cream_01` | 크림색 패널 |
| 6 | `Wall_Panel_Wood_Dark_01` | 어두운 나무 패널 |

### 구조물 — 기둥/보 (Structure) — 4개

| # | Asset 이름 | 설명 |
|---|-----------|------|
| 1 | `Structure_Pillar_Wood_Korea_01` | 세로 기둥 (단면) |
| 2 | `Structure_Beam_Horizontal_01` | 수평 보 |
| 3 | `Structure_Beam_Corner_01` | 꺾인 모서리 보 |
| 4 | `Structure_Beam_Decorated_01` | 장식 수평 보 (조각 포함) |

### 창문 (Window) — 4개

| # | Asset 이름 | 설명 |
|---|-----------|------|
| 1 | `Window_Grid_Small_Korea_01` | 소형 격자 창문 (한국) |
| 2 | `Window_Grid_Triple_Korea_01` | 3칸 격자 창문 (한국) |
| 3 | `Window_Shoji_Slide_Japan_01` | 슬라이딩 장지 창문 (일본) |
| 4 | `Window_Round_Ornate_China_01` | 원형 장식 창문 (중국) |

### 문 (Door) — 3개

| # | Asset 이름 | 설명 |
|---|-----------|------|
| 1 | `Door_Hanok_Single_Korea_01` | 한옥 단문 |
| 2 | `Door_Hanok_Double_Korea_01` | 한옥 이중 슬라이딩문 |
| 3 | `Door_Entrance_Ornate_China_01` | 정문 입구 (중국풍 장식) |

### 선반 (Shelf) — 2개

| # | Asset 이름 | 설명 |
|---|-----------|------|
| 1 | `Shelf_Wood_Simple_Korea_01` | 단순 목재 선반 |
| 2 | `Shelf_Gold_Ornate_China_01` | 금 장식 선반 (중국식) |

### 기단/몰딩 (Base) — 4개

| # | Asset 이름 | 설명 |
|---|-----------|------|
| 1 | `Base_Floor_Molding_Korea_01` | 바닥 기단 (단순) |
| 2 | `Base_Floor_Molding_Korea_02` | 바닥 기단 (중형) |
| 3 | `Base_Floor_Molding_Korea_03` | 바닥 기단 (대형) |
| 4 | `Base_Floor_Molding_China_01` | 바닥 기단 (중국 장식) |

---

## Part03 네이밍 요약

| PNG | Asset 종류 | 총 이름 수 |
|-----|-----------|-----------|
| `f837fa6a...` | 애니메이션 프레임 + 독립 오브젝트 | **57개 프레임 + 10개 정지 오브젝트** |
| `fed646b9...` | 건축/인테리어 부품 | **30개** |
| **합계** | | **~97개** |

---

## 통합 네이밍 Prefix 참조 (Part03 기준)

| Prefix | 의미 |
|--------|------|
| `Animation_Brewing_` | 브루잉 과정 애니메이션 프레임 |
| `Anim_` | AnimationClip 식별자 |
| `Effect_` | 시각 이펙트 (Steam 등) |
| `TeaWare_` | 찻잔/주전자 등 티웨어 |
| `Ingredient_` | 재료 오브젝트 (꿀, 크림 등) |
| `Floor_` | 바닥재 |
| `Wall_` | 벽 패널 |
| `Window_` | 창문 |
| `Door_` | 문 |
| `Shelf_` | 선반 |
| `Structure_` | 기둥/보 구조물 |
| `Base_` | 기단/몰딩 |

