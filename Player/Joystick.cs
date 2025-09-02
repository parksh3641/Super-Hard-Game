using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform background; // ���̽�ƽ ���
    public RectTransform handle;     // ���̽�ƽ �ڵ�
    public float joystickRadius = 100f; // �ڵ��� ���̽�ƽ�� ����� �ʵ��� �����ϴ� ������

    private Vector2 inputVector = Vector2.zero;
    private bool isJoystickActive = false;
    private bool isMobile;

    void Start()
    {
        background.gameObject.SetActive(false); // ������ �� ���̽�ƽ ����

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

        // ���콺 Ŭ�� �� ���̽�ƽ Ȱ��ȭ
        if (Input.GetMouseButtonDown(0))
        {
            ActivateJoystick(Input.mousePosition);
        }

        // ���콺 �巡�� �� ���̽�ƽ �ڵ� �̵�
        if (isJoystickActive && Input.GetMouseButton(0))
        {
            DragJoystick(Input.mousePosition);
        }

        // ���콺 ��ư�� ���� ���̽�ƽ ��Ȱ��ȭ
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
        background.position = position;  // Ŭ���� ��ġ�� ���̽�ƽ ��� ����
        background.gameObject.SetActive(true);
        handle.anchoredPosition = Vector2.zero; // �ڵ� �ʱ�ȭ
        isJoystickActive = true;
    }

    private void DragJoystick(Vector2 position)
    {
        Vector2 direction = position - (Vector2)background.position; // Ŭ���� ��ġ - ���̽�ƽ �߽� ��ġ
        float distance = Mathf.Clamp(direction.magnitude, 0, joystickRadius); // �ִ� �ݰ� ���� ����
        Vector2 clampedDirection = direction.normalized * distance; // ������ ������ ä �Ÿ� ����

        handle.anchoredPosition = clampedDirection; // �ڵ� ��ġ ������Ʈ
        inputVector = clampedDirection / joystickRadius; // ����ȭ�� �Է� ���� ����
    }

    private void DeactivateJoystick()
    {
        isJoystickActive = false;
        background.gameObject.SetActive(false); // ���̽�ƽ ����
        inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }

    public Vector2 GetJoystickInput()
    {
        return inputVector; // x, y (-1 ~ 1 ���� �� ��ȯ)
    }
}
