using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumManager : MonoBehaviour
{

}

public enum LanguageType
{
    Default = 0,
    Korean,
    English,
    Japanese,
    Chinese,
    Indian,
    Portuguese,
    Russian,
    German,
    Spanish,
    Arabic,
    Bengali,
    Indonesian,
    Italian,
    Dutch,
    Vietnamese,
    Thai
}

public enum LoginType
{
    None = 0,
    Guest,
    Stove,
    Steam
}

public enum GameBGMType
{
    Splash,
    Main,
    Opening
}

public enum GameSfxType
{
    Click,
    Bomb,
    Title,
    GameStart,
    GameOver,
    GameClear,
    Laser,
    UFO,
    Chat,
    LaserDead,
    Dalddo,
    RamG,
    Opening_CS01,
    Opening_E01,
    Opening_E02,
    Opening_E03,
    Opening_E04,
    Opening_ES01,
    OpenRanking,
    OpenPopup,
    UFODead,
    MissileDead,
    OutLoadDead,
    Warning,
    SelectFail
}

public enum ScreenMode
{
    FullScreen,
    Windowed,
}

public enum Resolution
{
    FHD_1920x1080,
    HD_1600x900,
    HD_1280x720,
}