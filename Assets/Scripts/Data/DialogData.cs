using System;

/// <summary>
/// 대화상자 생성 요청 데이터를 보관하는 순수 DTO 클래스입니다.
/// </summary>
public class DialogData
{
    public DialogType dialogType = DialogType.Confirm;
    public string titleKey;
    public string descriptionKey;
    public string confirmButtonKey;
    public string cancelButtonKey;
    public bool showCancelButton = true;
    public bool allowEscClose = true;
    public Action<DialogResult> onResult;

    public DialogData() { }

    public DialogData(
        DialogType type,
        string titleKey,
        string descriptionKey,
        Action<DialogResult> onResult,
        string confirmBtnKey = "dialog_btn_confirm",
        string cancelBtnKey = "dialog_btn_cancel",
        bool showCancel = true,
        bool allowEsc = true)
    {
        this.dialogType = type;
        this.titleKey = titleKey;
        this.descriptionKey = descriptionKey;
        this.onResult = onResult;
        this.confirmButtonKey = confirmBtnKey;
        this.cancelButtonKey = cancelBtnKey;
        this.showCancelButton = showCancel;
        this.allowEscClose = allowEsc;
    }
}
