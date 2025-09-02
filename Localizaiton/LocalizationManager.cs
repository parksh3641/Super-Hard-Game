using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.UI;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager instance;

    public TMP_FontAsset normalFontAsset; // 한국어, 영어 등
    public TMP_FontAsset jpFontAsset; // 일본어, 중국어
    public TMP_FontAsset inFontAsset; // 인도어

    public LocalizationDataBase localizationDataBase;

    public List<LocalizationContent> localizationContentList = new List<LocalizationContent>();
    public List<LocalizationContent_Image> localizationContentList_Image = new List<LocalizationContent_Image>();

    private static readonly string key = "MySecretKey12345"; // AES 암호화 키

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (localizationDataBase == null) localizationDataBase = Resources.Load("LocalizationDataBase") as LocalizationDataBase;

        localizationContentList.Clear();
        localizationDataBase.Initialize();

        string path = Path.Combine(Application.streamingAssetsPath, "Localization.bin");

        if (File.Exists(path))
        {
            string decryptedText = DecryptFile(path);
            if (!string.IsNullOrEmpty(decryptedText))
            {
                TextAsset textAsset = new TextAsset(decryptedText);
                SetLocalization(textAsset.ToString());
                Debug.Log("Localization 데이터 복호화 완료");
            }
            else
            {
                Debug.LogError("복호화된 Localization 데이터가 없습니다.");
            }
        }
        else
        {
            Debug.LogError("Localization 파일을 찾을 수 없습니다: " + path);
        }
    }

    private string DecryptFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"파일이 존재하지 않습니다: {filePath}");
            return null;
        }

        try
        {
            byte[] encryptedData = File.ReadAllBytes(filePath);
            return Decrypt(encryptedData);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"파일 복호화 중 오류 발생: {e.Message}");
            return null;
        }
    }

    private string Decrypt(byte[] cipherText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = new byte[16];

            using (MemoryStream ms = new MemoryStream(cipherText))
            using (CryptoStream cs = new CryptoStream(ms, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
            using (StreamReader sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }

    void SetLocalization(string tsv)
    {
        string[] row = tsv.Split('\n');
        int rowSize = row.Length;

        for (int i = 1; i < rowSize; i++)
        {
            string[] column = row[i].Split('\t');
            if (column.Length < 17) continue; // 데이터가 부족할 경우 건너뜀

            LocalizationData content = new LocalizationData
            {
                key = column[0],
                korean = column[1].Replace('#', '\n'),
                english = column[2].Replace('#', '\n'),
                japanese = column[3].Replace('#', '\n'),
                chinese = column[4].Replace('#', '\n'),
                indian = column[5].Replace('#', '\n'),
                portuguese = column[6].Replace('#', '\n'),
                russian = column[7].Replace('#', '\n'),
                german = column[8].Replace('#', '\n'),
                spanish = column[9].Replace('#', '\n'),
                arabic = column[10].Replace('#', '\n'),
                bengali = column[11].Replace('#', '\n'),
                indonesian = column[12].Replace('#', '\n'),
                italian = column[13].Replace('#', '\n'),
                dutch = column[14].Replace('#', '\n'),
                vietnamese = column[15].Replace('#', '\n'),
                thai = column[16].Replace('#', '\n')
            };

            localizationDataBase.SetLocalization(content);
        }
    }
    public void AddContent(LocalizationContent content)
    {
        localizationContentList.RemoveAll(item => item == null);

        if (!localizationContentList.Contains(content))
        {
            localizationContentList.Add(content);
            ChangeFont();
        }
    }

    public void AddContent_Image(LocalizationContent_Image content)
    {
        localizationContentList_Image.RemoveAll(item => item == null);

        if (!localizationContentList_Image.Contains(content))
        {
            localizationContentList_Image.Add(content);
        }
    }

    public string GetString(string name)
    {
        string str = "";

        foreach (var item in localizationDataBase.localizationDatas)
        {
            if (name.Equals(item.key))
            {
                switch (GameStateManager.instance.Language)
                {
                    case LanguageType.Korean:
                        str = item.korean;
                        break;
                    case LanguageType.English:
                        str = item.english;
                        break;
                    case LanguageType.Japanese:
                        str = item.japanese;
                        break;
                    case LanguageType.Chinese:
                        str = item.chinese;
                        break;
                    case LanguageType.Indian:
                        str = item.indian;
                        break;
                    case LanguageType.Portuguese:
                        str = item.portuguese;
                        break;
                    case LanguageType.Russian:
                        str = item.russian;
                        break;
                    case LanguageType.German:
                        str = item.german;
                        break;
                    case LanguageType.Spanish:
                        str = item.spanish;
                        break;
                    case LanguageType.Arabic:
                        str = item.arabic;
                        break;
                    case LanguageType.Bengali:
                        str = item.bengali;
                        break;
                    case LanguageType.Indonesian:
                        str = item.indonesian;
                        break;
                    case LanguageType.Italian:
                        str = item.italian;
                        break;
                    case LanguageType.Dutch:
                        str = item.dutch;
                        break;
                    case LanguageType.Vietnamese:
                        str = item.vietnamese;
                        break;
                    case LanguageType.Thai:
                        str = item.thai;
                        break;
                }
            }
        }

        if (str.Length == 0 || str.Equals("#VALUE!"))
        {
            str = name;
        }

        return str;
    }

    public void ChangeLanguage()
    {
        switch (GameStateManager.instance.Language)
        {
            case LanguageType.Korean:
                ChangeEnglish();
                break;
            case LanguageType.English:
                ChangeJapanese();
                break;
            case LanguageType.Japanese:
                ChangeChinese();
                break;
            case LanguageType.Chinese:
                ChangeIndian();
                break;
            case LanguageType.Indian:
                ChangePortuguese();
                break;
            case LanguageType.Portuguese:
                ChangeRussian();
                break;
            case LanguageType.Russian:
                ChangeGerman();
                break;
            case LanguageType.German:
                ChangeSpanish();
                break;
            case LanguageType.Spanish:
                ChangeArabic();
                break;
            case LanguageType.Arabic:
                ChangeBengali();
                break;
            case LanguageType.Bengali:
                ChangeIndonesian();
                break;
            case LanguageType.Indonesian:
                ChangeItalian();
                break;
            case LanguageType.Italian:
                ChangeDutch();
                break;
            case LanguageType.Dutch:
                ChangeVietnamese();
                break;
            case LanguageType.Vietnamese:
                ChangeThai();
                break;
            case LanguageType.Thai:
                ChangeKorean();
                break;
        }

        //if (country != null) country.sprite = countryArray[(int)GameStateManager.instance.Language - 1];
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
        GameStateManager.instance.Language = type;

        ChangeFont();

        for (int i = 0; i < localizationContentList.Count; i++)
        {
            if(localizationContentList[i] != null)
            {
                localizationContentList[i].ReLoad();
            }
        }

        for (int i = 0; i < localizationContentList_Image.Count; i++)
        {
            if(localizationContentList_Image[i] != null)
            {
                localizationContentList_Image[i].ReLoad();
            }
        }

        string iso = "";

        switch (type)
        {
            case LanguageType.Korean:
                iso = "ko";
                break;
            //case LanguageType.English:
            //    iso = "en";
            //    break;
            //case LanguageType.Japanese:
            //    iso = "ja";
            //    break;
            default:
                iso = "en";
                break;
        }

        //PlayfabManager.instance.SetProfileLanguage(iso);

        Debug.Log("Change Language : " + type);
    }

    public void ChangeFont()
    {
        for (int i = 0; i < localizationContentList.Count; i++)
        {
            if (localizationContentList[i] == null) continue;

            switch (GameStateManager.instance.Language)
            {
                case LanguageType.Korean:
                    localizationContentList[i].localizationTMPText.font = normalFontAsset;
                    break;
                case LanguageType.English:
                    localizationContentList[i].localizationTMPText.font = normalFontAsset;
                    break;
                case LanguageType.Japanese:
                    localizationContentList[i].localizationTMPText.font = jpFontAsset;
                    break;
                case LanguageType.Chinese:
                    localizationContentList[i].localizationTMPText.font = jpFontAsset;
                    break;
                case LanguageType.Indian:
                    localizationContentList[i].localizationTMPText.font = inFontAsset;
                    break;
                case LanguageType.Portuguese:
                    break;
                case LanguageType.Russian:
                    break;
                case LanguageType.German:
                    break;
                case LanguageType.Spanish:
                    break;
                case LanguageType.Arabic:
                    break;
                case LanguageType.Bengali:
                    break;
                case LanguageType.Indonesian:
                    break;
                case LanguageType.Italian:
                    break;
                case LanguageType.Dutch:
                    break;
                case LanguageType.Vietnamese:
                    break;
                case LanguageType.Thai:
                    break;
            }
        }
    }
}