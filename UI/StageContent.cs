using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageContent : MonoBehaviour
{
    public int index = 0;

    public LocalizationContent stageNameText;
    public TMP_Text bestTimeText;

    public GameObject lockedObj;

    StageManager stageManager;

    private void Awake()
    {
        
    }

    public void Initialize(int number, int maxStage, int time, StageManager manager)
    {
        index = number;
        stageManager = manager;

        stageNameText.localizationName = "Stage";
        stageNameText.plusText = " " + (index + 1);
        stageNameText.ReLoad();
        bestTimeText.text = TimeConverter.ConvertMillisecondsToTime(time);

        lockedObj.SetActive(true);

        if (index <= maxStage)
        {
            lockedObj.SetActive(false);
        }
    }

    public void OnClick()
    {
        stageManager.GoToStage(index);
    }
}
