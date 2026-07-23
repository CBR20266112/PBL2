using System.Collections;
using UnityEngine;

/// <summary>
/// DialogManager의 큐를 소비하고, DialogView에 다국어 번역 데이터를 전달하며,
/// ESC 키 입력을 감지/해석하여 요청자의 콜백(Action<DialogResult>)으로 결과를 반환하는 Controller 클래스입니다.
/// </summary>
public class DialogController : MonoBehaviour
{
    [SerializeField] private DialogView view;

    private Coroutine _queueProcessCoroutine;
    private DialogData _activeDialogData;
    private bool _isWaitingForUserResponse = false;
    private DialogResult _userResponse = DialogResult.Cancel;

    private void OnEnable()
    {
        // 1. DialogManager 이벤트 구독
        if (DialogManager.Instance != null)
        {
            DialogManager.Instance.OnDialogEnqueued += HandleDialogEnqueued;
        }

        // 2. DialogView 버튼 이벤트 구독
        if (view != null)
        {
            view.OnConfirmClicked += HandleConfirmClicked;
            view.OnCancelClicked += HandleCancelClicked;
        }

        // 3. 큐 가동 시작
        StartQueueProcessing();
    }

    private void OnDisable()
    {
        if (DialogManager.Instance != null)
        {
            DialogManager.Instance.OnDialogEnqueued -= HandleDialogEnqueued;
        }

        if (view != null)
        {
            view.OnConfirmClicked -= HandleConfirmClicked;
            view.OnCancelClicked -= HandleCancelClicked;
        }

        if (_queueProcessCoroutine != null)
        {
            StopCoroutine(_queueProcessCoroutine);
            _queueProcessCoroutine = null;
        }
    }

    /// <summary>
    /// ESC 키 입력 감지 및 해석 (수정 지침 4번: Controller가 입력 감지 담당)
    /// </summary>
    private void Update()
    {
        if (_isWaitingForUserResponse && _activeDialogData != null && _activeDialogData.allowEscClose)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("[DialogController] ESC 키 입력 감지 -> Cancel 처리");
                HandleCancelClicked();
            }
        }
    }

    private void HandleDialogEnqueued()
    {
        StartQueueProcessing();
    }

    private void StartQueueProcessing()
    {
        if (_queueProcessCoroutine == null)
        {
            _queueProcessCoroutine = StartCoroutine(ProcessQueueCoroutine());
        }
    }

    /// <summary>
    /// DialogManager의 큐를 소비하여 하나씩 대화상자를 출력하는 코루틴 루프
    /// </summary>
    private IEnumerator ProcessQueueCoroutine()
    {
        while (DialogManager.Instance != null && DialogManager.Instance.HasPendingDialogs)
        {
            if (!_isWaitingForUserResponse && DialogManager.Instance.TryDequeue(out DialogData data))
            {
                _activeDialogData = data;
                _isWaitingForUserResponse = true;
                _userResponse = DialogResult.Cancel;

                // DialogManager 상태 세팅
                DialogManager.Instance.SetCurrentDialog(data);

                // Display DTO 생성 (번역 포맷팅 완료)
                DialogDisplayData displayData = CreateDisplayData(data);

                if (view != null)
                {
                    view.ShowDialog(displayData);
                }

                // 사용자 응답(버튼 클릭 또는 ESC) 대기
                while (_isWaitingForUserResponse)
                {
                    yield return null;
                }

                // 대화상자 Hide 연출 진행
                bool isHideComplete = false;
                if (view != null)
                {
                    view.HideDialog(() => isHideComplete = true);
                    while (!isHideComplete)
                    {
                        yield return null;
                    }
                }

                // DialogManager 상태 해제
                DialogManager.Instance.ClearCurrentDialog();

                // 요청자 콜백 실행 (Action<DialogResult>)
                data.onResult?.Invoke(_userResponse);
                _activeDialogData = null;
            }
            else
            {
                yield return null;
            }
        }

        _queueProcessCoroutine = null;
    }

    private void HandleConfirmClicked()
    {
        if (!_isWaitingForUserResponse) return;
        _userResponse = DialogResult.Confirm;
        _isWaitingForUserResponse = false;
    }

    private void HandleCancelClicked()
    {
        if (!_isWaitingForUserResponse) return;
        _userResponse = DialogResult.Cancel;
        _isWaitingForUserResponse = false;
    }

    /// <summary>
    /// DialogData의 다국어 키를 LocalizationManager에서 조회하여 DialogDisplayData를 생성합니다.
    /// </summary>
    private DialogDisplayData CreateDisplayData(DialogData data)
    {
        string titleText = GetLocalizedText(data.titleKey);
        string descText = GetLocalizedText(data.descriptionKey);
        string confirmBtnText = GetLocalizedText(data.confirmButtonKey);
        string cancelBtnText = GetLocalizedText(data.cancelButtonKey);

        return new DialogDisplayData
        {
            titleText = titleText,
            descriptionText = descText,
            confirmButtonText = confirmBtnText,
            cancelButtonText = cancelBtnText,
            showCancelButton = data.showCancelButton
        };
    }

    private string GetLocalizedText(string key)
    {
        if (string.IsNullOrEmpty(key)) return string.Empty;

        if (LocalizationManager.Instance != null)
        {
            string text = LocalizationManager.Instance.GetText(key);
            if (!string.IsNullOrEmpty(text)) return text;
        }

        return key;
    }
}
