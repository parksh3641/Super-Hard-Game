using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using System;
using System.Collections;


public class StageDataLoader : MonoBehaviour
{
    public string stageFilePath = "Assets/StreamingAssets/Stage/Stage1Data.bin";

    public GameObject lastLine;

    public GameObject[] tilePrefabs;
    public GameObject startZoneObject;
    public Vector3 tileSpacing = new Vector3(10, 0, 10);

    public List<GameObject> generatedTiles = new List<GameObject>();

    public MiniMapController miniMapController;
    public UFOController ufoControllerPrefab;

    private MapController[,] mapControllers;
    private List<UFOPath> ufoPaths = new List<UFOPath>();

    List<string[]> mapData = new List<string[]>();
    List<string[]> laserData = new List<string[]>();
    List<string[]> ufoData = new List<string[]>();
    List<string[]> ufo2Data = new List<string[]>(); // UFO2 데이터 리스트 추가
    List<string[]> ufo3Data = new List<string[]>(); // UFO3 데이터 리스트 추가
    List<string[]> safeZoneData = new List<string[]>(); // 세이프티존 리스트 추가

    bool isReadingMap = false;
    bool isReadingLaser = false;
    bool isReadingUFO = false;
    bool isReadingUFO2 = false; // UFO2 데이터 읽기 여부 추가
    bool isReadingUFO3 = false; // UFO3 데이터 읽기 여부 추가
    bool isSafeZone = false;

    PlayerDataBase playerDataBase;

    private void Start()
    {
        if (playerDataBase == null) playerDataBase = Resources.Load("PlayerDataBase") as PlayerDataBase;


        StartCoroutine(LoadStageData());
    }

    IEnumerator LoadStageData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Stage", "Stage" + (PlayerPrefs.GetInt("Stage") + 1) + "Data.bin");

        string decryptedCSV = DecryptStageCSV.DecryptFile(filePath);

        if (string.IsNullOrEmpty(decryptedCSV))
        {
            Debug.LogError("복호화된 데이터가 없습니다!");
            yield break;
        }

        yield return null; // 한 프레임 대기 (데이터 로드 보장)

        string[] lines = decryptedCSV.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            Debug.Log(line); // 복호화된 CSV 데이터 출력
        }

        Vector3 startZonePosition = Vector3.zero;

        foreach (string line in lines)
        {
            if (line.StartsWith("#"))
            {
                // 🔹 모든 상태를 false로 초기화 (각 데이터 블록 시작 시 한 번만 적용)
                isReadingMap = false;
                isReadingLaser = false;
                isReadingUFO = false;
                isReadingUFO2 = false;
                isReadingUFO3 = false;
                isSafeZone = false;

                // 🔹 새로운 데이터 블록 감지
                if (line.Contains("MapData")) isReadingMap = true;
                if (line.Contains("LaserData")) isReadingLaser = true;
                if (line.Contains("UFOData") && !line.Contains("UFOData2") && !line.Contains("UFOData3")) isReadingUFO = true;
                if (line.Contains("UFOData2")) isReadingUFO2 = true;
                if (line.Contains("UFOData3")) isReadingUFO3 = true;
                if (line.Contains("SafeZone")) isSafeZone = true;

                Debug.Log($"🔹 데이터 블록 변경: Map({isReadingMap}), Laser({isReadingLaser}), UFO1({isReadingUFO}), UFO2({isReadingUFO2}), UFO3({isReadingUFO3}), SafeZone({isSafeZone})");
                continue;
            }

            if (isReadingMap)
            {
                mapData.Add(line.Split(','));
            }
            else if (isReadingLaser)
            {
                laserData.Add(line.Split(','));
            }
            else if (isReadingUFO)
            {
                ufoData.Add(line.Split(','));
            }
            else if (isReadingUFO2) // UFO2 데이터 추가
            {
                ufo2Data.Add(line.Split(','));
            }
            else if (isReadingUFO3) // UFO3 데이터 추가
            {
                ufo3Data.Add(line.Split(','));
            }
            else if (isSafeZone) // UFO3 데이터 추가
            {
                safeZoneData.Add(line.Split(','));
            }
        }

        //Debug.Log($"Map data loaded with {mapData.Count} rows.");
        //Debug.Log($"Laser data loaded with {laserData.Count} rows.");
        //Debug.Log($"UFO data loaded with {ufoData.Count} rows.");

        int mapWidth = mapData[0].Length;
        int mapHeight = mapData.Count;

        mapControllers = new MapController[mapHeight, mapWidth];

        int mapIndex = 0;

        // 🟢 MapData 생성
        for (int y = 0; y < mapHeight - 1; y++)
        {
            for (int x = 0; x < mapData[y].Length; x++)
            {
                int tileType = int.Parse(mapData[y][x]);

                Vector3 position = new Vector3(
                    x * tileSpacing.x,
                    0,
                    -y * tileSpacing.z
                );

                if (tileType >= 0 && tileType < tilePrefabs.Length)
                {
                    GameObject tile = Instantiate(tilePrefabs[tileType], position, Quaternion.identity, transform);

                    if (tileType != 0 && tileType != 13 && tileType != 14)
                    {
                        generatedTiles.Add(tile);
                    }

                    // MapController 저장
                    MapController mapController = tile.GetComponent<MapController>();
                    if (mapController != null)
                    {
                        mapControllers[y, x] = mapController;
                    }
                }

                if (tileType == 1)
                {
                    startZonePosition = position;
                    Debug.Log($"Start Zone found at: {startZonePosition}");
                }
            }
        }

        // 🟢 **세이프티존 데이터 처리**
        for (int y = 0; y < safeZoneData.Count; y++)
        {
            for (int x = 0; x < safeZoneData[y].Length; x++)
            {
                if (int.TryParse(safeZoneData[y][x].Trim(), out int tileType))
                {
                    if (tileType == 1) // 세이프티존이 있는 위치 감지
                    {
                        Vector3 mapPosition = new Vector3(
                            x * tileSpacing.x,
                            0,
                            -y * tileSpacing.z
                        );

                        if (mapControllers[y, x] != null)
                        {
                            mapControllers[y, x].SetSafeZone(); // 맵 컨트롤러에 특정 값 설정
                            Debug.Log($"🟢 SafeZone Applied at: {mapPosition}");
                        }
                    }
                }
            }
        }

        // 🟠 **레이저 데이터 전달 (좌표 매칭)**
        for (int y = 0; y < laserData.Count; y++)
        {
            for (int x = 0; x < laserData[y].Length - 1; x++)
            {
                int laserType = int.Parse(laserData[y][x]);

                if (mapControllers[y, x] != null)
                {
                    mapControllers[y, x].SetLaser(laserType);
                }
            }
        }

        Vector3 startPoint = Vector3.zero, startPoint2 = Vector3.zero, startPoint3 = Vector3.zero;
        Vector3 endPoint = Vector3.zero, endPoint2 = Vector3.zero, endPoint3 = Vector3.zero;
        bool startFound = false, startFound2 = false, startFound3 = false;
        bool endFound = false, endFound2 = false, endFound3 = false;

        if (ufoData.Count > 0 && ufoData[0].Length >= 2)
        {
            if (int.TryParse(ufoData[0][0].Trim(), out int setting1) &&
                int.TryParse(ufoData[0][1].Trim(), out int setting2))
            {
                ufoControllerPrefab.SetUFOSettings(setting1, setting2);
            }
        }

        if (ufo2Data.Count > 0 && ufo2Data[0].Length >= 2)
        {
            if (int.TryParse(ufo2Data[0][0].Trim(), out int setting1_2) &&
                int.TryParse(ufo2Data[0][1].Trim(), out int setting2_2))
            {
                ufoControllerPrefab.SetUFOSettings2(setting1_2, setting2_2);
                Debug.Log($"✅ UFO2 Settings Applied: ({setting1_2}, {setting2_2})");
            }
        }

        if (ufo3Data.Count > 0 && ufo3Data[0].Length >= 2)
        {
            if (int.TryParse(ufo3Data[0][0].Trim(), out int setting1_3) &&
                int.TryParse(ufo3Data[0][1].Trim(), out int setting2_3))
            {
                ufoControllerPrefab.SetUFOSettings3(setting1_3, setting2_3);
                Debug.Log($"✅ UFO3 Settings Applied: ({setting1_3}, {setting2_3})");
            }
        }

        for (int y = 1; y < ufoData.Count; y++) // 첫 줄(0,0)은 제외
        {
            for (int x = 0; x < ufoData[y].Length; x++)
            {
                if (string.IsNullOrWhiteSpace(ufoData[y][x])) continue;

                if (int.TryParse(ufoData[y][x].Trim(), out int tileType))
                {
                    // 🛠️ 좌표 변환 (Z축 수정)
                    Vector3 mapPosition = new Vector3(
                        x * tileSpacing.x,   // X축: 오른쪽으로 10씩 증가
                        0,                   // Y축: 고정
                        -(y - 1) * tileSpacing.z // Z축: 한 칸 조정 (y - 1)
                    );

                    // 🟢 1(시작점) 감지
                    if (tileType == 1 && !startFound)
                    {
                        startPoint = mapPosition;
                        startFound = true;
                        Debug.Log($"✅ UFO Start Point Found: {startPoint}");
                    }

                    // 🟠 2(도착점) 감지
                    if (tileType == 2 && !endFound)
                    {
                        endPoint = mapPosition;
                        endFound = true;
                        Debug.Log($"✅ UFO End Point Found: {endPoint}");
                    }
                }
            }
        }

        for (int y = 1; y < ufo2Data.Count; y++) // UFO2 데이터도 첫 줄(0,0) 제외
        {
            for (int x = 0; x < ufo2Data[y].Length; x++)
            {
                if (string.IsNullOrWhiteSpace(ufo2Data[y][x])) continue; // 빈 값 스킵

                if (int.TryParse(ufo2Data[y][x].Trim(), out int tileType))
                {
                    Vector3 mapPosition = new Vector3(
                        x * tileSpacing.x,
                        0,
                        -(y - 1) * tileSpacing.z
                    );

                    if (tileType == 1 && !startFound2)
                    {
                        startPoint2 = mapPosition;
                        startFound2 = true;
                        Debug.Log($"✅ UFO2 Start Point Found: {startPoint2}");
                    }

                    if (tileType == 2 && !endFound2)
                    {
                        endPoint2 = mapPosition;
                        endFound2 = true;
                        Debug.Log($"✅ UFO2 End Point Found: {endPoint2}");
                    }
                }
            }
        }

        for (int y = 1; y < ufo3Data.Count; y++) // UFO3 데이터도 첫 줄(0,0) 제외
        {
            for (int x = 0; x < ufo3Data[y].Length; x++)
            {
                if (string.IsNullOrWhiteSpace(ufo3Data[y][x])) continue; // 빈 값 스킵

                if (int.TryParse(ufo3Data[y][x].Trim(), out int tileType))
                {
                    Vector3 mapPosition = new Vector3(
                        x * tileSpacing.x,
                        0,
                        -(y - 1) * tileSpacing.z
                    );

                    if (tileType == 1 && !startFound3)
                    {
                        startPoint3 = mapPosition;
                        startFound3 = true;
                        Debug.Log($"✅ UFO3 Start Point Found: {startPoint3}");
                    }

                    if (tileType == 2 && !endFound3)
                    {
                        endPoint3 = mapPosition;
                        endFound3 = true;
                        Debug.Log($"✅ UFO3 End Point Found: {endPoint3}");
                    }
                }
            }
        }

        if (startFound && endFound)
        {
            ufoControllerPrefab.InitializePath1(startPoint, endPoint);
            Debug.Log($"🚀 UFO Path Initialized: Start({startPoint}) → End({endPoint})");
        }
        else
        {
            Debug.LogWarning("⚠️ UFO1 Start or End Point Not Found!");
        }

        if (startFound2 && endFound2)
        {
            ufoControllerPrefab.InitializePath2(startPoint2, endPoint2);
            Debug.Log($"🚀 UFO2 Path Initialized: Start({startPoint2}) → End({endPoint2})");
        }
        else
        {
            Debug.LogWarning("⚠️ UFO2 Start or End Point Not Found!");
        }

        if (startFound3 && endFound3)
        {
            ufoControllerPrefab.InitializePath3(startPoint3, endPoint3);
            Debug.Log($"🚀 UFO3 Path Initialized: Start({startPoint3}) → End({endPoint3})");
        }
        else
        {
            Debug.LogWarning("⚠️ UFO3 Start or End Point Not Found!");
        }

        // 🟡 맵 테두리 생성
        CreateMapBorders(mapWidth, mapHeight - 1);

        // 🟢 **Start Zone Object 이동**
        if (startZoneObject != null)
        {
            startZoneObject.transform.position = startZonePosition;
            Debug.Log($"Start Zone Object moved to: {startZonePosition}");
        }
        else
        {
            Debug.LogWarning("Start Zone Object is not assigned.");
        }

        // 🟣 **미니맵 카메라 조정**
        if (miniMapController != null)
        {
            miniMapController.AdjustCamera(mapWidth, mapHeight);
        }
        else
        {
            Debug.LogWarning("MiniMapController is not assigned.");
        }

        SortTilesByStartZone();
    }

    void SortTilesByStartZone()
    {
        if (startZoneObject == null)
        {
            Debug.LogError("🚨 StartZone이 설정되지 않았습니다!");
            return;
        }

        // 시작 위치
        Vector3 startPosition = startZoneObject.transform.position;

        // 방문한 타일을 추적하기 위한 HashSet
        HashSet<Vector3> visitedPositions = new HashSet<Vector3>();

        // 정렬된 결과를 저장할 새 리스트
        List<GameObject> sortedTiles = new List<GameObject>();

        // 시작 포인트에서 가장 가까운 타일 찾기
        GameObject closestTile = null;
        float minDistance = float.MaxValue;

        foreach (var tile in generatedTiles)
        {
            float distance = Vector3.Distance(startPosition, tile.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestTile = tile;
            }
        }

        if (closestTile == null)
        {
            Debug.LogError("🚨 시작점 근처에 타일이 없습니다!");
            return;
        }

        // BFS 탐색을 위한 큐
        Queue<GameObject> queue = new Queue<GameObject>();
        queue.Enqueue(closestTile);
        visitedPositions.Add(closestTile.transform.position);

        // BFS 탐색으로 연결된 타일 찾기
        while (queue.Count > 0)
        {
            GameObject currentTile = queue.Dequeue();
            sortedTiles.Add(currentTile);

            // 현재 타일에서 연결된 이웃 타일들 찾기 (상하좌우)
            List<GameObject> neighbors = FindConnectedTiles(currentTile, visitedPositions);

            foreach (var neighbor in neighbors)
            {
                visitedPositions.Add(neighbor.transform.position);
                queue.Enqueue(neighbor);
            }
        }

        // 연결되지 않은 나머지 타일들도 추가 (필요한 경우)
        foreach (var tile in generatedTiles)
        {
            if (!sortedTiles.Contains(tile))
            {
                sortedTiles.Add(tile);
            }
        }

        // 정렬된 리스트로 generatedTiles 업데이트
        generatedTiles = sortedTiles;

        // 정렬된 리스트를 기반으로 인덱스 다시 설정
        for (int i = 0; i < generatedTiles.Count; i++)
        {
            MapController mapController = generatedTiles[i].GetComponent<MapController>();
            if (mapController != null)
            {
                mapController.SetIndex(i); // 새로운 인덱스 설정
            }
        }

        Debug.Log($"✅ 타일맵 정렬 완료! (연결된 경로 순서로 정렬됨)");
    }

    // 현재 타일에 연결된 타일 찾기 (상하좌우 방향)
    List<GameObject> FindConnectedTiles(GameObject currentTile, HashSet<Vector3> visitedPositions)
    {
        List<GameObject> connectedTiles = new List<GameObject>();
        Vector3 currentPos = currentTile.transform.position;

        // 상하좌우 방향 벡터
        Vector3[] directions = new Vector3[]
        {
        new Vector3(tileSpacing.x, 0, 0),    // 오른쪽
        new Vector3(-tileSpacing.x, 0, 0),   // 왼쪽
        new Vector3(0, 0, tileSpacing.z),    // 위
        new Vector3(0, 0, -tileSpacing.z)    // 아래
        };

        foreach (var direction in directions)
        {
            Vector3 neighborPos = currentPos + direction;

            // 이미 방문한 위치면 건너뛰기
            if (visitedPositions.Contains(neighborPos))
                continue;

            // 해당 위치에 타일이 있는지 확인
            foreach (var tile in generatedTiles)
            {
                // 위치가 거의 같은지 확인 (부동소수점 오차 고려)
                if (Vector3.Distance(tile.transform.position, neighborPos) < 0.1f)
                {
                    connectedTiles.Add(tile);
                    break;
                }
            }
        }

        return connectedTiles;
    }


    void CreateMapBorders(int width, int height)
    {
        for (int x = -1; x <= width; x++)
        {
            Instantiate(lastLine, new Vector3(x * tileSpacing.x, 0, 1 * tileSpacing.z), Quaternion.identity, transform);
            Instantiate(lastLine, new Vector3(x * tileSpacing.x, 0, -height * tileSpacing.z), Quaternion.identity, transform);
        }

        for (int y = 0; y < height; y++)
        {
            Instantiate(lastLine, new Vector3(-1 * tileSpacing.x, 0, -y * tileSpacing.z), Quaternion.identity, transform);
            Instantiate(lastLine, new Vector3(width * tileSpacing.x, 0, -y * tileSpacing.z), Quaternion.identity, transform);
        }
    }

    private class UFOPath
    {
        public Vector3 startPoint;
        public Vector3 endPoint;
        public UFOPath(Vector3 start, Vector3 end) => (startPoint, endPoint) = (start, end);
    }
}
