/// <summary>
/// DialogView 화면 표시 전용 DTO 클래스입니다.
/// Controller에서 번역 및 포맷팅이 완료된 최종 텍스트 정보만 포함하며, allowEscClose나 비즈니스 데이터는 포함하지 않습니다.
/// </summary>
public class DialogDisplayData
{
    public string titleText;
    public string descriptionText;
    public string confirmButtonText;
    public string cancelButtonText;
    public bool showCancelButton = true;
}
