# PBL2 00_PROJECT_MASTER 문서 종합 가이드 (INDEX)

프로젝트 내 개발 규칙, 인프라, 게임플레이, UI 및 이력 관리를 통합 관리하는 단일 기준점(Source of Truth) 문서 목차입니다.

## 📌 프로젝트 핵심 개발 및 검증 현황 요약
- [VERIFICATION_STATUS_SUMMARY.md](file:///c:/Users/vipgo/Dev/PBL2/00_PROJECT_MASTER/VERIFICATION_STATUS_SUMMARY.md)
  - 코드 구현(✅ 완료), 정적 검증(✅ 완료), 자동화 검증(✅ 완료), Unity Play Mode 검증(⏳ 사용자 실행 필요) 표 요약

---

## 📌 핵심 개발 및 설계 문서

1. [AI_DEVELOPMENT_RULES.md](file:///c:/Users/vipgo/Dev/PBL2/00_PROJECT_MASTER/AI_DEVELOPMENT_RULES.md)
   - 프로젝트 **단일 규칙 기준서** (한국어 응답, MVP 패턴, 단방향 의존성 등)
2. [CHANGELOG.md](file:///c:/Users/vipgo/Dev/PBL2/00_PROJECT_MASTER/CHANGELOG.md)
   - 버전별 기능 추가 및 변경 사항 종합 기록 (v1.0.0 ~ v1.34.0)
3. [ERROR_RESOLVED_LOG.md](file:///c:/Users/vipgo/Dev/PBL2/00_PROJECT_MASTER/ERROR_RESOLVED_LOG.md)
   - 조치 및 해결된 주요 컴파일/구문 오류 요약 이력 (0 Errors / 0 Warnings 상태 유지)
4. [SMOKE_TEST_CHECKLIST.md](file:///c:/Users/vipgo/Dev/PBL2/00_PROJECT_MASTER/SMOKE_TEST_CHECKLIST.md)
   - Integration Phase 전체 플레이 스모크 테스트 17가지 체크리스트
5. [GAMEPLAY_VERTICAL_SLICE_REPORT.md](file:///c:/Users/vipgo/Dev/PBL2/00_PROJECT_MASTER/GAMEPLAY_VERTICAL_SLICE_REPORT.md)
   - Gameplay Vertical Slice 통합 검증, 발견된 문제 및 수정 버그 보고서
6. [CONTENT_CREATION_REPORT.md](file:///c:/Users/vipgo/Dev/PBL2/00_PROJECT_MASTER/CONTENT_CREATION_REPORT.md)
   - 초반/중반/후반 티어별 레시피, 재료, 손님, 상점 밸런스 제작 보고서
7. [CONTENT_EXPANSION_REPORT.md](file:///c:/Users/vipgo/Dev/PBL2/00_PROJECT_MASTER/CONTENT_EXPANSION_REPORT.md)
   - 대규모 콘텐츠 확장(재료 22+, 음료 32+, 레시피 32+, 손님 20+, 가구 20+, 해금 35+) 및 Validator 결과
8. [TWENTY_DAY_PLAYTEST_REPORT.md](file:///c:/Users/vipgo/Dev/PBL2/00_PROJECT_MASTER/TWENTY_DAY_PLAYTEST_REPORT.md)
   - 20일 플레이 시뮬레이션 검증, 발견된 문제, 원인 및 수치 수정 내역 보고서
9. [FTUE_IMPROVEMENT_REPORT.md](file:///c:/Users/vipgo/Dev/PBL2/00_PROJECT_MASTER/FTUE_IMPROVEMENT_REPORT.md)
   - Production Phase Task #1: 첫 플레이(FTUE) 경험 및 피드백/동선 개선 보고서
10. [JUICE_ENHANCEMENT_REPORT.md](file:///c:/Users/vipgo/Dev/PBL2/00_PROJECT_MASTER/JUICE_ENHANCEMENT_REPORT.md)
   - Production Phase Task #2: 게임 Juice (시각/청각 손맛 연출) 강화 보고서
11. [RELEASE_CANDIDATE_QA_REPORT.md](file:///c:/Users/vipgo/Dev/PBL2/00_PROJECT_MASTER/RELEASE_CANDIDATE_QA_REPORT.md)
   - Production Phase Task #3: Release Candidate (RC) 15대 필수 항목 QA 전수 검증 및 잔여 버그 0개 최종 보고서
12. [AUDIO_MASTER_DOCUMENT.md](file:///c:/Users/vipgo/Dev/PBL2/00_PROJECT_MASTER/AUDIO_MASTER_DOCUMENT.md) & [ART_ASSET_IMPORT_GUIDE.md](file:///c:/Users/vipgo/Dev/PBL2/00_PROJECT_MASTER/ART_ASSET_IMPORT_GUIDE.md)
   - 사운드 및 아트 에셋 임포트/가이드 통합 관리서

---

## 📊 로그 및 자동 갱신 체계

- **엑셀 이력 파일**: `UPDATE_LOG.xlsx` (작업 시 파이썬 `append_log_final.py` 스크립트로 자동 누적 기록)
- **최신 갱신 파일**: [append_log_final.py](file:///c:/Users/vipgo/Dev/PBL2/append_log_final.py)

---

## 🛠️ Infrastructure & Gameplay System 현황

| 분류 | 주요 매니저 / 시스템 | 상태 |
| :--- | :--- | :---: |
| **Infrastructure** | Audio, Save, Localization, Settings, Notification, Dialog, SceneTransition, DebugPanel | ✅ 완료 |
| **Gameplay** | Brewing, Inventory, Shop, Money, Day, Customer, Order, Serving, Affinity, Unlock, Furniture | ✅ 완료 |
| **UI Framework** | MainMenu, Settings, HUD, Brewing, Inventory, Shop, Furniture, Tutorial, Notification, Dialog | ✅ 완료 |
