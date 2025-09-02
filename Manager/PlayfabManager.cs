using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using PlayFab.ProfilesModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

using EntityKey = PlayFab.ProfilesModels.EntityKey;
using System.Text;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayfabManager : MonoBehaviour
{
    public static PlayfabManager instance;

    string customId = "";

    public bool isActive = false;
    public bool isLogin = false;

    private bool playerData = false;
    private bool statisticsData = false;
    private bool inventoryData = false;
    private bool grantItemData = false;
    private bool waitGrantItem = false;

    List<string> itemList = new List<string>();

    [Header("Entity")]
    private string entityId;
    private string entityType;
    private readonly Dictionary<string, string> entityFileJson = new Dictionary<string, string>();

    private List<ItemInstance> inventoryList = new List<ItemInstance>();

    public LocalizationContent statusText;

    private Action action;

    WaitForSeconds waitForSeconds = new WaitForSeconds(2f);

    PlayerDataBase playerDataBase;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 이동 후에도 유지

            if (playerDataBase == null) playerDataBase = Resources.Load("PlayerDataBase") as PlayerDataBase;

            playerDataBase.Initialize();
        }
        else
        {
            Destroy(gameObject); // 기존 인스턴스가 있으면 새 객체 파괴
        }

        isActive = false;
        isLogin = false;

        //CheckLogin();
    }

    public void CheckLogin()
    {
        if (GameStateManager.instance.AutoLogin)
        {
            switch (GameStateManager.instance.Login)
            {
                case LoginType.None:
                    break;
                case LoginType.Guest:
                    OnClickPlayfabLogin();
                    break;
            }
        }
    }

    public void LogOut()
    {
        PlayFabClientAPI.ForgetAllCredentials();

        SuccessLogOut();
    }

    public void SuccessLogOut()
    {
        GameStateManager.instance.Initialize();

        isActive = false;
        isLogin = false;

        playerDataBase.Initialize();

        Debug.Log("Logout");

        SceneManager.LoadScene("MainScene");
    }


    #region Message
    private void SetEditorOnlyMessage(string message, bool error = false)
    {
#if UNITY_EDITOR || UNITY_EDITOR_OSX
        if (error) Debug.LogError("<color=red>" + message + "</color>");
        //else Debug.Log(message);
#endif
    }
    private void DisplayPlayfabError(PlayFabError error)
    {
        SetEditorOnlyMessage("error : " + error.GenerateErrorReport(), true);
    }

    #endregion
    #region GuestLogin
    public void OnClickPlayfabLogin()
    {
        if (!NetworkConnect.instance.CheckConnectInternet())
        {
            SplashManager.instance.CheckInternetFailed();
            return;
        }

        if (isLogin) return;
        isLogin = true;

        statusText.localizationName = "LoginToServerSuccess";
        statusText.ReLoad();

        customId = GameStateManager.instance.CustomId;

        if (GameStateManager.instance.StoveId.Length > 0)
        {
            if (string.IsNullOrEmpty(customId))
                CreateStoveId();
            else
                LoginGuestId();
        }
        else
        {
            if (string.IsNullOrEmpty(customId))
                CreateGuestId();
            else
                LoginGuestId();
        }
    }

    private void CreateGuestId()
    {
        Debug.Log("New PlayfabId : Guest");

        customId = GetRandomPassword(16);

        GameStateManager.instance.CustomId = customId;

        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
        {
            CustomId = customId,
            CreateAccount = true
        }, result =>
        {
            GameStateManager.instance.AutoLogin = true;
            GameStateManager.instance.Login = LoginType.Guest;
            OnLoginSuccess(result);
        }, error =>
        {
            Debug.LogError("Login Fail - Guest");

            statusText.localizationName = "LoginFail";
            statusText.ReLoad();

            isLogin = false;

            Invoke("OnClickPlayfabLogin", 3f);
        });
    }

    private string GetRandomPassword(int _totLen)
    {
        string input = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var chars = Enumerable.Range(0, _totLen)
            .Select(x => input[UnityEngine.Random.Range(0, input.Length)]);
        return new string(chars.ToArray());
    }

    public void CreateStoveId()
    {
        Debug.Log("New PlayfabId : StoveId");

        statusText.localizationName = "FirstLogin";
        statusText.ReLoad();

        customId = GenerateEncryptedPassword(GameStateManager.instance.StoveId);

        GameStateManager.instance.CustomId = customId;

        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
        {
            CustomId = customId,
            CreateAccount = true
        }, result =>
        {
            GameStateManager.instance.AutoLogin = true;
            GameStateManager.instance.Login = LoginType.Guest;
            OnLoginSuccess(result);
        }, error =>
        {
            Debug.LogError("Login Fail - Guest");

            statusText.localizationName = "LoginFail";
            statusText.ReLoad();

            isLogin = false;

            Invoke("OnClickPlayfabLogin", 3f);
        });
    }

public string GenerateEncryptedPassword(string inputNumber, int totalLength = 16)
{
    // 1. 입력값이 숫자로만 이루어졌는지 확인
    if (!inputNumber.All(char.IsDigit))
    {
        Debug.LogError("입력값은 숫자로만 이루어져야 합니다.");
        return string.Empty;
    }

    // 2. 고정된 해시 기반 랜덤 시드 생성
    int seed = GenerateDeterministicSeed(inputNumber);
    System.Random random = new System.Random(seed);

    // 3. 랜덤 문자열 생성
    string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    // 4. 입력값이 totalLength보다 길면 자르고, 짧으면 랜덤 문자 추가
    string baseString = inputNumber.Length > totalLength
        ? inputNumber.Substring(0, totalLength)
        : inputNumber;

    int remainingLength = totalLength - baseString.Length;

    string randomString = new string(
        Enumerable.Range(0, remainingLength)
            .Select(_ => characters[random.Next(characters.Length)])
            .ToArray()
    );

    // 5. 입력값과 랜덤 문자열 합치기
    char[] combinedArray = (baseString + randomString).ToCharArray();

    // 6. **Fisher-Yates Shuffle 적용 (완전히 결정론적)**
    for (int i = combinedArray.Length - 1; i > 0; i--)
    {
        int j = random.Next(i + 1);
        (combinedArray[i], combinedArray[j]) = (combinedArray[j], combinedArray[i]); // Swap
    }

    return new string(combinedArray);
}

// SHA256을 이용한 고정된 해시 기반 시드 생성
private int GenerateDeterministicSeed(string input)
{
    using (SHA256 sha256 = SHA256.Create())
    {
        byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        int seed = BitConverter.ToInt32(hash, 0); // 첫 4바이트를 정수 시드로 변환
        return seed;
    }
}


    private void LoginGuestId()
    {
        Debug.Log("Guest Login");

        statusText.localizationName = "Login";
        statusText.ReLoad();

        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
        {
            CustomId = customId,
            CreateAccount = false
        }, result =>
        {
            GameStateManager.instance.AutoLogin = true;
            GameStateManager.instance.Login = LoginType.Guest;
            OnLoginSuccess(result);
        }, error =>
        {
            Debug.LogError("Login Fail - Guest");

            statusText.localizationName = "LoginFail";
            statusText.ReLoad();

            isLogin = false;

            Invoke("OnClickPlayfabLogin", 3f);
        });
    }

    #endregion

    public void OnLoginSuccess(PlayFab.ClientModels.LoginResult result)
    {
        SetEditorOnlyMessage("Playfab Login Success");

        statusText.localizationName = "LoginSuccess";
        statusText.ReLoad();

        customId = result.PlayFabId;
        entityId = result.EntityToken.Entity.Id;
        entityType = result.EntityToken.Entity.Type;

        GameStateManager.instance.PlayfabId = result.PlayFabId;

        StartCoroutine(LoadDataCoroutine());

        //GetTitleInternalData("CheckAOSVersion", CheckVersion); //버전 체크
    }

    public void CheckVersion(bool check)
    {
        Debug.Log("Checking Version...");

        if (check)
        {
            //GetTitleInternalData("AOSVersion", CheckUpdate);
        }
        else
        {
            StartCoroutine(LoadDataCoroutine());
        }
    }


    public void CheckUpdate(bool check)
    {
        if (check)
        {
            StartCoroutine(LoadDataCoroutine());
        }
        else
        {
            //GameManager.instance.OnNeedUpdate();
        }
    }


    public void SetProfileLanguage(LanguageType type)
    {
        EntityKey entity = new EntityKey();
        entity.Id = entityId;
        entity.Type = entityType;

        var request = new SetProfileLanguageRequest
        {
            Language = type.ToString(),
            ExpectedVersion = 0,
            Entity = entity
        };
        PlayFabProfilesAPI.SetProfileLanguage(request, res =>
        {
            Debug.Log("The language on the entity's profile has been updated.");
        }, FailureCallback);
    }

    public void GetPlayerNickName()
    {
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
        {
            PlayFabId = customId,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true
            }
        },
        (result) =>
        {
            if(result.PlayerProfile.DisplayName != null)
            {
                GameStateManager.instance.NickName = result.PlayerProfile.DisplayName;
            }
            else
            {
                UpdateDisplayName(GameStateManager.instance.PlayfabId);
            }
        },
        DisplayPlayfabError);
    }

    void FailureCallback(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your API call. Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }

    private void OnCloudUpdateStats(ExecuteCloudScriptResult result)
    {
        SetEditorOnlyMessage(PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).SerializeObject(result.FunctionResult));
        JsonObject jsonResult = (JsonObject)result.FunctionResult;
        foreach (var json in jsonResult)
        {
            SetEditorOnlyMessage(json.Key + " / " + json.Value);
        }
        object messageValue;
        jsonResult.TryGetValue("OnCloudUpdateStats() messageValue", out messageValue);
        SetEditorOnlyMessage((string)messageValue);

        //GetUserInventory();
    }


    IEnumerator LoadDataCoroutine()
    {
        playerData = false;
        statisticsData = false;
        inventoryData = false;

        Debug.Log("Load Data...");

        GetPlayerNickName();

        //yield return new WaitForSeconds(0.5f);

        //yield return GetCatalog();

        GetPlayerData();

        while (!playerData)
        {
            yield return null;
        }

        GetStatistics();

        while (!statisticsData)
        {
            yield return null;
        }

        GetUserInventory();

        while (!inventoryData)
        {
            yield return null;
        }

        isActive = true;

        if (GameStateManager.instance.NickName.Length >= 15 && playerDataBase.NickName == 0)
        {
            NickNameManager.instance.OpenView(false);
        }
        else
        {
            SplashManager.instance.LoginSuccess();
        }
    }

    public void GetUserInventory()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), result =>
        {
            var Inventory = result.Inventory;
            if (Inventory != null)
            {
                for (int i = 0; i < Inventory.Count; i++)
                {
                    inventoryList.Add(Inventory[i]);
                }

                foreach (ItemInstance list in inventoryList)
                {

                }

            }

            inventoryData = true;

        }, DisplayPlayfabError);
    }

    public void GetStatistics()
    {
        PlayFabClientAPI.GetPlayerStatistics(
           new GetPlayerStatisticsRequest(),
           (Action<GetPlayerStatisticsResult>)((result) =>
           {
               foreach (var statistics in result.Statistics)
               {
                   switch (statistics.StatisticName)
                   {
                       //case "":
                       //    string text = statistics.Value.ToString();
                       //    break;
                       case "Stage":
                           playerDataBase.Stage = statistics.Value;
                           break;
                       case "TotalTimeAttack":
                           playerDataBase.TotalTimeAttack = statistics.Value;
                           break;
                       case "TimeAttackStage1":
                           playerDataBase.TimeAttackStage1 = statistics.Value;
                           break;
                       case "TimeAttackStage2":
                           playerDataBase.TimeAttackStage2 = statistics.Value;
                           break;
                       case "TimeAttackStage3":
                           playerDataBase.TimeAttackStage3 = statistics.Value;
                           break;
                       case "TimeAttackStage4":
                           playerDataBase.TimeAttackStage4 = statistics.Value;
                           break;
                       case "TimeAttackStage5":
                           playerDataBase.TimeAttackStage5 = statistics.Value;
                           break;
                       case "TimeAttackStage6":
                           playerDataBase.TimeAttackStage6 = statistics.Value;
                           break;
                       case "TimeAttackStage7":
                           playerDataBase.TimeAttackStage7 = statistics.Value;
                           break;
                       case "TimeAttackStage8":
                           playerDataBase.TimeAttackStage8 = statistics.Value;
                           break;
                       case "TimeAttackStage9":
                           playerDataBase.TimeAttackStage9 = statistics.Value;
                           break;
                       case "TimeAttackStage10":
                           playerDataBase.TimeAttackStage10 = statistics.Value;
                           break;
                       case "CountOutLoadDead":
                           playerDataBase.CountOutLoadDead = statistics.Value;
                           break;
                       case "CountMissileDead":
                           playerDataBase.CountMissileDead = statistics.Value;
                           break;
                       case "CountLaserDead":
                           playerDataBase.CountLaserDead = statistics.Value;
                           break;
                       case "CountUFODead":
                           playerDataBase.CountUFODead = statistics.Value;
                           break;
                       case "CountGameOver":
                           playerDataBase.CountGameOver = statistics.Value;
                           break;
                       case "CountUnknown":
                           playerDataBase.CountUnknown = statistics.Value;
                           break;
                       case "PlayTime":
                           playerDataBase.PlayTime = statistics.Value;
                           break;
                       case "NickName":
                           playerDataBase.NickName = statistics.Value;
                           break;
                       case "Developer":
                           playerDataBase.Developer = statistics.Value;
                           break;
                   }
               }

               statisticsData = true;
           })
           , (error) =>
           {

           });
    }

    public void SetPlayerData(Dictionary<string, string> data)
    {
        var request = new UpdateUserDataRequest() { Data = data, Permission = UserDataPermission.Public };
        try
        {
            PlayFabClientAPI.UpdateUserData(request, (result) =>
            {
                Debug.Log("Update Player Data!");

            }, DisplayPlayfabError);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public void GetPlayerData()
    {
        var request = new GetUserDataRequest() { PlayFabId = GameStateManager.instance.PlayfabId };
        PlayFabClientAPI.GetUserData(request, (result) =>
        {
            //ResetInfo resetInfo = new ResetInfo();

            foreach (var eachData in result.Data)
            {
                string key = eachData.Key;
                if (key.Contains("ResetInfo"))
                {
                    //resetInfo = JsonUtility.FromJson<ResetInfo>(eachData.Value.Value);
                    //playerDataBase.resetInfo = resetInfo;
                }
            }

            playerData = true;

        }, DisplayPlayfabError);
    }

    public void GetPlayerProfile(string playFabId, Action<string> action)
    {
        string countryCode = "";

        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
        {
            PlayFabId = playFabId,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowLocations = true
            }
        }, result =>
        {
            countryCode = result.PlayerProfile.Locations[0].CountryCode.Value.ToString();
            action?.Invoke(countryCode);

        }, error =>
        {
            action?.Invoke("");
        });
    }

    public void UpdatePlayerStatistics(List<StatisticUpdate> data)
    {
        if (!NetworkConnect.instance.CheckConnectInternet())
        {
            SplashManager.instance.CheckInternetFailed();
            return;
        }
        try
        {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "UpdatePlayerStatistics",
                FunctionParameter = new
                {
                    Statistics = data
                },
                GeneratePlayStreamEvent = true,
            }, OnCloudUpdateStats
            , DisplayPlayfabError);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
    public void UpdatePlayerStatisticsInsert(string name, int value)
    {
        if (!NetworkConnect.instance.CheckConnectInternet())
        {
            SplashManager.instance.CheckInternetFailed();
            return;
        }

        try
        {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "UpdatePlayerStatistics",
                FunctionParameter = new
                {
                    Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate {StatisticName = name, Value = value}
                }
                },
                GeneratePlayStreamEvent = true,
            },
            result =>
            {
                OnCloudUpdateStats(result);
            }
            , DisplayPlayfabError);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public void UpdateDisplayName(string nickname, Action successAction, Action failAction)
    {
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = nickname
        },
        result =>
        {
            Debug.Log("Update NickName : " + result.DisplayName);

            GameStateManager.instance.NickName = result.DisplayName;

            //GameManager.instance.Initialize();

            successAction?.Invoke();
        }
        , error =>
        {
            string report = error.GenerateErrorReport();
            if (report.Contains("Name not available"))
            {
                failAction?.Invoke();
            }
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    public void UpdateDisplayName(string nickname)
    {
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = nickname
        },
        result =>
        {
            Debug.Log("Update First NickName : " + result.DisplayName);

            GameStateManager.instance.NickName = result.DisplayName;
        }
        , error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    public void GetTitleInternalData(string name, Action<bool> action)
    {
        PlayFabServerAPI.GetTitleInternalData(new PlayFab.ServerModels.GetTitleDataRequest(),
            result =>
            {
                if (name.Equals("CheckAOSVersion"))
                {
                    if (result.Data[name].Equals("ON"))
                    {
                        action?.Invoke(true);
                    }
                    else
                    {
                        action?.Invoke(false);
                    }
                }
            },
            error =>
            {
                Debug.Log("Got error getting titleData:");
                Debug.Log(error.GenerateErrorReport());

                action?.Invoke(false);
            }
        );
    }

    public void GetTitleInternalData(string name, Action<string> action)
    {
        PlayFabServerAPI.GetTitleInternalData(new PlayFab.ServerModels.GetTitleDataRequest(),
            result =>
            {
                if (result.Data.ContainsKey(name))
                {
                    action.Invoke(result.Data[name]);
                }
            },
            error =>
            {
                Debug.Log("Got error getting titleData:");
                Debug.Log(error.GenerateErrorReport());
            }
        );
    }

    public void GetLeaderboarder(string name, int min, Action<GetLeaderboardResult> callBack)
    {
        var requestLeaderboard = new GetLeaderboardRequest
        {
            StartPosition = min,
            StatisticName = name,
            MaxResultsCount = 100,

            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowLocations = true,
                ShowDisplayName = true,
                ShowStatistics = true
            }
        };

        PlayFabClientAPI.GetLeaderboard(requestLeaderboard, callBack, DisplayPlayfabError);
    }

    public void GetLeaderboardMyRank(string name, Action<GetLeaderboardAroundPlayerResult> successCalback)
    {
        var request = new GetLeaderboardAroundPlayerRequest()
        {
            StatisticName = name,
            MaxResultsCount = 1,
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, successCalback, DisplayPlayfabError);
    }

    public void SetProfileLanguage(string language)
    {
        EntityKey entity = new EntityKey();
        entity.Id = entityId;
        entity.Type = entityType;

        var request = new SetProfileLanguageRequest
        {
            Language = language,
            ExpectedVersion = 0,
            Entity = entity
        };
        PlayFabProfilesAPI.SetProfileLanguage(request, res =>
        {
            Debug.Log("The language on the entity's profile has been updated.");
        }, FailureCallback);
    }

    public void GetServerTime(Action<DateTime> action)
    {
        if (!NetworkConnect.instance.CheckConnectInternet())
        {
            SplashManager.instance.CheckInternetFailed();
            return;
        }
        try
        {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "GetServerTime",
                GeneratePlayStreamEvent = true,
            }, result =>
            {
                string date = PlayFabSimpleJson.SerializeObject(result.FunctionResult);

                string year = date.Substring(1, 4);
                string month = date.Substring(6, 2);
                string day = date.Substring(9, 2);
                string hour = date.Substring(12, 2);
                string minute = date.Substring(15, 2);
                string second = date.Substring(18, 2);

                DateTime serverTime = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day), 0, 0, 0);

                serverTime = serverTime.AddDays(1);

                DateTime time = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day), int.Parse(hour), int.Parse(minute), int.Parse(second));

                TimeSpan span = serverTime - time;

                action?.Invoke(DateTime.Parse(span.ToString()));
            }, DisplayPlayfabError);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    //"2022-04-24T22:17:04.548Z"

    public void ReadTitleNews(Action<List<TitleNewsItem>> action)
    {
        List<TitleNewsItem> item = new List<TitleNewsItem>();

        PlayFabClientAPI.GetTitleNews(new GetTitleNewsRequest(), result =>
        {
            foreach (var list in result.News)
            {
                item.Add(list);
            }

            action.Invoke(item);

        }, error =>
        {

        });
    }

    public void ConsumeItem(string itemInstanceID)
    {
        try
        {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "ConsumeItem",
                FunctionParameter = new { ConsumeCount = 1, ItemInstanceId = itemInstanceID },
                GeneratePlayStreamEvent = true,
            }, OnCloudUpdateStats, DisplayPlayfabError);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public void GrantItemsToUser(string itemIds, string catalogVersion)
    {
        grantItemData = false;

        if (!waitGrantItem)
        {
            waitGrantItem = true;
            StartCoroutine(WaitGrantItemCoroution());
        }

        try
        {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "GrantItemsToUser",
                FunctionParameter = new { ItemIds = itemIds, CatalogVersion = catalogVersion },
                GeneratePlayStreamEvent = true,
            }, result =>
            {
                grantItemData = true;

            }, DisplayPlayfabError);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    IEnumerator WaitGrantItemCoroution()
    {
        while (!grantItemData)
        {
            yield return null;
        }

        GetUserInventory();

        waitGrantItem = false;
    }

    public void GrantItemToUser(string catalogversion, List<string> itemIds)
    {
        try
        {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "GrantItemToUser",
                FunctionParameter = new { CatalogVersion = catalogversion, ItemIds = itemIds },
                GeneratePlayStreamEvent = true,
            }, OnCloudUpdateStats, DisplayPlayfabError);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public void SetInventoryCustomData(string itemInstanceID, Dictionary<string, string> datas)
    {
        try
        {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "UpdateUserInventoryItemCustomData",
                FunctionParameter = new { Data = datas, ItemInstanceId = itemInstanceID },
                GeneratePlayStreamEvent = true,
            }, OnCloudUpdateStats, DisplayPlayfabError);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
}