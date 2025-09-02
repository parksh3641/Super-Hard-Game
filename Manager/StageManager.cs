using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager instance;

    public Canvas view;

    public StageContent stageContentPrefab;
    public RectTransform stageContentTransform;

    public List<StageContent> stageContentList = new List<StageContent>();

    PlayerDataBase playerDataBase;

    string stageFolderPath = Path.Combine(Application.streamingAssetsPath, "Stage");
    string searchPattern = "*.bin"; // 파일 이름 패턴
    int fileCount;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 이동 후에도 유지

            if (playerDataBase == null) playerDataBase = Resources.Load("PlayerDataBase") as PlayerDataBase;

            fileCount = CountStageFiles(stageFolderPath, searchPattern);
            Debug.Log($"Number of stage data files: {fileCount}");

            for (int i = 0; i < fileCount; i ++)
            {
                StageContent stageContent = Instantiate(stageContentPrefab, transform.position, Quaternion.identity, transform);
                stageContent.transform.SetParent(stageContentTransform);
                stageContent.transform.localScale = Vector3.one;
                stageContent.gameObject.SetActive(false);
                stageContentList.Add(stageContent);
            }
        }
        else
        {
            Destroy(gameObject); // 기존 인스턴스가 있으면 새 객체 파괴
        }

        view.enabled = false;
    }

    int CountStageFiles(string folderPath, string pattern)
    {
        try
        {
            // 해당 폴더에서 패턴에 맞는 파일 목록 검색
            string[] files = Directory.GetFiles(folderPath, pattern);
            return files.Length;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error counting files: {e.Message}");
            return 0;
        }
    }

    public void OpenView()
    {
        if (!view.enabled)
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
        for (int i = 0; i < stageContentList.Count; i++)
        {
            stageContentList[i].gameObject.SetActive(true);
            stageContentList[i].Initialize(i, playerDataBase.Stage, playerDataBase.GetTimeAttack(i), this);
        }
    }

    public void GoToStage(int index)
    {
        PlayerPrefs.SetInt("Stage", index);
        
        Debug.Log("스테이지 선택 : " + index);

        if (index != 0)
        {
#if UNITY_ANDORID || UNITY_IOS && !UNITY_EDITOR
            GoogleAdsManager.instance.reward.ShowAd(0, SuccessWatchAd);
#else
            SuccessWatchAd();
#endif
        }
        else
        {
            SuccessWatchAd();
        }
    }

    public void SuccessWatchAd()
    {
        OpenView();

        SceneManager.LoadScene("LoadingScene");
    }
}
