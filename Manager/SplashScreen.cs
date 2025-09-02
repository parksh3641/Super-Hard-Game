using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Net.Sockets;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour
{
    [Header("Canvas Group 설정")]
    public GameObject logoObject;
    public CanvasGroup logoGroup; // CanvasGroup 참조
    public AudioSource audioSource;

    [Header("Language")]
    public GameObject language;
    public Image[] buttonImg;
    public Sprite[] buttonSprite;
    public GameObject locked;
    public bool isSelected = false;
    LanguageType languageType = LanguageType.Default;

    [Header("페이드 설정")]
    public float initialDelay = 1.0f;    // 시작 전 대기 시간
    public float fadeInDuration = 1.0f;  // 페이드 인 시간
    public float displayDuration = 1.0f; // 표시 시간
    public float fadeOutDuration = 1.0f; // 페이드 아웃 시간

    private void Awake()
    {
        logoObject.SetActive(false);
        language.SetActive(false);

        audioSource.Stop();
    }

    void Start()
    {
        logoObject.SetActive(true);

        if (logoGroup != null)
        {
            // 처음에 완전히 투명하게 설정
            logoGroup.alpha = 0;
        }

        // 페이드 효과 시작
        StartCoroutine(FadeLogo());

        if (GameStateManager.instance.Language == LanguageType.Default)
        {
            Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);

#if UNITY_ANDROID || UNITY_IOS

            if (Application.systemLanguage == SystemLanguage.Korean)
            {
                GameStateManager.instance.Language = LanguageType.Korean;
            }
            else if (Application.systemLanguage == SystemLanguage.Japanese)
            {
                GameStateManager.instance.Language = LanguageType.Japanese;
            }
            else if (Application.systemLanguage == SystemLanguage.Chinese)
            {
                GameStateManager.instance.Language = LanguageType.Chinese;
            }
            else if (Application.systemLanguage == SystemLanguage.Portuguese)
            {
                GameStateManager.instance.Language = LanguageType.Portuguese;
            }
            else if (Application.systemLanguage == SystemLanguage.Russian)
            {
                GameStateManager.instance.Language = LanguageType.Russian;
            }
            else if (Application.systemLanguage == SystemLanguage.German)
            {
                GameStateManager.instance.Language = LanguageType.German;
            }
            else if (Application.systemLanguage == SystemLanguage.Spanish)
            {
                GameStateManager.instance.Language = LanguageType.Spanish;
            }
            else if (Application.systemLanguage == SystemLanguage.Arabic)
            {
                GameStateManager.instance.Language = LanguageType.Arabic;
            }
            else if (Application.systemLanguage == SystemLanguage.Indonesian)
            {
                GameStateManager.instance.Language = LanguageType.Indonesian;
            }
            else if (Application.systemLanguage == SystemLanguage.Italian)
            {
                GameStateManager.instance.Language = LanguageType.Italian;
            }
            else if (Application.systemLanguage == SystemLanguage.Dutch)
            {
                GameStateManager.instance.Language = LanguageType.Dutch;
            }
            else if (Application.systemLanguage.ToString() == "Hindi")
            {
                GameStateManager.instance.Language = LanguageType.Indian;
            }
            else if (Application.systemLanguage == SystemLanguage.Vietnamese)
            {
                GameStateManager.instance.Language = LanguageType.Vietnamese;
            }
            //else if (Application.systemLanguage == SystemLanguage.Thai)
            //{
            //    GameStateManager.instance.Language = LanguageType.Thai;
            //}
            else
            {
                GameStateManager.instance.Language = LanguageType.Korean;
            }
        }
#endif
        }
    }

    IEnumerator FadeLogo()
    {
        // 1. 처음 1초 대기
        yield return new WaitForSeconds(initialDelay);

        audioSource.volume = GameStateManager.instance.SfxValue;
        audioSource.Play();

        // 2. 페이드 인 (0 → 1)
        yield return StartCoroutine(FadeCanvasGroup(0, 1, fadeInDuration));

        // 3. 1초 동안 유지
        yield return new WaitForSeconds(displayDuration);

        // 4. 페이드 아웃 (1 → 0)
        yield return StartCoroutine(FadeCanvasGroup(1, 0, fadeOutDuration));

        // 5. 1초 대기
        yield return new WaitForSeconds(displayDuration);

        if(GameStateManager.instance.Language == LanguageType.Default)
        {
            language.SetActive(true);

            locked.SetActive(true);
            isSelected = false;

            for(int i = 0; i < buttonImg.Length; i ++)
            {
                buttonImg[i].sprite = buttonSprite[0];
            }
        }
        else
        {
            SceneManager.LoadScene("LoginScene");
        }
    }

    IEnumerator FadeCanvasGroup(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            logoGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            yield return null;
        }

        logoGroup.alpha = endAlpha;
    }

    public void ChangeKorean()
    {
        ChangeLanguage(LanguageType.Korean);
    }

    public void ChangeEnglish()
    {
        ChangeLanguage(LanguageType.English);
    }

    public void ChangeJapanese()
    {
        ChangeLanguage(LanguageType.Japanese);
    }

    public void ChangeChinese()
    {
        ChangeLanguage(LanguageType.Chinese);
    }

    public void ChangeIndian()
    {
        ChangeLanguage(LanguageType.Indian);
    }

    public void ChangePortuguese()
    {
        ChangeLanguage(LanguageType.Portuguese);
    }

    public void ChangeRussian()
    {
        ChangeLanguage(LanguageType.Russian);
    }

    public void ChangeGerman()
    {
        ChangeLanguage(LanguageType.German);
    }

    public void ChangeSpanish()
    {
        ChangeLanguage(LanguageType.Spanish);
    }

    public void ChangeArabic()
    {
        ChangeLanguage(LanguageType.Arabic);
    }

    public void ChangeBengali()
    {
        ChangeLanguage(LanguageType.Bengali);
    }

    public void ChangeIndonesian()
    {
        ChangeLanguage(LanguageType.Indonesian);
    }

    public void ChangeItalian()
    {
        ChangeLanguage(LanguageType.Italian);
    }

    public void ChangeDutch()
    {
        ChangeLanguage(LanguageType.Dutch);
    }

    public void ChangeVietnamese()
    {
        ChangeLanguage(LanguageType.Vietnamese);
    }

    public void ChangeThai()
    {
        ChangeLanguage(LanguageType.Thai);
    }

    public void ChangeLanguage(LanguageType type)
    {
        languageType = type;

        locked.SetActive(false);
        isSelected = true;

        for (int i = 0; i < buttonImg.Length; i++)
        {
            buttonImg[i].sprite = buttonSprite[0];
        }

        buttonImg[(int)type - 1].sprite = buttonSprite[1];
    }

    public void Confrim()
    {
        if (!isSelected) return;

        GameStateManager.instance.Language = languageType;

        SceneManager.LoadScene("LoginScene");
    }
}