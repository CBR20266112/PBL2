using UnityEngine;

/// <summary>
/// 각 국가별 테마의 오디오 및 메타 정보를 담는 데이터 컨테이너.
/// 코드를 수정하지 않고 새 국가 테마를 쉽게 추가할 수 있습니다.
/// </summary>
[CreateAssetMenu(fileName = "ThemeAudioData_", menuName = "Audio/ThemeAudioData")]
public class ThemeAudioData : ScriptableObject
{
    [Tooltip("테마의 고유 ID (예: Korea, China, Japan) - 기존 SoundTheme Enum 문자열과 매칭됩니다.")]
    public string themeID;

    [Tooltip("UI 등에 표시될 사용자 친화적인 이름 (예: 한국, 중국)")]
    public string displayName;

    [Header("Audio")]
    [Tooltip("이 테마의 대표 배경음악(BGM) 클립")]
    public AudioClip bgmClip;

    [Tooltip("이 테마의 대표 환경음(Ambience) 클립")]
    public AudioClip ambienceClip;

    [Header("UI & Meta")]
    [Tooltip("향후 UI에 활용될 테마 대표 색상")]
    public Color themeColor = Color.white;

    [TextArea]
    [Tooltip("이 테마에 대한 간단한 설명")]
    public string description;
}
