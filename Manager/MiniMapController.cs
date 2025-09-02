using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    public Camera miniMapCamera; // 미니맵 카메라 참조

    public GameObject miniMap;

    public float tileSpacingX = 10f; // 타일 간 X축 간격
    public float tileSpacingZ = -10f; // 타일 간 Z축 간격

    public float sizeMultiplier = 5f; // 한 칸당 사이즈 증가 비율
    public float sizeOffset = 2f; // 사이즈 추가 오프셋

    public float height = 10f; // Y축 고정 높이

    private void Start()
    {
#if UNITY_EDITOR || UNITY_EDITOR_64
        miniMap.SetActive(true);
#else
        miniMap.SetActive(false);
#endif
    }

    public void AdjustCamera(int mapWidth, int mapHeight)
    {
        if (miniMapCamera == null)
        {
            Debug.LogError("MiniMap Camera is not assigned.");
            return;
        }

        // 맵의 중앙 좌표 계산 (타일 간격을 고려)
        float centerX = (mapWidth - 1) * tileSpacingX / 2;
        float centerZ = (mapHeight - 1) * tileSpacingZ / 2;

        // 카메라 위치 설정
        Vector3 newPosition = new Vector3(
            centerX,
            height,
            centerZ + 5
        );

        // 카메라 사이즈 설정: 가로 칸당 x5 + 2
        float newSize = (mapWidth * sizeMultiplier) + sizeOffset - 10;

        // 설정 적용
        miniMapCamera.transform.position = newPosition;
        miniMapCamera.orthographicSize = newSize;

        Debug.Log($"MiniMap Camera adjusted: Position = {newPosition}, Size = {newSize}");
    }
}
