using UnityEngine;
using UnityEngine.UI;

public class MinimapPlayerIcon : MonoBehaviour
{
    public Transform player;             // �÷��̾��� Transform
    public Camera minimapCamera;         // Orthographic �̴ϸ� ī�޶�
    public RectTransform playerIcon;     // ������ �÷��̾� ������ (UI Image)
    public RectTransform minimapPanel;   // �̴ϸ� UI�� RectTransform (RawImage ����)

    void Update()
    {
        UpdatePlayerIconPosition();
    }

    private void UpdatePlayerIconPosition()
    {
        // �÷��̾��� ���� ��ǥ�� �̴ϸ� ī�޶��� ����Ʈ ��ǥ�� ��ȯ
        Vector3 viewportPosition = minimapCamera.WorldToViewportPoint(player.position);

        // ����Ʈ ��ǥ (0~1)�� �̴ϸ� UI ��ǥ�� ��ȯ
        float x = (viewportPosition.x - 0.5f) * minimapPanel.rect.width;
        float y = (viewportPosition.y - 0.5f) * minimapPanel.rect.height;

        // ������ ��ġ ���� (�̴ϸ��� �߽��� �������� �̵�)
        playerIcon.anchoredPosition = new Vector2(x, y);
    }
}
