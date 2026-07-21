# 오디오 마스터 문서 (Audio Master Document)

이 문서는 **Tea Culture Café**의 모든 사운드 디자인(BGM 및 SFX) 기획과 기술적 구현 가이드를 통합한 마스터 문서입니다. 새로운 사운드 리소스를 제작하거나, 오디오 시스템을 확장할 때 참조하는 단일 진실 공급원(Single Source of Truth)으로 사용됩니다.

---

## 1. 사운드 디자인 철학 (Sound Design Philosophy)
게임 내 모든 오디오는 게임 플레이를 지배하지 않으며, 아래의 감성적 목표를 조용히 뒷받침해야 합니다.
- **핵심 감성**: 편안함 (Comfort), 따뜻함 (Warmth), 장인정신 (Craftsmanship), 슬로우 라이프 (Slow living)
- **금지 사항**: 과도한 감정 기복, 화려한 오케스트레이션 전개, 거친 신스음, 자극적 타격음, 현대적 일렉트로닉 비트

---

## 2. BGM 및 Ambience 디자인 가이드

모든 BGM은 55~75 BPM 사이의 매우 느린 템포를 유지하며, 끊김 없는 자연스러운 루프(Loop)로 제작되어야 합니다. 긴 게임 세션 동안 들어도 피로감이 없어야 합니다.

### 2.1 Default (기본 카페)
- **분위기**: 전통 동아시아 다과점, 따뜻한 환대, 나무 인테리어.
- **주요 악기**: 어쿠스틱 피아노, 고쟁/고토 류의 발현악기, 샤쿠하치, 부드러운 바람.

### 2.2 Korea (한국 테마)
- **분위기**: 평화로운 한옥 다실, 마룻바닥, 가을 오후, 소박한 아름다움.
- **주요 악기**: 가야금, 대금, 부드러운 장구 질감, 창호지 문을 스치는 바람.

### 2.3 China (중국 테마)
- **분위기**: 대나무 숲 다정, 고전 정원, 흐르는 물, 아침 안개.
- **주요 악기**: 디즈, 얼후(매우 부드럽게), 고쟁, 물소리, 대나무 흔들리는 소리.

### 2.4 Japan (일본 테마)
- **분위기**: 고요한 젠 가든, 다다미, 균형과 집중, 차선(Tea whisk)으로 차를 내는 고요함.
- **주요 악기**: 고토, 샤쿠하치, 시시오도시(대나무 분수), 사찰 종소리(미세하게).

### 2.5 기타 테마 (Vietnam, Kyrgyzstan)
- (향후 업데이트 예정 - 각 국가의 차 문화에 맞는 자연 앰비언스와 부드러운 전통 악기 혼용)

---

## 3. SFX 디자인 가이드

### 3.1 카테고리 분류 (6종)
모든 효과음은 아래 6가지 카테고리로 엄격히 분류됩니다.
1. **UI**: 버튼, 호버, 팝업, 알림
2. **Tea**: 물 따르기, 차 젓기, 주전자 끓는 소리
3. **Cooking**: 재료 섞기, 썰기, 준비 완료
4. **Customer**: 손님 입장, 반응(만족/실망), 결제
5. **Environment**: 룸 톤, 자연(새/바람) 루프 앰비언스
6. **System**: 레벨업, 해금, 세이브

### 3.2 명명 규칙 및 베리에이션
- **명명 규칙**: `[카테고리]_[액션]_[번호]` (예: `UI_Button_Click_01`, `Tea_Pour_02`)
- **베리에이션**: 반복 피로도를 줄이기 위해 동일 액션에 2~3개 이상의 오디오 샘플 베리에이션 필수.

---

## 4. 기술 구현 가이드 (Technical Implementation)

### 4.1 AudioMixer 아키텍처
```text
Master
 ├── BGM (CrossFade 지원)
 ├── SFX (UI, Tea, Cooking, Customer 등 서브그룹)
 └── Ambience (Fade In/Out 지원)
```

### 4.2 오디오 동시 재생 제어 (Polyphony & Randomization)
- **Pitch/Volume 랜덤화**: 찻잔 소리, 발걸음 등 짧은 SFX 재생 시 AudioManager 내부에서 피치(0.95~1.05)와 볼륨(-2dB~0)에 미세한 난수 적용.
- **Polyphony (동시 재생 제한)**: AudioSource 풀(Pool)의 낭비를 막고 오디오 피크를 방지하기 위해 카테고리별 최대 동시 재생 수 설정. (예: UI는 1, Cooking은 3, Customer Voice는 8)
- **Priority**: UI/시스템(0~32), 액션(33~80), 환경음(129~) 순으로 중요도 지정.

### 4.3 시스템 관리 구조
- **AudioManager**: GC-Free `AudioSourcePool`을 관리하는 싱글톤. 테마 간 음악 및 앰비언스 전환 시 DOTween/Coroutine을 활용한 부드러운 크로스페이드 처리.
- **ThemeDatabase & SfxDatabase**: `ScriptableObject` 기반의 데이터 중심 아키텍처(Data-Driven). 테마/SFX가 수백 개 추가되어도 코드 수정 없이 Inspector에서 ID로 매핑 관리. (최초 Awake 시 딕셔너리로 Pre-warming 처리됨)
- **상태 연동**: `SettingsManager`를 통해 PlayerPrefs에 사용자 볼륨 설정이 영구 저장되며, AudioMixer의 데시벨(dB) 단위로 실시간 동기화됨.

### 4.4 Unity 에셋 임포트 (Import Settings)
- **UI**: `Decompress On Load`, PCM 포맷
- **SFX**: `Compressed In Memory`, Vorbis 포맷
- **BGM/Ambience**: `Streaming`, Vorbis 포맷
