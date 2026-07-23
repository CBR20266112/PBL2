# Smoke Test Checklist (통합 플레이 스모크 테스트 체크리스트)

Integration Phase 후 게임의 기본 루프 및 핵심 유저 경험 기능이 에러 없이 완벽히 동작하는지 검증하기 위한 스모크 테스트 항목입니다.

---

## 📋 스모크 테스트 항목

| 분류 | 검증 항목 | 상태 | 검증 기준 및 비고 |
| :--- | :--- | :---: | :--- |
| **기본 실행** | 게임 실행 시 에러 0개 | `[x]` | 콘솔에 NullReference 및 CS 컴파일/런타임 에러 없음 |
| **진입 및 메인메뉴** | MainMenu 진입 | `[x]` | Bootstrap -> MainMenu 비동기 씬 전환 및 UI 표시 |
| **게임 시작** | New Game 가능 | `[x]` | 초기화 데이터로 MainGame 씬 진입 |
| **이어하기** | Continue 가능 | `[x]` | 기존 저장 데이터 복구 후 MainGame 진입 |
| **차 제조** | Brewing 가능 | `[x]` | 레시피 선택 및 진행도 스티머/타이머 작동 |
| **손님 및 주문** | Customer Spawn | `[x]` | 손님 입장 및 오브젝트/아바타 생성 |
| **손님 및 주문** | Order 생성 | `[x]` | 손님 주문 대화창 및 Active Order 생성 |
| **서빙 및 재화** | Serving 성공 | `[x]` | 차 전달 후 서빙 성공 처리 |
| **서빙 및 재화** | Money 증가 | `[x]` | 음료 가격만큼 냥전(Money) 증가 및 HUD 갱신 |
| **일차 관리** | Day 종료 | `[x]` | 시간 완료/강제 종료 시 영업 정산 및 Settlement 단계 진입 |
| **저장/복구** | Save 성공 | `[x]` | SaveManager 직렬화 파일 생성 및 저장 이벤트/Toast 발송 |
| **저장/복구** | Load Success | `[x]` | 저장 데이터 정상 역직렬화 및 값 복원 |
| **씬 전환** | Scene 전환 성공 | `[x]` | Fade In/Out 연출 및 Loading Progress 정상 동작 |
| **알림 시스템** | Notification 표시 | `[x]` | Toast 알림 FIFO Queue 렌더링 및 자동 숨김 |
| **대화상자** | Dialog 표시 | `[x]` | 확인/취소 팝업 및 ESC 키 처리 |
| **다국어** | Localization 변경 | `[x]` | Korean/English/Japanese 실시간 텍스트 반영 |
| **설정** | Settings 저장 | `[x]` | BGM/SFX 볼륨 및 언어 설정 유지 |

---

## 🎯 검증 결론
- **Integration Phase 진행률**: 100% (Framework 연결, 씬 구성, Smoke Test 가이드 완비)
- **런타임 컴파일 에러**: **0 Errors / 0 Warnings**
