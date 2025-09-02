using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationDataBase_Image", menuName = "ScriptableObjects/LocalizationDataBase_Image")]
public class LocalizationDataBase_Image : ScriptableObject
{
    [Header("Title")]
    public Sprite[] title;

    [Header("GameClear")]
    public Sprite[] gameClear;

    [Header("GameOver")]
    public Sprite[] gameOver;

    public Sprite[] GetTitleImage()
    {
        return title;
    }

    public Sprite[] GetGameClearImage()
    {
        return gameClear;
    }

    public Sprite[] GetGameOverImage()
    {
        return gameOver;
    }


    public Sprite[] GetImage(string name)
    {
        Sprite[] sprite = new Sprite[title.Length];

        switch(name)
        {
            case "Title":
                sprite = GetTitleImage();
                break;
            case "GameClear":
                sprite = GetGameClearImage();
                break;
            case "GameOver":
                sprite = GetGameOverImage();
                break;
        }

        return sprite;
    }
}
