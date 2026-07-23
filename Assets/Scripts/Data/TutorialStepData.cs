using UnityEngine;

/// <summary>
/// 튜토리얼의 단일 단계 데이터를 정의하는 ScriptableObject 에셋입니다.
/// </summary>
[CreateAssetMenu(fileName = "TutorialStep_", menuName = "Tea Culture Game/Tutorial Step Data")]
public class TutorialStepData : ScriptableObject
{
    [Header("기본 정보")]
    [Tooltip("단계 고유 ID")]
    public string stepId;

    [Tooltip("다국어 제목 키")]
    public string titleKey;

    [Tooltip("다국어 안내 설명 키")]
    public string descriptionKey;

    [Header("진행 조건 및 연동")]
    [Tooltip("이 단계를 완료하기 위한 조건 유형")]
    public TutorialConditionType conditionType = TutorialConditionType.None;

    [Tooltip("이벤트 조건 달성 시 자동으로 다음 단계 진행 여부")]
    public bool autoAdvanceOnCondition = true;

    [Header("UI 제어")]
    [Tooltip("TutorialHighlightRegistry에 등록된 강조 대상 UI ID")]
    public string highlightTargetId;

    [Tooltip("이 단계에서 건너뛰기 버튼 사용 허용 여부")]
    public bool allowSkip = true;
}
