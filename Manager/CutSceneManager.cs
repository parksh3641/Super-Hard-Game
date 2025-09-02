using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using System;

public class CutSceneManager : MonoBehaviour
{
    public static CutSceneManager instance;

    public Canvas view;

    public GameObject textField;
    public TW_Regular tW_Regular;

    [Space]
    [Header("Opening")]
    public Sprite[] openingImages; // 컷씬 이미지 배열
    public string[] openingTexts;
    public int[] openingSpeakTypes; //누가 말하는 중인지?
    public string[] openingSfxTypes; //이펙트 재생

    [Space]
    [Header("Mid")]
    public Sprite[] midImages;
    public string[] midTexts;
    public int[] midSpeakTypes;
    public string[] midSfxTypes;

    [Space]
    [Header("Ending")]
    public Sprite[] endingImages;
    public string[] endingTexts;
    public int[] endingSpeakTypes;
    public string[] endingSfxTypes;

    [Space]
    [Header("Ending_Fail")] //죽음의 이지선다 실패
    public Sprite[] endingFailImages;
    public string[] endingFailTexts;
    public int[] endingFailSpeakTypes;
    public string[] endingFailSfxTypes;

    [Space]
    [Header("Target")]
    public Image[] cutSceneImage;
    public string[] cutSceneTexts;
    public int[] cutSceneSpeakTypes;
    public string[] cutSceneSfxTypes;

    [SerializeField] private int cutsceneIndex = 0; //어떤 종류의 컷씬인지

    [SerializeField] private float fadeDuration = 1f; // 페이드 지속 시간
    [SerializeField] private int currentCutSceneIndex = 0; // 현재 컷씬 인덱스
    [SerializeField] private int endCutSceneIndex = 0;

    private bool isCutsceneActive = false;
    private bool isCutsceneFinished = false;

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

    public void OpenView(int number)
    {
        if (!view.enabled)
        {
            view.enabled = true;

            SoundManager.instance.StopBGM();

            Initialize(number);
        }
        else
        {
            view.enabled = false;
        }
    }

    void Initialize(int number)
    {
        cutsceneIndex = number;

        switch(number)
        {
            case 0:
                endCutSceneIndex = openingImages.Length - 1;

                for (int i =0; i < openingImages.Length; i ++)
                {
                    cutSceneImage[i].sprite = openingImages[i];
                }

                cutSceneTexts = new string[openingTexts.Length];

                for (int i = 0; i < openingTexts.Length; i++)
                {
                    cutSceneTexts[i] = openingTexts[i];
                }

                cutSceneSpeakTypes = new int[openingSpeakTypes.Length];

                for (int i = 0; i < openingSpeakTypes.Length; i++)
                {
                    cutSceneSpeakTypes[i] = openingSpeakTypes[i];
                }

                cutSceneSfxTypes = new string[openingSfxTypes.Length];

                for (int i = 0; i < openingSfxTypes.Length; i++)
                {
                    cutSceneSfxTypes[i] = openingSfxTypes[i];
                }

                Debug.Log("Opening CutScene");
                break;
            case 1:
                endCutSceneIndex = midImages.Length - 1;

                for (int i = 0; i < midImages.Length; i++)
                {
                    cutSceneImage[i].sprite = midImages[i];
                }

                cutSceneTexts = new string[midTexts.Length];

                for (int i = 0; i < midTexts.Length; i++)
                {
                    cutSceneTexts[i] = midTexts[i];
                }

                cutSceneSpeakTypes = new int[midSpeakTypes.Length];

                for (int i = 0; i < midSpeakTypes.Length; i++)
                {
                    cutSceneSpeakTypes[i] = midSpeakTypes[i];
                }

                cutSceneSfxTypes = new string[midSfxTypes.Length];

                for (int i = 0; i < midSfxTypes.Length; i++)
                {
                    cutSceneSfxTypes[i] = midSfxTypes[i];
                }

                Debug.Log("Mid CutScene");
                break;
            case 2:
                endCutSceneIndex = endingImages.Length - 1;

                for (int i = 0; i < endingImages.Length; i++)
                {
                    cutSceneImage[i].sprite = endingImages[i];
                }

                cutSceneTexts = new string[endingTexts.Length];

                for (int i = 0; i < endingTexts.Length; i++)
                {
                    cutSceneTexts[i] = endingTexts[i];
                }

                cutSceneSpeakTypes = new int[endingSpeakTypes.Length];

                for (int i = 0; i < endingSpeakTypes.Length; i++)
                {
                    cutSceneSpeakTypes[i] = endingSpeakTypes[i];
                }

                cutSceneSfxTypes = new string[endingSfxTypes.Length];

                for (int i = 0; i < endingSfxTypes.Length; i++)
                {
                    cutSceneSfxTypes[i] = endingSfxTypes[i];
                }

                Debug.Log("Ending CutScene");
                break;
            case 3:
                endCutSceneIndex = endingFailImages.Length - 1;

                for (int i = 0; i < endingFailImages.Length; i++)
                {
                    cutSceneImage[i].sprite = endingFailImages[i];
                }

                cutSceneTexts = new string[endingFailTexts.Length];

                for (int i = 0; i < endingFailTexts.Length; i++)
                {
                    cutSceneTexts[i] = endingFailTexts[i];
                }

                cutSceneSpeakTypes = new int[endingFailSpeakTypes.Length];

                for (int i = 0; i < endingFailSpeakTypes.Length; i++)
                {
                    cutSceneSpeakTypes[i] = endingFailSpeakTypes[i];
                }

                cutSceneSfxTypes = new string[endingFailSfxTypes.Length];

                for (int i = 0; i < endingFailSfxTypes.Length; i++)
                {
                    cutSceneSfxTypes[i] = endingFailSfxTypes[i];
                }

                Debug.Log("Ending Fail CutScene");
                break;
        }

        // 모든 컷씬 이미지를 숨기기
        foreach (var img in cutSceneImage)
        {
            img.color = new Color(1, 1, 1, 0); // 완전히 투명
            img.enabled = false;
        }

        currentCutSceneIndex = 0;
        isCutsceneFinished = false;

        textField.SetActive(false);

        // 첫 번째 컷씬 시작
        StartCoroutine(ShowCutscene(0, true));
    }

    public void NextButton()
    {
        // 클릭 시 다음 컷씬으로 이동
        if (!isCutsceneActive)
        {
            if (isCutsceneFinished)
            {
                EndCutscene(); // 종료 처리
            }
            else
            {
                NextCutscene(); // 다음 컷씬
            }
        }
    }

    private void NextCutscene()
    {
        // 배열 끝에 도달한 경우 종료 처리
        if (currentCutSceneIndex >= endCutSceneIndex)
        {
            isCutsceneFinished = true; // 종료 플래그 설정
            return;
        }

        switch (cutsceneIndex)
        {
            case 0:
                if (currentCutSceneIndex == 11)
                {
                    SoundManager.instance.PlayBGM(GameBGMType.Opening);
                }
                else if(currentCutSceneIndex == 12)
                {
                    SoundManager.instance.StopBGM();
                }

                if(currentCutSceneIndex >= 7 && currentCutSceneIndex <= 9)
                {

                }
                else
                {
                    SoundManager.instance.PlaySFX(GameSfxType.Opening_CS01);
                }

                break;
        }

        currentCutSceneIndex++;
        StartCoroutine(ShowCutscene(currentCutSceneIndex));
    }

    private IEnumerator ShowCutscene(int index, bool isFirst = false)
    {
        isCutsceneActive = true;

        if (cutSceneTexts[index].Length > 0)
        {
            textField.SetActive(true);

            tW_Regular.ORIGINAL_TEXT = LocalizationManager.instance.GetString(cutSceneTexts[index]);
            tW_Regular.StartTypewriter();
        }
        else
        {
            textField.SetActive(false);
        }

        if (cutSceneSpeakTypes[index] > 0)
        {
            if (cutSceneSpeakTypes[index] == 1)
            {
                SoundManager.instance.PlaySFX(GameSfxType.Dalddo);
            }
            else
            {
                SoundManager.instance.PlaySFX(GameSfxType.RamG);
            }
        }

        if (cutSceneSfxTypes[index].Length > 0)
        {
            GameSfxType sfxType = (GameSfxType)Enum.Parse(typeof(GameSfxType), cutSceneSfxTypes[index]);

            SoundManager.instance.PlaySFX(sfxType);
        }

        var img = cutSceneImage[index];
        img.enabled = true;

        float timer = 0f;

        // 페이드 인 효과
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            img.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        img.color = new Color(1, 1, 1, 1);

        if (isFirst)
        {
            // 첫 번째 컷씬에서 자동으로 다음 클릭을 대기
            yield return new WaitForSeconds(1f); // 첫 컷씬은 잠깐 기다림
        }

        isCutsceneActive = false;
    }


    private void EndCutscene()
    {
        // 모든 컷씬 이미지 끄기
        foreach (var img in cutSceneImage)
        {
            img.enabled = false;
        }

        isCutsceneFinished = true;

        view.enabled = false;

        SoundManager.instance.StopSFX();
        SoundManager.instance.PlayBGM(GameBGMType.Main);

        Debug.Log("Cutscene Finished!");

        if (cutsceneIndex == 0)
        {
            if (!GameStateManager.instance.First)
            {
                GameManager.instance.OpenTutorial();
            }
        }

        GameStateManager.instance.IsPlaying = true;
    }

    public void SkipCutscene()
    {
        // 즉시 모든 컷씬 종료
        StopAllCoroutines();
        EndCutscene();
        Debug.Log("Cutscene Skipped!");
    }
}
