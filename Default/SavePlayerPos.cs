using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SavePlaysePos : MonoBehaviour
{
    public GameObject player;

    private List<Vector2> playerPositions = new List<Vector2>(); // �÷��̾� ��ġ ��� ����Ʈ

    WaitForSeconds waitForSeconds = new WaitForSeconds(1);

    Vector2 currentPosition;

    public void StartSavePos()
    {
        StartCoroutine(SavePlayerPositionCoroutine());

        Debug.Log("�÷��̾� ��ġ �� ���� ����");
    }

    private IEnumerator SavePlayerPositionCoroutine()
    {
        currentPosition = new Vector2(player.transform.position.x, player.transform.position.z);
        playerPositions.Add(currentPosition); // ��ġ ����
        yield return waitForSeconds; // 1�� ���
        StartCoroutine(SavePlayerPositionCoroutine());
    }

    // ���� ���� �� CSV ���Ϸ� ������ ����
    public void EndSavePlayerPos(int number)
    {
        // �⺻ ���: StreamingAssets/PlayerMoveData
        string baseFolderPath = Path.Combine(Application.streamingAssetsPath, "PlayerMoveData");
        if (!Directory.Exists(baseFolderPath))
        {
            Directory.CreateDirectory(baseFolderPath); // PlayerMoveData ������ ������ ����
        }

        // Stage<number> ���� ����
        string stageFolderName = $"Stage{number + 1}";
        string stageFolderPath = Path.Combine(baseFolderPath, stageFolderName);
        if (!Directory.Exists(stageFolderPath))
        {
            Directory.CreateDirectory(stageFolderPath); // Stage<number> ������ ������ ����
        }

        // ��¥ ��� CSV ���� �̸� ����
        string fileName = $"PlayerMoveData_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv"; // ��¥ ���� ���ϸ�
        string filePath = Path.Combine(stageFolderPath, fileName);

        // CSV ���� ����
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("X,Z"); // CSV ���
            foreach (Vector2 position in playerPositions)
            {
                writer.WriteLine($"{position.x},{position.y}");
            }
        }

        Debug.Log($"�÷��̾� ��ġ �� ���� �Ϸ� : {filePath}");
    }
}
