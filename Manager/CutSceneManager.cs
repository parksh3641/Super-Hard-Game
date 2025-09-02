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
    public Sprite[] openingImages; // �ƾ� �̹��� �迭
    public string[] openingTexts;
    public int[] openingSpeakTypes; //���� ���ϴ� ������?
    public string[] openingSfxTypes; //����Ʈ ���

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
    [Header("Ending_Fail")] //������ �������� ����
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

    [SerializeField] private int cutsceneIndex = 0; //� ������ �ƾ�����

    [SerializeField] private float fadeDuration = 1f; // ���̵� ���� �ð�
    [SerializeField] private int currentCutSceneIndex = 0; // ���� �ƾ� �ε���
    [SerializeField] private int endCutSceneIndex = 0;

    private bool isCutsceneActive = false;
    private bool isCutsceneFinished = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �� �̵� �Ŀ��� ����
        }
        else
        {
            Destroy(gameObject); // ���� �ν��Ͻ��� ������ �� ��ü �ı�
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

        // ��� �ƾ� �̹����� �����
        foreach (var img in cutSceneImage)
        {
            img.color = new Color(1, 1, 1, 0); // ������ ����
            img.enabled = false;
        }

        currentCutSceneIndex = 0;
        isCutsceneFinished = false;

        textField.SetActive(false);

        // ù ��° �ƾ� ����
        StartCoroutine(ShowCutscene(0, true));
    }

    public void NextButton()
    {
        // Ŭ�� �� ���� �ƾ����� �̵�
        if (!isCutsceneActive)
        {
            if (isCutsceneFinished)
            {
                EndCutscene(); // ���� ó��
            }
            else
            {
                NextCutscene(); // ���� �ƾ�
            }
        }
    }

    private void NextCutscene()
    {
        // �迭 ���� ������ ��� ���� ó��
        if (currentCutSceneIndex >= endCutSceneIndex)
        {
            isCutsceneFinished = true; // ���� �÷��� ����
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

        // ���̵� �� ȿ��
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
            // ù ��° �ƾ����� �ڵ����� ���� Ŭ���� ���
            yield return new WaitForSeconds(1f); // ù �ƾ��� ��� ��ٸ�
        }

        isCutsceneActive = false;
    }


    private void EndCutscene()
    {
        // ��� �ƾ� �̹��� ����
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
        // ��� ��� �ƾ� ����
        StopAllCoroutines();
        EndCutscene();
        Debug.Log("Cutscene Skipped!");
    }
}
