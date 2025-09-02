using UnityEngine;
using Steamworks;

public class SteamDebugManager : MonoBehaviour
{
    private void Start()
    {
        // Steam 초기화 상태 확인
        CheckSteamStatus();
    }

    private void CheckSteamStatus()
    {
        Debug.Log("=== Steam 상태 확인 ===");
        
        // Steam 클라이언트 실행 여부 확인
        if (!SteamAPI.IsSteamRunning())
        {
            Debug.LogError("Steam 클라이언트가 실행되지 않았습니다!");
            return;
        }
        else
        {
            Debug.Log("Steam 클라이언트가 실행 중입니다.");
        }

        // SteamManager 초기화 상태 확인
        if (SteamManager.Initialized)
        {
            Debug.Log("Steam API 초기화 성공!");
            
            // 현재 사용자 정보 출력
            CSteamID steamId = SteamUser.GetSteamID();
            string userName = SteamFriends.GetPersonaName();
            
            Debug.Log($"Steam ID: {steamId}");
            Debug.Log($"사용자명: {userName}");
            Debug.Log($"현재 App ID: {SteamUtils.GetAppID()}");
        }
        else
        {
            Debug.LogError("Steam API 초기화 실패!");
            
            // 가능한 원인들 체크
            CheckPossibleIssues();
        }
    }

    private void CheckPossibleIssues()
    {
        Debug.Log("=== 가능한 문제점들 ===");
        
        // steam_appid.txt 파일 존재 여부 확인
        string appIdPath = Application.dataPath + "/../steam_appid.txt";
        if (!System.IO.File.Exists(appIdPath))
        {
            Debug.LogError($"steam_appid.txt 파일이 없습니다! 경로: {appIdPath}");
            Debug.LogError("프로젝트 루트 폴더에 steam_appid.txt 파일을 생성하고 '480'을 입력하세요.");
        }
        else
        {
            string content = System.IO.File.ReadAllText(appIdPath).Trim();
            Debug.Log($"steam_appid.txt 내용: '{content}'");
            
            if (content != "480")
            {
                Debug.LogWarning("테스트용으로 App ID를 480 (SpaceWar)으로 설정하는 것을 권장합니다.");
            }
        }

        // Steam 라이브러리에 해당 게임이 있는지 확인
        Debug.Log("Steam 라이브러리에서 SpaceWar (App ID: 480)를 확인하세요.");
        Debug.Log("SpaceWar가 없다면 Steam 스토어에서 무료로 다운로드할 수 있습니다.");
    }

    // 버튼으로 수동 재초기화 테스트
    [ContextMenu("Steam 재초기화 테스트")]
    public void TestSteamReinitialization()
    {
        Debug.Log("Steam 재초기화 테스트 시작...");
        CheckSteamStatus();
    }
}