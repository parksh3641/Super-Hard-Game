using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float zoomSpeed = 5.0f; // �� �ӵ�
    public float minZoom = 4.0f;   // �ּ� �� ��
    public float maxZoom = 6.0f;   // �ִ� �� ��
    public float defaultZoom = 5.0f; // �⺻ �� ��

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();

        if (cam == null)
        {
            Debug.LogError("CameraZoom ��ũ��Ʈ�� ī�޶� ������Ʈ�� ã�� ���߽��ϴ�.");
            return;
        }

        cam.orthographicSize = defaultZoom; // �⺻ �� ����
    }

    void Update()
    {
        if (cam == null) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel"); // ���콺 �� �Է� �ޱ�

        if (scroll != 0.0f)
        {
            cam.orthographicSize -= scroll * zoomSpeed; // �� ��ũ�ѿ� ���� �� �� ����
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom); // �ּ�/�ִ� ������ ����
        }
    }
}
