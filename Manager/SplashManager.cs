using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SplashManager : MonoBehaviour
{
    public static SplashManager instance;

    public Canvas loginCanvas;
    public Canvas mainCanvas;

    public TMP_Text nickNameText;
    public TMP_Text bestStageText;
    public TMP_Text versionText;

    public GameObject authFailedObj;
    public GameObject checkInternetObj;

    PlayerDataBase playerDataBase;

    private void Awake()
    {
        instance = this;

        if (playerDataBase == null) playerDataBase = Resources.Load("PlayerDataBase") as PlayerDataBase;

        loginCanvas.enabled = true;
        mainCanvas.enabled = false;

        versionText.text = "Ver " + Application.version.ToString();

        authFailedObj.SetActive(false);
        checkInternetObj.SetActive(false);
    }

    private void Start()
    {
        if (PlayfabManager.instance.isActive)
        {
            LoginSuccess();
        }
        else
        {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_WIN
            Login();
#endif
        }

        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayBGM(GameBGMType.Splash);
        }
    }


    public void Login()
    {
        PlayfabManager.instance.OnClickPlayfabLogin();
    }

    public void LoginSuccess()
    {
        loginCanvas.enabled = false;
        mainCanvas.enabled = true;

        nickNameText.text = GameStateManager.instance.NickName;
        bestStageText.text = LocalizationManager.instance.GetString("Stage") + " " + (playerDataBase.Stage + 1);
    }

    public void GameStart()
    {
        StageManager.instance.OpenView();

        //SceneManager.LoadScene("MainScene");
    }

    public void ChangeNickName()
    {
        NickNameManager.instance.OpenView(true);
    }

    public void Ranking()
    {
        RankingManager.instance.OpenRankingView();
    }

    public void Option()
    {
        OptionManager.instance.OpenOption();
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void CutScene(int number)
    {
        CutSceneManager.instance.OpenView(number);
    }

    public void UserAuthenticationFailed()
    {
        authFailedObj.SetActive(true);
    }
    public void CloseGame()
    {
        Application.Quit();
    }

    public void CheckInternetFailed()
    {
        checkInternetObj.SetActive(true);
    }

    public void CheckInternet()
    {
        PlayfabManager.instance.OnClickPlayfabLogin();
    }
}
