using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class OptionManager : MonoBehaviour
{
    public static OptionManager instance;

    public Canvas view;

    public GameObject musicCheckMark;
    public Slider musicSlider;

    [Space]
    public GameObject sfxCheckMark;
    public Slider sfxSlider;

    private bool isMusicOn = true;
    private bool isSfxOn = true;

    public Slider sensitivity;
    public TMP_Text sensitivityText;

    [Space]
    public GameObject[] screenModeCheckMark; //화면 모드
    public GameObject[] resolutionCheckMark; //해상도
    public GameObject[] languageCheckMark; //언어

    [Header("Credit")]
    public GameObject credit;


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

        view.enabled = false;
    }

    private void Update()
    {
        // Alt + Enter 입력 감지
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Return))
        {
            ToggleFullScreenMode();
        }

        if (GameStateManager.instance.ScreenMode == ScreenMode.FullScreen && Screen.fullScreenMode != FullScreenMode.FullScreenWindow)
        {
            GameStateManager.instance.ScreenMode = ScreenMode.Windowed;
            UpdateScreenModeUI();
        }
        else if (GameStateManager.instance.ScreenMode == ScreenMode.Windowed && Screen.fullScreenMode != FullScreenMode.Windowed)
        {
            GameStateManager.instance.ScreenMode = ScreenMode.FullScreen;
            UpdateScreenModeUI();
        }
    }

    public void OpenOption()
    {
        if(!view.enabled)
        {
            view.enabled = true;

            SoundManager.instance.PlaySFX(GameSfxType.OpenPopup);

            Initialize();
        }
        else
        {
            view.enabled = false;
        }
    }

    private void Initialize()
    {
        isMusicOn = GameStateManager.instance.Music;
        musicSlider.value = GameStateManager.instance.MusicValue;
        UpdateMusicUI();

        isSfxOn = GameStateManager.instance.Sfx;
        sfxSlider.value = GameStateManager.instance.SfxValue;
        UpdateSfxUI();

        musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        sfxSlider.onValueChanged.AddListener(ChangeSfxVolume);

        SoundManager.instance.SetMusic(musicSlider.value);
        SoundManager.instance.SetSfx(sfxSlider.value);

        sensitivity.value = GameStateManager.instance.MouseSensitivity;
        sensitivityText.text = sensitivity.value.ToString();
        sensitivity.onValueChanged.AddListener(ChangeMouseSensitivity);

        for (int i = 0; i < screenModeCheckMark.Length; i ++)
        {
            screenModeCheckMark[i].SetActive(false);
        }

        screenModeCheckMark[(int)GameStateManager.instance.ScreenMode].SetActive(true);

        for (int i = 0; i < resolutionCheckMark.Length; i++)
        {
            resolutionCheckMark[i].SetActive(false);
        }

        resolutionCheckMark[(int)GameStateManager.instance.Resolution].SetActive(true);

        for (int i = 0; i < languageCheckMark.Length; i++)
        {
            languageCheckMark[i].SetActive(false);
        }

        languageCheckMark[(int)GameStateManager.instance.Language - 1].SetActive(true);

        credit.SetActive(false);
    }

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        GameStateManager.instance.Music = isMusicOn;
        UpdateMusicUI();
    }

    private void ChangeMusicVolume(float value)
    {
        GameStateManager.instance.MusicValue = value;

        SoundManager.instance.SetMusic(musicSlider.value);

        if (value > 0)
        {
            GameStateManager.instance.Music = true;

            musicCheckMark.SetActive(false);
        }
        else
        {
            GameStateManager.instance.Music = false;

            musicCheckMark.SetActive(true);
        }
    }

    public void ToggleSfx()
    {
        isSfxOn = !isSfxOn;
        GameStateManager.instance.Sfx = isSfxOn;
        UpdateSfxUI();
    }

    private void ChangeSfxVolume(float value)
    {
        GameStateManager.instance.SfxValue = value;

        SoundManager.instance.SetSfx(sfxSlider.value);

        if (value > 0)
        {
            GameStateManager.instance.Sfx = true;

            sfxCheckMark.SetActive(false);
        }
        else
        {
            GameStateManager.instance.Sfx = false;

            sfxCheckMark.SetActive(true);
        }
    }

    private void UpdateMusicUI()
    {
        if (!isMusicOn)
        {
            PlayerPrefs.SetFloat("SaveMusicValue", musicSlider.value);

            musicSlider.value = 0;

            musicCheckMark.SetActive(true);
        }
        else
        {
            if (PlayerPrefs.GetFloat("SaveMusicValue") > 0f)
            {
                musicSlider.value = PlayerPrefs.GetFloat("SaveMusicValue");

                PlayerPrefs.SetFloat("SaveMusicValue", 0);
            }

            musicCheckMark.SetActive(false);
        }
    }

    private void UpdateSfxUI()
    {
        if (!isSfxOn)
        {
            PlayerPrefs.SetFloat("SaveSfxValue", sfxSlider.value);

            sfxSlider.value = 0;

            sfxCheckMark.SetActive(true);
        }
        else
        {
            if(PlayerPrefs.GetFloat("SaveSfxValue") > 0f)
            {
                sfxSlider.value = PlayerPrefs.GetFloat("SaveSfxValue");

                PlayerPrefs.SetFloat("SaveSfxValue", 0);
            }

            sfxCheckMark.SetActive(false);
        }
    }

    public void ChangeMouseSensitivity(float value)
    {
        GameStateManager.instance.MouseSensitivity = (int)value;
        sensitivityText.text = value.ToString();
    }

    public void ChangeScreenMode(int index)
    {
        switch (GameStateManager.instance.ScreenMode)
        {
            case ScreenMode.FullScreen:
                Screen.fullScreenMode = FullScreenMode.Windowed;

                GameStateManager.instance.ScreenMode = ScreenMode.Windowed;

                Debug.Log("FullScreenMode → Windowed");
                break;
            case ScreenMode.Windowed:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

                GameStateManager.instance.ScreenMode = ScreenMode.FullScreen;

                Debug.Log("Windowed → FullScreenMode");
                break;
        }

        for (int i = 0; i < screenModeCheckMark.Length; i++)
        {
            screenModeCheckMark[i].SetActive(false);
        }

        screenModeCheckMark[(int)GameStateManager.instance.ScreenMode].SetActive(true);
    }

    private void UpdateScreenModeUI()
    {
        for (int i = 0; i < screenModeCheckMark.Length; i++)
        {
            screenModeCheckMark[i].SetActive(false);
        }

        screenModeCheckMark[(int)GameStateManager.instance.ScreenMode].SetActive(true);
    }

    public void ChangeResolution(int index)
    {
        GameStateManager.instance.Resolution = Resolution.FHD_1920x1080 + index;

        int width = 1920;
        int height = 1080;

        switch (GameStateManager.instance.Resolution)
        {
            case Resolution.FHD_1920x1080:
                width = 1920;
                height = 1080;
                break;
            case Resolution.HD_1600x900:
                width = 1600;
                height = 900;
                break;
            case Resolution.HD_1280x720:
                width = 1280;
                height = 720;
                break;
        }

        // 창 모드를 윈도우 모드로 설정하여 해상도 변경 시 창 크기도 변경되도록 함
        Screen.SetResolution(width, height, FullScreenMode.Windowed);

        for (int i = 0; i < resolutionCheckMark.Length; i++)
        {
            resolutionCheckMark[i].SetActive(false);
        }

        resolutionCheckMark[(int)GameStateManager.instance.Resolution].SetActive(true);
    }


    public void ChangeLocalization(int index)
    {
        LocalizationManager.instance.ChangeLanguage(LanguageType.Korean + index);

        for (int i = 0; i < languageCheckMark.Length; i++)
        {
            languageCheckMark[i].SetActive(false);
        }

        languageCheckMark[(int)GameStateManager.instance.Language - 1].SetActive(true);

        Debug.Log("언어 변경 : " + (LanguageType.Korean + index));
    }

    private void ToggleFullScreenMode()
    {
        if (Screen.fullScreenMode == FullScreenMode.FullScreenWindow)
        {
            ChangeScreenMode(0);
        }
        else
        {
            ChangeScreenMode(1);
        }
    }

    public void OpenCredit()
    {
        credit.SetActive(true);
    }

    public void CloseCredit()
    {
        credit.SetActive(false);
    }

    public void ResetButton()
    {
        isMusicOn = true;
        isSfxOn = true;

        PlayerPrefs.SetFloat("SaveMusicValue", 0);
        PlayerPrefs.SetFloat("SaveSfxValue", 0);

        GameStateManager.instance.Music = true;
        GameStateManager.instance.MusicValue = 1.0f;
        GameStateManager.instance.Sfx = true;
        GameStateManager.instance.SfxValue = 1.0f;

        musicSlider.value = 1.0f;
        sfxSlider.value = 1.0f;

        UpdateMusicUI();
        UpdateSfxUI();

        SoundManager.instance.SetMusic(1.0f);
        SoundManager.instance.SetSfx(1.0f);

        GameStateManager.instance.MouseSensitivity = 0;
        sensitivity.value = 0;
        sensitivityText.text = "0";
    }
}
