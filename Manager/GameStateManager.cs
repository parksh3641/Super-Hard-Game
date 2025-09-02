using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;

    public GameSettings gameSettings;

    [NonSerialized]
    public const string DEVICESETTINGFILENAME = "DeviceSetting.bin";

    [SerializeField]
    public class GameSettings
    {
        [Space]
        [Header("Login")]
        public string playfabId = "";
        public string customId = "";
        public string stoveId = "";
        public string region = "";

        public bool autoLogin = false;
        public LoginType loginType = LoginType.None;
        public string nickName = "";

        [Space]
        [Header("Language")]
        public LanguageType language = LanguageType.Default;

        [Space]
        [Header("Game Setting")]
        public ScreenMode screenMode = ScreenMode.FullScreen;
        public Resolution resolution = Resolution.FHD_1920x1080;

        public bool music = true;
        public float musicValue = 1.0f;
        public bool sfx = true;
        public float sfxValue = 1.0f;
        public bool vibration = true;
        public int mouseSensitivity = 0;

        [Space]
        [Header("Player Setting")]
        public bool isPlaying = false;
        public float playerMoveSpeed = 5.8f;
        public float playerAcceleration = 25.0f;
        public float playerRotationalSpeed = 910.0f;
        public float missileSpeed = 4.5f;
        public int deathNumber = 0;
        public int adCount = 0;

        [Space]
        [Header("Cut Scene")]
        public bool first = false;
        public bool opening = false;
        public bool mid = false;
        public bool ending = false;
    }

    #region Data

    public string PlayfabId
    {
        get
        {
            return gameSettings.playfabId;
        }
        set
        {
            gameSettings.playfabId = value;
            SaveFile();
        }
    }

    public string CustomId
    {
        get
        {
            return gameSettings.customId;
        }
        set
        {
            gameSettings.customId = value;
            SaveFile();
        }
    }

    public string StoveId
    {
        get
        {
            return gameSettings.stoveId;
        }
        set
        {
            gameSettings.stoveId = value;
            SaveFile();
        }
    }

    public string Region
    {
        get
        {
            return gameSettings.region;
        }
        set
        {
            gameSettings.region = value;
            SaveFile();
        }
    }

    public string NickName
    {
        get
        {
            return gameSettings.nickName;
        }
        set
        {
            gameSettings.nickName = value;
            SaveFile();
        }
    }

    public LanguageType Language
    {
        get
        {
            return gameSettings.language;
        }
        set
        {
            gameSettings.language = value;
            SaveFile();
        }
    }
    public bool AutoLogin
    {
        get
        {
            return gameSettings.autoLogin;
        }
        set
        {
            gameSettings.autoLogin = value;
            SaveFile();
        }
    }

    public LoginType Login
    {
        get
        {
            return gameSettings.loginType;
        }
        set
        {
            gameSettings.loginType = value;
            SaveFile();
        }
    }

    public ScreenMode ScreenMode
    {
        get
        {
            return gameSettings.screenMode;
        }
        set
        {
            gameSettings.screenMode = value;
            SaveFile();
        }
    }

    public Resolution Resolution
    {
        get
        {
            return gameSettings.resolution;
        }
        set
        {
            gameSettings.resolution = value;
            SaveFile();
        }
    }

    public bool Music
    {
        get
        {
            return gameSettings.music;
        }
        set
        {
            gameSettings.music = value;
            SaveFile();
        }
    }

    public float MusicValue
    {
        get
        {
            return gameSettings.musicValue;
        }
        set
        {
            gameSettings.musicValue = value;
            SaveFile();
        }
    }

    public bool Sfx
    {
        get
        {
            return gameSettings.sfx;
        }
        set
        {
            gameSettings.sfx = value;
            SaveFile();
        }
    }

    public float SfxValue
    {
        get
        {
            return gameSettings.sfxValue;
        }
        set
        {
            gameSettings.sfxValue = value;
            SaveFile();
        }
    }

    public bool Vibration
    {
        get
        {
            return gameSettings.vibration;
        }
        set
        {
            gameSettings.vibration = value;
            SaveFile();
        }
    }

    public int MouseSensitivity
    {
        get
        {
            return gameSettings.mouseSensitivity;
        }
        set
        {
            gameSettings.mouseSensitivity = value;
            SaveFile();
        }
    }

    public bool IsPlaying
    {
        get
        {
            return gameSettings.isPlaying;
        }
        set
        {
            gameSettings.isPlaying = value;
            SaveFile();
        }
    }

    public float PlayerMoveSpeed
    {
        get
        {
            return gameSettings.playerMoveSpeed;
        }
        set
        {
            gameSettings.playerMoveSpeed = value;
            SaveFile();
        }
    }

    public float PlayerAcceleration
    {
        get
        {
            return gameSettings.playerAcceleration;
        }
        set
        {
            gameSettings.playerAcceleration = value;
            SaveFile();
        }
    }

    public float PlayerRotationalSpeed
    {
        get
        {
            return gameSettings.playerRotationalSpeed;
        }
        set
        {
            gameSettings.playerRotationalSpeed = value;
            SaveFile();
        }
    }

    public float MissileSpeed
    {
        get
        {
            return gameSettings.missileSpeed;
        }
        set
        {
            gameSettings.missileSpeed = value;
            SaveFile();
        }
    }

    public int DeathNumber
    {
        get
        {
            return gameSettings.deathNumber;
        }
        set
        {
            gameSettings.deathNumber = value;
            SaveFile();
        }
    }

    public int AdCount
    {
        get
        {
            return gameSettings.adCount;
        }
        set
        {
            gameSettings.adCount = value;
            SaveFile();
        }
    }

    public bool First
    {
        get
        {
            return gameSettings.first;
        }
        set
        {
            gameSettings.first = value;
            SaveFile();
        }
    }

    public bool Opening
    {
        get
        {
            return gameSettings.opening;
        }
        set
        {
            gameSettings.opening = value;
            SaveFile();
        }
    }

    public bool Mid
    {
        get
        {
            return gameSettings.mid;
        }
        set
        {
            gameSettings.mid = value;
            SaveFile();
        }
    }

    public bool Ending
    {
        get
        {
            return gameSettings.ending;
        }
        set
        {
            gameSettings.ending = value;
            SaveFile();
        }
    }

    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 이동 후에도 유지
        }
        else
        {
            Destroy(gameObject); // 기존 인스턴스가 있으면 새 객체 파괴
        }

        LoadData();
    }

    public void Initialize()
    {
        gameSettings = new GameSettings();

        string str = JsonUtility.ToJson(gameSettings);
        FileIO.SaveData(DEVICESETTINGFILENAME, str, true);
    }

    private void LoadData()
    {
        try
        {
            string stjs = FileIO.LoadData(DEVICESETTINGFILENAME, true);

            if (!string.IsNullOrEmpty(stjs))
            {
                gameSettings = JsonUtility.FromJson<GameSettings>(stjs);
            }
            else
            {
                gameSettings = new GameSettings();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Load Error \n" + e.Message);
        }
    }

    public void SaveFile()
    {
        try
        {
            string str = JsonUtility.ToJson(gameSettings);
            FileIO.SaveData(DEVICESETTINGFILENAME, str, true);
        }
        catch (Exception e)
        {
            Debug.LogError("Save Error \n" + e.Message);
        }
    }
}