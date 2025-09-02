using Stove.PCSDK.NET;
using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StovePCSDKManager : MonoBehaviour
{
    public static StovePCSDKManager instance;

    // Setting value filled through 'LoadConfig'
    public string Env;
    public string AppKey;
    public string AppSecret;
    public string GameId;
    public StovePCLogLevel LogLevel;
    public string LogPath;
    
    private StovePCCallback callback;
    private Coroutine runcallbackCoroutine;

    StovePCResult sdkResult;

    StovePCConfig config;

    public LocalizationContent statusText;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor) return;

        string configFilePath = Application.streamingAssetsPath + "/Text/StovePCConfig.Unity.txt";

        this.callback = new StovePCCallback
        {
            OnError = new StovePCErrorDelegate(this.OnError),
            OnInitializationComplete = new StovePCInitializationCompleteDelegate(this.OnInitializationComplete),
            OnToken = new StovePCTokenDelegate(this.OnToken),
            OnUser = new StovePCUserDelegate(this.OnUser),
            OnOwnership = new StovePCOwnershipDelegate(this.OnOwnership),
        };

        if (File.Exists(configFilePath))
        {
            string configText = File.ReadAllText(configFilePath);
            config = JsonUtility.FromJson<StovePCConfig>(configText);

            Env = config.Env;
            AppKey = config.AppKey;
            AppSecret = config.AppSecret;
            GameId = config.GameId;
            LogLevel = config.LogLevel;
            LogPath = config.LogPath;
        }

        sdkResult = StovePC.Initialize(config, callback);

        statusText.localizationName = "AuthCheck";
        statusText.ReLoad();

        if (StovePCResult.NoError == sdkResult)
        {
            statusText.localizationName = "AuthSuccess";
            statusText.ReLoad();

            runcallbackCoroutine = StartCoroutine(RunCallback(2f));
        }
        else
        {
            statusText.localizationName = "AuthFail";
            statusText.ReLoad();

            SplashManager.instance.UserAuthenticationFailed();

            Debug.Log("�ʱ�ȭ ���з� ���� ����");
        }
    }

    private void OnDestroy()
    {
        if (runcallbackCoroutine != null)
        {
            StopCoroutine(runcallbackCoroutine);
            runcallbackCoroutine = null;
        }

        StovePCResult result = StovePC.Uninitialize();
        if (result == StovePCResult.NoError)
        {
            // ���� ó��
        }
    }

    #region Event Handlers
    public void ButtonLoadConfig_Click()
    {
        string configFilePath = Application.streamingAssetsPath + "/Text/StovePCConfig.Unity.txt";

        if (File.Exists(configFilePath))
        {
            string configText = File.ReadAllText(configFilePath);
            StovePCConfig config = JsonUtility.FromJson<StovePCConfig>(configText);

            this.Env = config.Env;
            this.AppKey = config.AppKey;
            this.AppSecret = config.AppSecret;
            this.GameId = config.GameId;
            this.LogLevel = config.LogLevel;
            this.LogPath = config.LogPath;

            WriteLog(configText);
        }
        else
        {
            string msg = String.Format("File not found : {0}", configFilePath);
            WriteLog(msg);
        }
    }

    public void ToggleRunCallback_ValueChanged(bool isOn)
    {
        if (isOn)
        {
            float intervalSeconds = 1f;
            runcallbackCoroutine = StartCoroutine(RunCallback(intervalSeconds));

            WriteLog("RunCallback Start");
        }
        else
        {
            if (runcallbackCoroutine != null)
            {
                StopCoroutine(runcallbackCoroutine);
                runcallbackCoroutine = null;

                WriteLog("RunCallback Stop");
            }
        }
    }
    #endregion


    #region Coroutine
    private IEnumerator RunCallback(float intervalSeconds)
    {
        WaitForSeconds wfs = new WaitForSeconds(intervalSeconds);
        while (true)
        {
            StovePC.RunCallback();
            yield return wfs;
        }
    }
    #endregion


    #region Methods

    private void WriteLog(string functionName, StovePCResult result)
    {
        if (String.IsNullOrEmpty(functionName))
            functionName = "Unknown";

        string msg = String.Format("{0} Success", functionName);
        if (result != StovePCResult.NoError)
        {
            msg = String.Format("{0} Fail : {1}", functionName, result.ToString());
        }

        Debug.Log(msg + Environment.NewLine);

        AppendUILog(msg);
    }
    private void WriteLog(string log)
    {
        Debug.Log(log + Environment.NewLine);

        AppendUILog(log);
    }

    private void AppendUILog(string log)
    {
        GameObject content = GameObject.Find("ContentLog");
        GameObject textLog = content.transform.GetChild(0).gameObject;
        GameObject copyLog = Instantiate<GameObject>(textLog, content.transform);
        var copyTextComponent = copyLog.GetComponent<Text>();
        copyTextComponent.text = log;
    }

    private void ToggleRunCallback(bool isOn)
    {
        GameObject toggleRunCallback = GameObject.Find("ToggleRunCallback");
        Toggle toggleComponent = toggleRunCallback.GetComponent<Toggle>();
        toggleComponent.isOn = isOn;
    }
    #endregion


    #region SDK Callback Methods
    private void OnError(StovePCError error)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("OnError");
        sb.AppendFormat(" - error.FunctionType : {0}" + Environment.NewLine, error.FunctionType.ToString());
        sb.AppendFormat(" - error.Result : {0}" + Environment.NewLine, error.Result.ToString());
        sb.AppendFormat(" - error.Message : {0}" + Environment.NewLine, error.Message);
        sb.AppendFormat(" - error.ExternalError : {0}", error.ExternalError.ToString());

        switch (error.FunctionType)
        {
            case StovePCFunctionType.Initialize:
            case StovePCFunctionType.GetUser:
            case StovePCFunctionType.GetOwnership:
                Debug.Log("OnError �߻�");
                SplashManager.instance.UserAuthenticationFailed();
                break;
        }

        WriteLog(sb.ToString());
    }
    #endregion


    #region SDK Initialization
    public void ButtonInitialize_Click()
    {
        StovePCResult sdkResult = StovePCResult.NoError;

        // Todo: Write your code here.

        WriteLog("Initialize", sdkResult);
    }

    private void OnInitializationComplete()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("OnInitializationComplete");
        sb.AppendFormat(" - nothing");

        sdkResult = StovePC.GetUser();

        if (StovePCResult.NoError == sdkResult)
        {
            Debug.Log("�α��� ����");
        }
        else
        {
            Debug.Log("�α��� ����");
        }

        WriteLog(sb.ToString());
    }

    #endregion


    #region SDK Termination
    public void ButtonUninitialize_Click()
    {
        ToggleRunCallback(false);

        StovePCResult sdkResult = StovePCResult.NoError;

        // Todo: Write your code here.

        WriteLog("Uninitialize", sdkResult);
    }
    #endregion


    #region Acquiring User Information
    public void ButtonGetUser_Click()
    {
        StovePCResult sdkResult = StovePCResult.NoError;

        // Todo: Write your code here.

        WriteLog("GetUser", sdkResult);
    }

    private void OnUser(StovePCUser user)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("OnUser");
        sb.AppendFormat(" - user.MemberNo : {0}" + Environment.NewLine, user.MemberNo.ToString());
        sb.AppendFormat(" - user.Nickname : {0}" + Environment.NewLine, user.Nickname);
        sb.AppendFormat(" - user.GameUserId : {0}", user.GameUserId);

        statusText.localizationName = "LoginToServer";
        statusText.ReLoad();

        GameStateManager.instance.StoveId = user.MemberNo.ToString();
        PlayfabManager.instance.OnClickPlayfabLogin();

        WriteLog(sb.ToString());
    }
    #endregion


    #region Acquiring Token Information
    public void ButtonGetToken_Click()
    {
        StovePCResult sdkResult = StovePCResult.NoError;

        // Todo: Write your code here.

        WriteLog("GetToken", sdkResult);
    }

    private void OnToken(StovePCToken token)
    {
        StringBuilder sb = new StringBuilder();

        // Todo: Write your code here.

        WriteLog(sb.ToString());
    }
    #endregion

    private void OnOwnership(StovePCOwnership[] ownerships)
    {
        bool owned = false;

        foreach (var ownership in ownerships)
        {
            // [LOGIN_USER_MEMBER_NO] StovePCUser ����ü�� MemberNo
            // [OwnershipCode] 1: ������ ȹ��, 2: ������ ����(���� ����� ���)
            if (ownership.OwnershipCode != 1)
            {
                continue;
            }

            // [GameCode] 3: BASIC ����, 5: DLC
            if (ownership.GameId == "GLZD_IND_DEMO_01_IND" &&
                ownership.GameCode == 3)
            {
                owned = true; // ������ Ȯ�� ���� true�� ����
            }
        }

        if (owned)
        {
            // ������ ������ ���������� �Ϸ� �� ���� �������� ���� �ۼ�
        }
        else
        {
            Debug.Log("�������� �����ϴ�");

            SplashManager.instance.UserAuthenticationFailed();
            // ������ �������� �� ������ �����ϰ� �����޼��� ǥ�� ���� �ۼ�
        }
    }
}
