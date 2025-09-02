using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SavePlaysePos : MonoBehaviour
{
    public GameObject player;

    private List<Vector2> playerPositions = new List<Vector2>(); // 플레이어 위치 기록 리스트

    WaitForSeconds waitForSeconds = new WaitForSeconds(1);

    Vector2 currentPosition;

    public void StartSavePos()
    {
        StartCoroutine(SavePlayerPositionCoroutine());

        Debug.Log("플레이어 위치 값 저장 시작");
    }

    private IEnumerator SavePlayerPositionCoroutine()
    {
        currentPosition = new Vector2(player.transform.position.x, player.transform.position.z);
        playerPositions.Add(currentPosition); // 위치 저장
        yield return waitForSeconds; // 1초 대기
        StartCoroutine(SavePlayerPositionCoroutine());
    }

    // 게임 종료 시 CSV 파일로 데이터 저장
    public void EndSavePlayerPos(int number)
    {
        // 기본 경로: StreamingAssets/PlayerMoveData
        string baseFolderPath = Path.Combine(Application.streamingAssetsPath, "PlayerMoveData");
        if (!Directory.Exists(baseFolderPath))
        {
            Directory.CreateDirectory(baseFolderPath); // PlayerMoveData 폴더가 없으면 생성
        }

        // Stage<number> 폴더 생성
        string stageFolderName = $"Stage{number + 1}";
        string stageFolderPath = Path.Combine(baseFolderPath, stageFolderName);
        if (!Directory.Exists(stageFolderPath))
        {
            Directory.CreateDirectory(stageFolderPath); // Stage<number> 폴더가 없으면 생성
        }

        // 날짜 기반 CSV 파일 이름 생성
        string fileName = $"PlayerMoveData_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv"; // 날짜 형식 파일명
        string filePath = Path.Combine(stageFolderPath, fileName);

        // CSV 파일 저장
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("X,Z"); // CSV 헤더
            foreach (Vector2 position in playerPositions)
            {
                writer.WriteLine($"{position.x},{position.y}");
            }
        }

        Debug.Log($"플레이어 위치 값 저장 완료 : {filePath}");
    }
}
