using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NickNameManager : MonoBehaviour
{
    public static NickNameManager instance;

    public Canvas view;

    public TMP_InputField inputField;

    public LocalizationContent warningText;

    public string[] lines;
    string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

    public GameObject cancelButton;

    PlayerDataBase playerDataBase;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ¾À ÀÌµ¿ ÈÄ¿¡µµ À¯Áö
        }
        else
        {
            Destroy(gameObject); // ±âÁ¸ ÀÎ½ºÅÏ½º°¡ ÀÖÀ¸¸é »õ °´Ã¼ ÆÄ±«
        }

        inputField.text = "";

        string file = SystemPath.GetPath() + "BadWord.txt";
        string source;

        if (File.Exists(file))
        {
            StreamReader word = new StreamReader(file);
            source = word.ReadToEnd();
            word.Close();

            lines = Regex.Split(source, LINE_SPLIT_RE);
        }

        view.enabled = false;

        if (playerDataBase == null) playerDataBase = Resources.Load("PlayerDataBase") as PlayerDataBase;
    }

    public void OpenView(bool check)
    {
        if (!view.enabled)
        {
            inputField.text = "";
            warningText.localizationName = "";
            warningText.ReLoad();

            view.enabled = true;

            SoundManager.instance.PlaySFX(GameSfxType.OpenPopup);

            cancelButton.SetActive(check);
        }
    }
    public void CheckNickName()
    {
        if (!NetworkConnect.instance.CheckConnectInternet())
        {
            warningText.localizationName = "CheckInternet";
            warningText.ReLoad();

            SoundManager.instance.PlaySFX(GameSfxType.Warning);
            return;
        }

        for (int i = 0; i < lines.Length; i++)
        {
            if (inputField.text.ToLower().Contains(lines[i]))
            {
                warningText.localizationName = "SignNotion1";
                warningText.ReLoad();

                SoundManager.instance.PlaySFX(GameSfxType.Warning);
                return;
            }
        }

        string Check = Regex.Replace(inputField.text, @"[^a-zA-Z0-9°¡-ÆR]", "", RegexOptions.Singleline);

        if (inputField.text.Equals(Check) == true)
        {
            string newNickName = ((inputField.text.Trim()).Replace(" ", ""));
            string oldNickName = "";

            if (GameStateManager.instance.NickName != null)
            {
                oldNickName = GameStateManager.instance.NickName.Trim().Replace(" ", "");
            }
            else
            {
                oldNickName = "";
            }

            if (newNickName.Length > 2)
            {
                if (!(newNickName.Equals(oldNickName)))
                {
                    PlayfabManager.instance.UpdateDisplayName(newNickName, Success, Failure);
                }
                else
                {
                    warningText.localizationName = "SignNotion2";
                    warningText.ReLoad();
                    SoundManager.instance.PlaySFX(GameSfxType.Warning);
                }
            }
            else
            {
                warningText.localizationName = "SignNotion3";
                warningText.ReLoad();
                SoundManager.instance.PlaySFX(GameSfxType.Warning);
            }
        }
        else
        {
            warningText.localizationName = "SignNotion4";
            warningText.ReLoad();
            SoundManager.instance.PlaySFX(GameSfxType.Warning);
        }
    }

    private void Success()
    {
        warningText.localizationName = "SignNotion5";
        warningText.ReLoad();

        //SoundManager.instance.PlaySFX(GameSfxType.Success);
        //NotionManager.instance.UseNotion(NotionType.SignNotion6);

        playerDataBase.NickName = 1;
        PlayfabManager.instance.UpdatePlayerStatisticsInsert("NickName", 1);

        view.enabled = false;

        SplashManager.instance.LoginSuccess();
    }

    private void Failure()
    {
        warningText.localizationName = "SignNotion6";
        warningText.ReLoad();
        SoundManager.instance.PlaySFX(GameSfxType.Warning);
    }
}