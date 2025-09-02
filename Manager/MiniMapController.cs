using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    public Camera miniMapCamera; // �̴ϸ� ī�޶� ����

    public GameObject miniMap;

    public float tileSpacingX = 10f; // Ÿ�� �� X�� ����
    public float tileSpacingZ = -10f; // Ÿ�� �� Z�� ����

    public float sizeMultiplier = 5f; // �� ĭ�� ������ ���� ����
    public float sizeOffset = 2f; // ������ �߰� ������

    public float height = 10f; // Y�� ���� ����

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

        // ���� �߾� ��ǥ ��� (Ÿ�� ������ ���)
        float centerX = (mapWidth - 1) * tileSpacingX / 2;
        float centerZ = (mapHeight - 1) * tileSpacingZ / 2;

        // ī�޶� ��ġ ����
        Vector3 newPosition = new Vector3(
            centerX,
            height,
            centerZ + 5
        );

        // ī�޶� ������ ����: ���� ĭ�� x5 + 2
        float newSize = (mapWidth * sizeMultiplier) + sizeOffset - 10;

        // ���� ����
        miniMapCamera.transform.position = newPosition;
        miniMapCamera.orthographicSize = newSize;

        Debug.Log($"MiniMap Camera adjusted: Position = {newPosition}, Size = {newSize}");
    }
}
