using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 화면에 손님 캐릭터의 시각적 요소를 표시하고 숨기는 UI 전용 View 클래스입니다.
/// 데이터 이벤트나 로직에 직접 의존하지 않고 외부(Presenter/GameFlow)의 명시적 호출로 작동합니다.
/// Instantiate/Destroy를 수행하지 않으며 활성화/비활성화 제어만 담당하여 오브젝트 풀링에 용이합니다.
/// </summary>
public class CustomerView : MonoBehaviour
{
    [Header("UI 시각 요소 바인딩")]
    [Tooltip("손님 스탠딩 이미지를 표시할 UI Image")]
    [SerializeField] private Image _standingUIImage;

    [Tooltip("손님 초상화 아이콘을 표시할 UI Image (선택 사항)")]
    [SerializeField] private Image _portraitImage;

    // 현재 노출 중인 손님 데이터
    private CustomerData _currentVisibleCustomer;

    /// <summary>
    /// 전달받은 CustomerData의 에셋(sprite, portrait)을 UI Image에 적용하고 표시합니다.
    /// </summary>
    /// <param name="customer">표시할 CustomerData</param>
    public void ShowCustomer(CustomerData customer)
    {
        if (customer == null)
        {
            HideCustomer();
            return;
        }

        _currentVisibleCustomer = customer;

        // 1. UI 스탠딩 이미지 바인딩 (customer.sprite 사용)
        if (_standingUIImage != null)
        {
            _standingUIImage.sprite = customer.sprite;
            _standingUIImage.gameObject.SetActive(true);
        }

        // 2. UI 초상화 이미지 바인딩 (customer.portrait 사용)
        if (_portraitImage != null)
        {
            _portraitImage.sprite = customer.portrait;
            _portraitImage.gameObject.SetActive(true);
        }

        Debug.Log($"[CustomerView] 손님 UI 노출: {customer.customerId}");
    }

    /// <summary>
    /// 손님 UI 요소를 비활성화하여 화면에서 숨깁니다.
    /// Instantiate/Destroy를 하지 않으므로 안전하게 재사용(Object Pooling)이 가능합니다.
    /// </summary>
    public void HideCustomer()
    {
        if (_standingUIImage != null)
        {
            _standingUIImage.gameObject.SetActive(false);
        }

        if (_portraitImage != null)
        {
            _portraitImage.gameObject.SetActive(false);
        }

        _currentVisibleCustomer = null;
        Debug.Log("[CustomerView] 손님 UI 숨김 처리 완료.");
    }

    /// <summary>
    /// 현재 화면에 노출 중인 손님이 존재하는지 여부를 반환합니다.
    /// </summary>
    public bool HasVisibleCustomer()
    {
        return _currentVisibleCustomer != null;
    }

    /// <summary>
    /// (Hook) 필요 시 외부에서 명시적으로 호출할 수 있는 등장 애니메이션/연출 훅입니다.
    /// </summary>
    public void PlayEntranceAnimation()
    {
        // TODO: 향후 애니메이션 트윈/페이드인 연동
        Debug.Log("[CustomerView] PlayEntranceAnimation Hook 호출됨.");
    }

    /// <summary>
    /// (Hook) 필요 시 외부에서 명시적으로 호출할 수 있는 퇴장 애니메이션/연출 훅입니다.
    /// </summary>
    public void PlayExitAnimation()
    {
        // TODO: 향후 퇴장 연출 연동
        Debug.Log("[CustomerView] PlayExitAnimation Hook 호출됨.");
    }
}
