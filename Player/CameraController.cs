using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform player;          // 플레이어 Transform
    public Vector3 offset = new Vector3(0, 10, -10); // 카메라 오프셋
    public float smoothTime = 0.1f;    // 이동 부드럽기 정도 (값이 낮을수록 빠르게 이동)

    private Vector3 velocity = Vector3.zero; // SmoothDamp에 사용될 속도 참조값

    void LateUpdate()
    {
        if (player != null)
        {
            // 목표 위치 계산 (Y 값은 offset.y를 고정으로 사용)
            Vector3 targetPosition = new Vector3(
                player.position.x + offset.x,
                player.position.y + offset.y, // Y값은 offset.y 사용
                player.position.z + offset.z
            );

            // 부드러운 이동 (SmoothDamp 사용)
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}
