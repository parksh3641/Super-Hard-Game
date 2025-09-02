using UnityEngine;
using UnityEngine.UI;

public class MinimapPlayerIcon : MonoBehaviour
{
    public Transform player;             // 플레이어의 Transform
    public Camera minimapCamera;         // Orthographic 미니맵 카메라
    public RectTransform playerIcon;     // 빨간색 플레이어 아이콘 (UI Image)
    public RectTransform minimapPanel;   // 미니맵 UI의 RectTransform (RawImage 영역)

    void Update()
    {
        UpdatePlayerIconPosition();
    }

    private void UpdatePlayerIconPosition()
    {
        // 플레이어의 월드 좌표를 미니맵 카메라의 뷰포트 좌표로 변환
        Vector3 viewportPosition = minimapCamera.WorldToViewportPoint(player.position);

        // 뷰포트 좌표 (0~1)를 미니맵 UI 좌표로 변환
        float x = (viewportPosition.x - 0.5f) * minimapPanel.rect.width;
        float y = (viewportPosition.y - 0.5f) * minimapPanel.rect.height;

        // 아이콘 위치 설정 (미니맵의 중심을 기준으로 이동)
        playerIcon.anchoredPosition = new Vector2(x, y);
    }
}
