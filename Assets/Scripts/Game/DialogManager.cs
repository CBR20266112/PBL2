using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// confirmation 대화상자 데이터의 큐(Queue) 저장 및 현재 표시 중인 Dialog 상태를 관리하는 독립 싱글톤 매니저입니다.
/// 다른 Manager를 직접 참조하지 않으며, 뷰 연출이나 번역은 Controller가 전담합니다.
/// </summary>
public class DialogManager : Singleton<DialogManager>
{
    private readonly Queue<DialogData> _dialogQueue = new Queue<DialogData>();

    /// <summary>
    /// 현재 활성화(표시 중)된 대화상자 데이터
    /// </summary>
    public DialogData CurrentDialog { get; private set; }

    /// <summary>
    /// 현재 대화상자가 표시되고 있는지 여부
    /// </summary>
    public bool IsDialogShowing => CurrentDialog != null;

    /// <summary>
    /// 대기 큐에 보관된 대화상자 수
    /// </summary>
    public int QueueCount => _dialogQueue.Count;

    /// <summary>
    /// 큐에 대기 중인 대화상자 요청이 있는지 여부
    /// </summary>
    public bool HasPendingDialogs => _dialogQueue.Count > 0;

    // 새로운 대화상자 요청이 큐에 추가될 때 발생하는 이벤트
    public event Action OnDialogEnqueued;

    // 현재 대화상자 상태가 변경될 때 발생하는 이벤트
    public event Action<DialogData> OnCurrentDialogChanged;

    /// <summary>
    /// 대화상자 생성 요청을 큐에 추가합니다.
    /// </summary>
    public void Enqueue(DialogData data)
    {
        if (data == null) return;

        _dialogQueue.Enqueue(data);
        Debug.Log($"[DialogManager] 대화상자 Enqueue: {data.dialogType} (대기 수: {_dialogQueue.Count})");
        OnDialogEnqueued?.Invoke();
    }

    /// <summary>
    /// Controller가 큐에서 요청을 하나씩 꺼내 처리하기 위한 API입니다.
    /// </summary>
    public bool TryDequeue(out DialogData data)
    {
        if (_dialogQueue.Count > 0)
        {
            data = _dialogQueue.Dequeue();
            return true;
        }

        data = null;
        return false;
    }

    /// <summary>
    /// 현재 표시 중인 Dialog 상태를 설정합니다.
    /// </summary>
    public void SetCurrentDialog(DialogData data)
    {
        CurrentDialog = data;
        OnCurrentDialogChanged?.Invoke(CurrentDialog);
    }

    /// <summary>
    /// 현재 표시 중인 Dialog 상태를 해제합니다.
    /// </summary>
    public void ClearCurrentDialog()
    {
        CurrentDialog = null;
        OnCurrentDialogChanged?.Invoke(null);
    }

    /// <summary>
    /// 대기 큐를 초기화합니다.
    /// </summary>
    public void ClearQueue()
    {
        _dialogQueue.Clear();
    }
}
