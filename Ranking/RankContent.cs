using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankContent : MonoBehaviour
{
    public Image main;
    public Image indexImage;
    public TMP_Text indexText;
    public TMP_Text nickNameText;
    public TMP_Text scoreText;

    public GameObject selected;

    private void Awake()
    {
        indexText.text = "";
        nickNameText.text = "";
    }

    public void Initialize(int index, string nickName, int score, bool checkMy) //순위, 닉네임, 점수, 내 자신일 경우 테두리
    {
        if(index == 1)
        {
            indexImage.enabled = true;
            indexText.text = "";
        }
        else
        {
            indexImage.enabled = false;
            indexText.text = index.ToString();
        }

        main.color = Color.white;

        nickNameText.text = nickName;
        scoreText.text = TimeConverter.ConvertMillisecondsToTime(score);

        if(checkMy)
        {
            main.color = new Color(1, 200f / 255f, 0);

            if(score == 10000000)
            {
                indexText.text = "-";
            }
        }

        //selected.SetActive(checkMy);
    }
}
