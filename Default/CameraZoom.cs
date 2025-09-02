using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float zoomSpeed = 5.0f; // 줌 속도
    public float minZoom = 4.0f;   // 최소 줌 값
    public float maxZoom = 6.0f;   // 최대 줌 값
    public float defaultZoom = 5.0f; // 기본 줌 값

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();

        if (cam == null)
        {
            Debug.LogError("CameraZoom 스크립트가 카메라 컴포넌트를 찾지 못했습니다.");
            return;
        }

        cam.orthographicSize = defaultZoom; // 기본 줌 설정
    }

    void Update()
    {
        if (cam == null) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel"); // 마우스 휠 입력 받기

        if (scroll != 0.0f)
        {
            cam.orthographicSize -= scroll * zoomSpeed; // 휠 스크롤에 따라 줌 값 변경
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom); // 최소/최대 값으로 제한
        }
    }
}
