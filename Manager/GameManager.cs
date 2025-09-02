using PlayFab.EconomyModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Material[] skyBox;

    public TMP_Text timerText;
    public LocalizationContent stageText;

    public bool isTimer = false; // Ÿ�̸� ���� ����
    public bool isRunning = true;
    private float elapsedTime = 0f; // ��� �ð� (��)

    [Header("View")]
    public GameObject delayView;
    public GameObject gamePauseView;
    public GameObject gameOverView;
    public LocalizationContent gameOverProgressText;
    public Image gameOverImage;
    public Sprite[] gameOverSprite;

    public GameObject gameClearView;
    public TMP_Text gameClearView_TimerText;
    public GameObject gameClearView_BestTime;

    public GameObject gameDeathView;

    public GameObject nextStageButton;

    public GameObject checkInternet;

    public TMP_Text playerSpeedText;
    public TMP_Text playerAccelerationText;
    public TMP_Text playerRotationalSpeedText;
    public TMP_Text missileSpeedText;
    public TMP_Text playerSizeText;

    public GameObject invincibility;
    public GameObject miniMap;

    public GameObject developer;
    public TMP_Text developerText;

    public GameObject tutorial;

    [Header("Controller")]
    public PlayerController playerController;

    public List<MapController> mapControllerList;

    // Ű �Է� ��ٿ��� ���� ��ųʸ�
    private Dictionary<KeyCode, float> keyCooldowns = new Dictionary<KeyCode, float>();
    private float cooldownTime = 0.1f; // �� Ű �Է� ��ٿ� �ð�

    private bool isClick = false;

    private int nowStage = 0;

    private int timeAttack = 0;
    private int minutes;
    private int seconds;
    private int milliseconds;

    private int totalTimeAttack = 0;
    private int gameOverNumber = 0;

    PlayerDataBase playerDataBase;

    string stageFolderPath = Path.Combine(Application.streamingAssetsPath, "Stage");
    string searchPattern = "*.bin"; // ���� �̸� ����
    int maxStage = 0;

    float progress = 0;

    public StageDataLoader stageDataLoader;
    public SavePlaysePos savePlaysePos;

    WaitForSeconds waitForSeconds = new WaitForSeconds(60);

    public delegate void GameEventHandler();
    public static event GameEventHandler OnGameStart, OnGamePause, OnGameOver;


    private void Awake()
    {
        instance = this;

        Time.timeScale = 1;
        isRunning = true;

        if (playerDataBase == null) playerDataBase = Resources.Load("PlayerDataBase") as PlayerDataBase;

        delayView.SetActive(false);
        gamePauseView.SetActive(false);
        gameOverView.SetActive(false);
        gameClearView.SetActive(false);
        invincibility.SetActive(false);
        gameDeathView.SetActive(false);
        checkInternet.SetActive(false);

        KeyCode[] keys = {
            KeyCode.Escape, KeyCode.R, KeyCode.Q, KeyCode.E, KeyCode.U
            , KeyCode.N, KeyCode.P, KeyCode.M,
            KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4,
            KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8
            , KeyCode.Alpha9, KeyCode.Alpha0, KeyCode.T
        };

        foreach (KeyCode key in keys)
        {
            keyCooldowns[key] = -cooldownTime; // �ʱ� �ð� ����
        }

        maxStage = CountStageFiles(stageFolderPath, searchPattern);

        Debug.Log("�ְ� �������� : " + maxStage);

        nowStage = PlayerPrefs.GetInt("Stage");

        stageText.localizationName = "Stage";
        stageText.plusText = " " + (nowStage + 1).ToString();
        stageText.ReLoad();

        if (nowStage + 1 < 5)
        {
            RenderSettings.skybox = skyBox[0];
        }
        else
        {
            RenderSettings.skybox = skyBox[1];
        }

        miniMap.SetActive(false);
        developer.SetActive(false);
        developerText.text = "������ ���(T)";

        tutorial.SetActive(false);

        Invoke("Initialize", 0.2f);
    }

    int CountStageFiles(string folderPath, string pattern)
    {
        try
        {
            // �ش� �������� ���Ͽ� �´� ���� ��� �˻�
            string[] files = Directory.GetFiles(folderPath, pattern);
            return files.Length;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error counting files: {e.Message}");
            return 0;
        }
    }

    void Start()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.StopSFX();
            SoundManager.instance.PlayBGM(GameBGMType.Main);
            SoundManager.instance.PlaySFX(GameSfxType.GameStart);
        }

        StartCoroutine(PlayTimeCoroutine());

#if UNITY_EDITOR
        playerDataBase.Developer = 1;
#endif
    }

    IEnumerator PlayTimeCoroutine()
    {
        yield return waitForSeconds;
        playerDataBase.PlayTime += 1;
        PlayfabManager.instance.UpdatePlayerStatisticsInsert("PlayTime", playerDataBase.PlayTime);

        StartCoroutine(PlayTimeCoroutine());
    }

    void Initialize()
    {
        gameOverNumber = 0;
        GameStateManager.instance.IsPlaying = true;

        SetText();

        if (!GameStateManager.instance.Opening && PlayerPrefs.GetInt("Stage") == 0)
        {
            CutSceneManager.instance.OpenView(0);

            GameStateManager.instance.Opening = true;
            GameStateManager.instance.IsPlaying = false;
        }
        else
        {
            if(!GameStateManager.instance.First)
            {
                OpenTutorial();
            }
        }

        if (!GameStateManager.instance.Mid && PlayerPrefs.GetInt("Stage") == 4)
        {
            CutSceneManager.instance.OpenView(1);

            GameStateManager.instance.Mid = true;
            GameStateManager.instance.IsPlaying = false;
        }

        if(GameStateManager.instance.DeathNumber == 0)
        {
            if(UnityEngine.Random.Range(0,2) == 0)
            {
                GameStateManager.instance.DeathNumber = 1;
            }
            else
            {
                GameStateManager.instance.DeathNumber = 2;
            }
        }
    }

    public void OpenTutorial()
    {
        tutorial.SetActive(true);
    }

    public void CloseTutorial()
    {
        GameStateManager.instance.First = true;

        tutorial.SetActive(false);
    }

    public void Update()
    {
        //CheckKey(KeyCode.Escape, () => GamePause());
        if (playerDataBase.Developer == 1)
        {
            CheckKey(KeyCode.R, () => GoToRetry());
            CheckKey(KeyCode.Q, () => Invincibility());
            CheckKey(KeyCode.E, () => SaveSetting());
            CheckKey(KeyCode.U, () => ResetSetting());
            CheckKey(KeyCode.M, () => MiniMapSetting());
            CheckKey(KeyCode.Alpha1, () => SpeedUp());
            CheckKey(KeyCode.Alpha2, () => SpeedDown());
            CheckKey(KeyCode.Alpha3, () => AccelerationUp());
            CheckKey(KeyCode.Alpha4, () => AccelerationDown());
            CheckKey(KeyCode.Alpha5, () => RotationalSpeedUp());
            CheckKey(KeyCode.Alpha6, () => RotationalSpeedDown());
            CheckKey(KeyCode.Alpha7, () => MissileSpeedUp());
            CheckKey(KeyCode.Alpha8, () => MissileSpeedDown());
            CheckKey(KeyCode.Alpha9, () => PlayerSizeUp());
            CheckKey(KeyCode.Alpha0, () => PlayerSizeDown());
            CheckKey(KeyCode.N, () => NextStage_Developer());
            CheckKey(KeyCode.P, () => PreviousStage());
            CheckKey(KeyCode.T, () => OpenDeveloperMode());
        }
    }

    void CheckKey(KeyCode key, System.Action action)
    {
        if (Input.GetKeyDown(key) && Time.time - keyCooldowns[key] >= cooldownTime)
        {
            keyCooldowns[key] = Time.time; // ���� �ð����� ��ٿ� ����
            action?.Invoke(); // �׼� ����
        }
    }

    public void GameStart()
    {
        //savePlaysePos.StartSavePos();

        StartTimer(); // Ÿ�̸� ����

        OnGameStart?.Invoke();
    }

    public void StartTimer()
    {
        if (!isTimer)
        {
            isTimer = true;
            StartCoroutine(TimerCoroutine());
        }
    }

    public void StopTimer()
    {
        isTimer = false;
        isRunning = false;
    }

    public void ResetTimer()
    {
        StopTimer();
        elapsedTime = 0f;
        UpdateTimerText();
    }

    public void TryConnect()
    {
        GameOver(gameOverNumber);
    }

    public void GameOver(int number)
    {
        gameOverNumber = number;

        if (!NetworkConnect.instance.CheckConnectInternet())
        {
            checkInternet.SetActive(true);
            SoundManager.instance.PlaySFX(GameSfxType.Warning);
            return;
        }

        GameStateManager.instance.IsPlaying = false;

        playerDataBase.CountGameOver += 1;
        PlayfabManager.instance.UpdatePlayerStatisticsInsert("CountGameOver", playerDataBase.CountGameOver);

        if (SoundManager.instance != null)
        {
            switch(number)
            {
                case 0:
                    SoundManager.instance.PlaySFX(GameSfxType.OutLoadDead);

                    playerDataBase.CountOutLoadDead += 1;
                    PlayfabManager.instance.UpdatePlayerStatisticsInsert("CountOutLoadDead", playerDataBase.CountOutLoadDead);

                    Debug.Log("���� �ۿ��� ����");
                    break;
                case 1:
                    SoundManager.instance.PlaySFX(GameSfxType.MissileDead);
                    SoundManager.instance.PlaySFX(GameSfxType.GameOver);

                    playerDataBase.CountMissileDead += 1;
                    PlayfabManager.instance.UpdatePlayerStatisticsInsert("CountMissileDead", playerDataBase.CountMissileDead);

                    Debug.Log("�̻��Ͽ� ����");
                    break;
                case 2:
                    SoundManager.instance.PlaySFX(GameSfxType.LaserDead);
                    SoundManager.instance.PlaySFX(GameSfxType.GameOver);

                    playerDataBase.CountLaserDead += 1;
                    PlayfabManager.instance.UpdatePlayerStatisticsInsert("CountLaserDead", playerDataBase.CountLaserDead);

                    Debug.Log("�������� ����");
                    break;
                case 3:
                    SoundManager.instance.PlaySFX(GameSfxType.UFODead);

                    playerDataBase.CountUFODead += 1;
                    PlayfabManager.instance.UpdatePlayerStatisticsInsert("CountUFODead", playerDataBase.CountUFODead);

                    Debug.Log("UFO�� ��ġ��");
                    break;
                case 4:


                    Debug.Log("������ �������ٷ� ���");
                    break;
                default:

                    SoundManager.instance.PlaySFX(GameSfxType.GameOver);

                    playerDataBase.CountUnknown += 1;
                    PlayfabManager.instance.UpdatePlayerStatisticsInsert("CountUnknown", playerDataBase.CountUnknown);

                    Debug.Log("�� �� ���� ����");
                    break;
            }
        }

        //savePlaysePos.EndSavePlayerPos(PlayerPrefs.GetInt("Stage"));

        StopTimer();
        delayView.SetActive(true);
        gameOverView.SetActive(true);
        gameOverImage.sprite = gameOverSprite[number];

        gameOverProgressText.localizationName = "Progress";
        gameOverProgressText.plusText = " : " + progress.ToString("N2") + "%";
        gameOverProgressText.ReLoad();

        Invoke("Delay", 0.3f);
    }

    void Delay()
    {
        delayView.SetActive(false);
    }

    public void GameClear()
    {
        if (!isRunning) return;

        if(!CheckHackTime() && playerDataBase.Developer != 1)
        {
            Debug.Log("���ǵ� ���Դϴ�");

            PlayfabManager.instance.UpdatePlayerStatisticsInsert("Hack", 1);
            GameOver(0);
            return;
        }

        //savePlaysePos.EndSavePlayerPos(PlayerPrefs.GetInt("Stage"));

        StopTimer();

        nextStageButton.SetActive(false);

        if (nowStage + 1 >= maxStage)
        {
            gameDeathView.SetActive(true);
            nextStageButton.SetActive(false);
        }
        else
        {
            nextStageButton.SetActive(true);

            if (playerDataBase.Developer != 1)
            {
                SaveTime();
            }

            delayView.SetActive(true);
            gameClearView.SetActive(true);
            gameClearView_TimerText.text = timerText.text;

            Invoke("Delay", 0.3f);

            if (SoundManager.instance != null)
            {
                SoundManager.instance.PlaySFX(GameSfxType.GameClear);
            }

            if (nowStage + 1 > playerDataBase.Stage)
            {
                playerDataBase.Stage = nowStage + 1;
                PlayfabManager.instance.UpdatePlayerStatisticsInsert("Stage", playerDataBase.Stage);

                Debug.Log("�ְ� �������� ���� : " + (nowStage + 1));
            }
        }
    }

    public void DeathButton(int number)
    {
        gameDeathView.SetActive(false);

        if(number == 0)
        {
            if(GameStateManager.instance.DeathNumber == 2)
            {
                CutSceneManager.instance.OpenView(3);

                SoundManager.instance.PlaySFX(GameSfxType.SelectFail);

                GameOver(4);

                return;
            }
        }
        else
        {
            if (GameStateManager.instance.DeathNumber == 1)
            {
                CutSceneManager.instance.OpenView(3);

                SoundManager.instance.PlaySFX(GameSfxType.SelectFail);

                GameOver(4);

                return;
            }
        }

        if (!GameStateManager.instance.Ending)
        {
            CutSceneManager.instance.OpenView(2);

            GameStateManager.instance.Ending = true;
            GameStateManager.instance.IsPlaying = false;
        }

        if (playerDataBase.Developer != 1)
        {
            SaveTime();
        }

        delayView.SetActive(true);
        gameClearView.SetActive(true);
        gameClearView_TimerText.text = timerText.text;

        Invoke("Delay", 0.3f);

        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlaySFX(GameSfxType.GameClear);
        }

        playerDataBase.Stage = maxStage;
        PlayfabManager.instance.UpdatePlayerStatisticsInsert("Stage", playerDataBase.Stage);

        Debug.Log("�ְ� ��������!");
    }    

    public bool CheckHackTime() //Ŭ���� �ð��� �ʹ� ���� ���
    {
        timeAttack = ((minutes * 60 * 1000) + (seconds * 1000) + milliseconds);

        bool check = true;

        switch (nowStage)
        {
            case 0:
                if(timeAttack <= 8000)
                {
                    check = false;
                }
                break;
            case 1:
                if (timeAttack <= 8500)
                {
                    check = false;
                }
                break;
            case 2:
                if (timeAttack <= 9000)
                {
                    check = false;
                }
                break;
            case 3:
                if (timeAttack <= 9000)
                {
                    check = false;
                }
                break;
            case 4:
                if (timeAttack <= 19000)
                {
                    check = false;
                }
                break;
            case 5:
                if (timeAttack <= 21000)
                {
                    check = false;
                }
                break;
            case 6:
                if (timeAttack <= 34000)
                {
                    check = false;
                }
                break;
            case 7:
                if (timeAttack <= 36000)
                {
                    check = false;
                }
                break;
        }

        return check;
    }

    public void SaveTime()
    {
        timeAttack = 10000000 - ((minutes * 60 * 1000) + (seconds * 1000) + milliseconds);

        gameClearView_BestTime.SetActive(false);

        Debug.Log("Ŭ���� Ÿ�� : " + timeAttack);

        switch (nowStage)
        {
            case 0:
                if(playerDataBase.TimeAttackStage1 != 10000000 && playerDataBase.TimeAttackStage1 > timeAttack)
                {
                    Debug.Log("�������� " + (nowStage + 1) + " �ְ� ��� ���� ����");
                    return;
                }

                Debug.Log("�������� " + (nowStage + 1) + " �ְ� ��� ���� ����");

                gameClearView_BestTime.SetActive(true);
                playerDataBase.TimeAttackStage1 = timeAttack;
                PlayfabManager.instance.UpdatePlayerStatisticsInsert("TimeAttackStage1", timeAttack);

                break;
            case 1:
                if (playerDataBase.TimeAttackStage2 != 10000000 && playerDataBase.TimeAttackStage2 > timeAttack)
                {
                    Debug.Log("�������� " + (nowStage + 1) + " �ְ� ��� ���� ����");
                    return;
                }

                Debug.Log("�������� " + (nowStage + 1) + " �ְ� ��� ���� ����");

                gameClearView_BestTime.SetActive(true);
                playerDataBase.TimeAttackStage2 = timeAttack;
                PlayfabManager.instance.UpdatePlayerStatisticsInsert("TimeAttackStage2", timeAttack);

                break;
            case 2:
                if (playerDataBase.TimeAttackStage3 != 10000000 && playerDataBase.TimeAttackStage3 > timeAttack)
                {
                    Debug.Log("�������� " + (nowStage + 1) + " �ְ� ��� ���� ����");
                    return;
                }

                Debug.Log("�������� " + (nowStage + 1) + " �ְ� ��� ���� ����");

                gameClearView_BestTime.SetActive(true);
                playerDataBase.TimeAttackStage3 = timeAttack;
                PlayfabManager.instance.UpdatePlayerStatisticsInsert("TimeAttackStage3", timeAttack);

                break;
            case 3:
                if (playerDataBase.TimeAttackStage4 != 10000000 && playerDataBase.TimeAttackStage4 > timeAttack)
                {
                    Debug.Log("�������� " + (nowStage + 1) + " �ְ� ��� ���� ����");
                    return;
                }

                Debug.Log("�������� " + (nowStage + 1) + " �ְ� ��� ���� ����");

                gameClearView_BestTime.SetActive(true);
                playerDataBase.TimeAttackStage4 = timeAttack;
                PlayfabManager.instance.UpdatePlayerStatisticsInsert("TimeAttackStage4", timeAttack);

                break;
            case 4:
                if (playerDataBase.TimeAttackStage5 != 10000000 && playerDataBase.TimeAttackStage5 > timeAttack)
                {
                    Debug.Log("�������� " + (nowStage + 1) + " �ְ� ��� ���� ����");
                    return;
                }

                Debug.Log("�������� " + (nowStage + 1) + " �ְ� ��� ���� ����");

                gameClearView_BestTime.SetActive(true);
                playerDataBase.TimeAttackStage5 = timeAttack;
                PlayfabManager.instance.UpdatePlayerStatisticsInsert("TimeAttackStage5", timeAttack);

                break;
            case 5:
                if (playerDataBase.TimeAttackStage6 != 10000000 && playerDataBase.TimeAttackStage6 > timeAttack)
                {
                    Debug.Log("�������� " + (nowStage + 1) + " �ְ� ��� ���� ����");
                    return;
                }

                Debug.Log("�������� " + (nowStage + 1) + " �ְ� ��� ���� ����");

                gameClearView_BestTime.SetActive(true);
                playerDataBase.TimeAttackStage6 = timeAttack;
                PlayfabManager.instance.UpdatePlayerStatisticsInsert("TimeAttackStage6", timeAttack);

                break;
            case 6:
                if (playerDataBase.TimeAttackStage7 != 10000000 && playerDataBase.TimeAttackStage7 > timeAttack)
                {
                    Debug.Log("�������� " + (nowStage + 1) + " �ְ� ��� ���� ����");
                    return;
                }

                Debug.Log("�������� " + (nowStage + 1) + " �ְ� ��� ���� ����");

                gameClearView_BestTime.SetActive(true);
                playerDataBase.TimeAttackStage7 = timeAttack;
                PlayfabManager.instance.UpdatePlayerStatisticsInsert("TimeAttackStage7", timeAttack);

                break;
            case 7:
                if (playerDataBase.TimeAttackStage8 != 10000000 && playerDataBase.TimeAttackStage8 > timeAttack)
                {
                    Debug.Log("�������� " + (nowStage + 1) + " �ְ� ��� ���� ����");
                    return;
                }

                Debug.Log("�������� " + (nowStage + 1) + " �ְ� ��� ���� ����");

                gameClearView_BestTime.SetActive(true);
                playerDataBase.TimeAttackStage8 = timeAttack;
                PlayfabManager.instance.UpdatePlayerStatisticsInsert("TimeAttackStage8", timeAttack);

                break;
            case 8:
                if (playerDataBase.TimeAttackStage9 != 10000000 && playerDataBase.TimeAttackStage9 > timeAttack)
                {
                    Debug.Log("�������� " + (nowStage + 1) + " �ְ� ��� ���� ����");
                    return;
                }

                Debug.Log("�������� " + (nowStage + 1) + " �ְ� ��� ���� ����");

                gameClearView_BestTime.SetActive(true);
                playerDataBase.TimeAttackStage9 = timeAttack;
                PlayfabManager.instance.UpdatePlayerStatisticsInsert("TimeAttackStage9", timeAttack);

                break;
            case 9:
                if (playerDataBase.TimeAttackStage10 != 10000000 && playerDataBase.TimeAttackStage10 > timeAttack)
                {
                    Debug.Log("�������� " + (nowStage + 1) + " �ְ� ��� ���� ����");
                    return;
                }

                Debug.Log("�������� " + (nowStage + 1) + " �ְ� ��� ���� ����");

                gameClearView_BestTime.SetActive(true);
                playerDataBase.TimeAttackStage10 = timeAttack;
                PlayfabManager.instance.UpdatePlayerStatisticsInsert("TimeAttackStage10", timeAttack);

                break;
        }

        totalTimeAttack = 10000000 - playerDataBase.GetTotalTimeAttack();
        playerDataBase.TotalTimeAttack = totalTimeAttack;
        PlayfabManager.instance.UpdatePlayerStatisticsInsert("TotalTimeAttack", totalTimeAttack);
    }


    public void GamePause()
    {
        if (!isRunning) return;

        if (!gamePauseView.activeInHierarchy)
        {
            gamePauseView.SetActive(true);

            Time.timeScale = 0;
        }
        else
        {
            GameContinue();
        }
    }

    public void GameContinue()
    {
        Time.timeScale = 1;

        gamePauseView.SetActive(false);
    }

    private IEnumerator TimerCoroutine()
    {
        while (isTimer)
        {
            elapsedTime += Time.deltaTime;

            // UI ����
            UpdateTimerText();

            yield return null;
        }
    }

    // Ÿ�̸� UI ������Ʈ
    private void UpdateTimerText()
    {
        minutes = Mathf.FloorToInt(elapsedTime / 60f); // �� ���
        seconds = Mathf.FloorToInt(elapsedTime % 60f); // �� ���
        milliseconds = Mathf.FloorToInt((elapsedTime * 1000f) % 1000f); // �и������� ���

        timerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    public void AddMapController(MapController mapController)
    {
        mapControllerList.Add(mapController);
    }

    public void GoToRetry()
    {
        Time.timeScale = 1;

        GameStateManager.instance.AdCount += 1;

        if(GameStateManager.instance.AdCount >= 3)
        {
            GameStateManager.instance.AdCount = 0;

#if  UNITY_ANDORID || UNITY_IOS
            GoogleAdsManager.instance.screen.ShowAd(0, SuccessWatchAd);
#endif

            return;
        }

        SceneManager.LoadScene("MainScene");
    }

    public void SuccessWatchAd()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void Option()
    {
        OptionManager.instance.OpenOption();
    }

    public void Stage()
    {
        StageManager.instance.OpenView();
    }

    public void GoToMain()
    {
        Time.timeScale = 1;

        if (SoundManager.instance != null)
        {
            SoundManager.instance.StopSFX();
        }

        SceneManager.LoadScene("LoginScene");
    }

    public void Invincibility()
    {
        playerController.Invincibility();

        if(!invincibility.activeInHierarchy)
        {
            invincibility.SetActive(true);
        }
        else
        {
            invincibility.SetActive(false);
        }
    }

    public void SpeedUp()
    {
        playerController.SpeedUp();

        SetText();
    }

    public void SpeedDown()
    {
        playerController.SpeedDown();

        SetText();
    }
    public void AccelerationUp()
    {
        playerController.AccelerationUp();

        SetText();
    }
    public void AccelerationDown()
    {
        playerController.AccelerationDown();

        SetText();
    }
    public void RotationalSpeedUp()
    {
        playerController.RotationalSpeedUp();

        SetText();
    }
    public void RotationalSpeedDown()
    {
        playerController.RotationalSpeedDown();

        SetText();
    }

    public void SaveSetting()
    {
        playerController.SaveSetting();

        SetText();

        Debug.Log("���� �� ���� �Ϸ�");
    }

    public void ResetSetting()
    {
        playerController.ResetSetting();

        SetText();

        Debug.Log("���� �� �ʱ�ȭ �Ϸ�");
    }

    public void MiniMapSetting()
    {
        if(!miniMap.activeInHierarchy)
        {
            miniMap.SetActive(true);
        }
        else
        {
            miniMap.SetActive(false);
        }
    }

    public void MissileSpeedUp()
    {
        for (int i = 0; i < mapControllerList.Count; i++)
        {
            mapControllerList[i].SpeedUp();
        }

        SetText();
    }
    public void MissileSpeedDown()
    {
        for (int i = 0; i < mapControllerList.Count; i++)
        {
            mapControllerList[i].SpeedDown();
        }

        SetText();
    }

    public void PlayerSizeUp()
    {
        playerController.PlayerSizeUp();

        SetText();
    }

    public void PlayerSizeDown()
    {
        playerController.PlayerSizeDown();

        SetText();
    }

    public void NextStage()
    {
        if(!isClick)
        {
            Debug.Log("���� ��������");

            PlayerPrefs.SetInt("Stage", nowStage + 1);

            isClick = true;

            SceneManager.LoadScene("MainScene");
        }
    }
    
    public void NextStage_Developer()
    {
        if (!isClick)
        {
            Debug.Log("���� ��������");

            PlayerPrefs.SetInt("Stage", nowStage + 1);

            isClick = true;

            playerDataBase.Stage += 1;
            SceneManager.LoadScene("MainScene");
        }
    }

    public void PreviousStage()
    {
        if (!isClick)
        {
            Debug.Log("���� ��������");

            isClick = true;

            playerDataBase.Stage -= 1;
            PlayerPrefs.SetInt("Stage", nowStage - 1);

            if (playerDataBase.Stage < 0)
            {
                playerDataBase.Stage = 0;

                PlayerPrefs.SetInt("Stage", 0);
            }

            SceneManager.LoadScene("MainScene");
        }
    }


    public void SetText()
    {
        playerSpeedText.text = "�̵� �ӵ� : " + playerController.maxSpeed.ToString("N1");
        playerAccelerationText.text = "���ӵ� : " + playerController.acceleration.ToString("N1");
        playerRotationalSpeedText.text = "ȸ�� �ӵ� : " + playerController.rotationSpeed.ToString("N1");

        if (nowStage + 1 < 5)
        {
            missileSpeedText.text = "�̻��� �ӵ� : " + mapControllerList[0].cannonList[0].missileSpeed.ToString("N1");
        }
        else
        {
            missileSpeedText.text = "�̻��� �ӵ� : " + (mapControllerList[0].cannonList[0].missileSpeed + 0.5f).ToString("N1");
        }

        playerSizeText.text = "ĳ���� ũ�� : " + playerController.size.ToString("N1");
    }

    public void OpenDeveloperMode()
    {
        if (!developer.activeInHierarchy)
        {
            developer.SetActive(true);
        }
        else
        {
            developer.SetActive(false);
        }
    }

    public void UpdatePlayerProgress(int index)
    {
        progress = (float)index / (stageDataLoader.generatedTiles.Count - 1) * 100f; // ���൵ ���

        Debug.Log(index + "��° �ʿ� ����");
    }
}
