using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform background; // 조이스틱 배경
    public RectTransform handle;     // 조이스틱 핸들
    public float joystickRadius = 100f; // 핸들이 조이스틱을 벗어나지 않도록 제한하는 반지름

    private Vector2 inputVector = Vector2.zero;
    private bool isJoystickActive = false;
    private bool isMobile;

    void Start()
    {
        background.gameObject.SetActive(false); // 시작할 때 조이스틱 숨김

        isMobile = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer);

        if (!isMobile)
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!isMobile || !GameStateManager.instance.IsPlaying)
        {
            background.gameObject.SetActive(false);
            return;
        }

        // 마우스 클릭 시 조이스틱 활성화
        if (Input.GetMouseButtonDown(0))
        {
            ActivateJoystick(Input.mousePosition);
        }

        // 마우스 드래그 시 조이스틱 핸들 이동
        if (isJoystickActive && Input.GetMouseButton(0))
        {
            DragJoystick(Input.mousePosition);
        }

        // 마우스 버튼을 떼면 조이스틱 비활성화
        if (Input.GetMouseButtonUp(0))
        {
            DeactivateJoystick();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ActivateJoystick(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        DragJoystick(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        DeactivateJoystick();
    }

    private void ActivateJoystick(Vector2 position)
    {
        background.position = position;  // 클릭한 위치에 조이스틱 배경 설정
        background.gameObject.SetActive(true);
        handle.anchoredPosition = Vector2.zero; // 핸들 초기화
        isJoystickActive = true;
    }

    private void DragJoystick(Vector2 position)
    {
        Vector2 direction = position - (Vector2)background.position; // 클릭한 위치 - 조이스틱 중심 위치
        float distance = Mathf.Clamp(direction.magnitude, 0, joystickRadius); // 최대 반경 내로 제한
        Vector2 clampedDirection = direction.normalized * distance; // 방향을 유지한 채 거리 제한

        handle.anchoredPosition = clampedDirection; // 핸들 위치 업데이트
        inputVector = clampedDirection / joystickRadius; // 정규화된 입력 벡터 저장
    }

    private void DeactivateJoystick()
    {
        isJoystickActive = false;
        background.gameObject.SetActive(false); // 조이스틱 숨김
        inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }

    public Vector2 GetJoystickInput()
    {
        return inputVector; // x, y (-1 ~ 1 사이 값 반환)
    }
}
