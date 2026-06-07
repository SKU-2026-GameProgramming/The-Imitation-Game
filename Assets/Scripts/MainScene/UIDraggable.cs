using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 유니티 UI(uGUI) 요소에 부착하여 마우스나 터치 드래그로 위치를 이동시키는 컴포넌트입니다.
/// </summary>
public class UIDraggable : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    // 이동시킬 대상 UI의 RectTransform (비어있으면 이 컴포넌트가 붙은 오브젝트를 이동)
    [SerializeField] private RectTransform targetRectTransform;

    // 캔버스 전체의 참조 (좌표 계산용)
    private Canvas _canvas;

    private void Awake()
    {
        // 최상위 혹은 부모에 있는 Canvas 컴포넌트를 자동으로 찾습니다.
        _canvas = GetComponentInParent<Canvas>();

        if (targetRectTransform == null)
        {
            targetRectTransform = GetComponent<RectTransform>();
        }
    }

    /// <summary>
    /// UI를 클릭(터치)하는 순간 호출됩니다.
    /// 창을 드래그할 때 다른 UI보다 화면 맨 앞으로 오게 설정합니다.
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        // 클릭한 UI 창을 계층 구조의 맨 아래로 내려서 화면상 맨 앞으로 오게 만듭니다.
        targetRectTransform.SetAsLastSibling();
    }

    /// <summary>
    /// 드래그 중일 때 매 프레임 호출됩니다.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (_canvas == null) return;

        // 마우스의 움직임 델타(delta) 값을 캔버스 스케일에 맞춰 변환한 뒤, UI의 위치에 더해줍니다.
        // CanvasScaler가 적용되어 있어도 해상도에 맞게 정확하게 움직이도록 보정하는 공식입니다.
        targetRectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }
}
