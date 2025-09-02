using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageTimeAttack
{

}

[CreateAssetMenu(fileName = "PlayerDataBase", menuName = "ScriptableObjects/PlayerDataBase")]
public class PlayerDataBase : ScriptableObject
{
    [Header("Player")]
    [SerializeField] private int stage = 0;

    [Header("Record")]
    [SerializeField] private int totalTimeAttack = 0;
    [SerializeField] private int timeAttackStage1 = 0;
    [SerializeField] private int timeAttackStage2 = 0;
    [SerializeField] private int timeAttackStage3 = 0;
    [SerializeField] private int timeAttackStage4 = 0;
    [SerializeField] private int timeAttackStage5 = 0;
    [SerializeField] private int timeAttackStage6 = 0;
    [SerializeField] private int timeAttackStage7 = 0;
    [SerializeField] private int timeAttackStage8 = 0;
    [SerializeField] private int timeAttackStage9 = 0;
    [SerializeField] private int timeAttackStage10 = 0;

    [Header("Data")]
    [SerializeField] private int countOutLoadDead = 0;
    [SerializeField] private int countMissileDead = 0;
    [SerializeField] private int countLaserDead = 0;
    [SerializeField] private int countUFODead = 0;
    [SerializeField] private int countGameOver = 0;
    [SerializeField] private int countUnknown = 0;
    [SerializeField] private int playTime = 0;
    [SerializeField] private int nickName = 0;
    [SerializeField] private int developer = 0;

    public int Stage
    {
        get { return stage; }
        set { stage = value; }
    }

    public int TotalTimeAttack
    {
        get { return totalTimeAttack; }
        set { totalTimeAttack = value; }
    }

    public int TimeAttackStage1
    {
        get { return timeAttackStage1; }
        set { timeAttackStage1 = value; }
    }

    public int TimeAttackStage2
    {
        get { return timeAttackStage2; }
        set { timeAttackStage2 = value; }
    }
    public int TimeAttackStage3
    {
        get { return timeAttackStage3; }
        set { timeAttackStage3 = value; }
    }
    public int TimeAttackStage4
    {
        get { return timeAttackStage4; }
        set { timeAttackStage4 = value; }
    }
    public int TimeAttackStage5
    {
        get { return timeAttackStage5; }
        set { timeAttackStage5 = value; }
    }
    public int TimeAttackStage6
    {
        get { return timeAttackStage6; }
        set { timeAttackStage6 = value; }
    }
    public int TimeAttackStage7
    {
        get { return timeAttackStage7; }
        set { timeAttackStage7 = value; }
    }
    public int TimeAttackStage8
    {
        get { return timeAttackStage8; }
        set { timeAttackStage8 = value; }
    }
    public int TimeAttackStage9
    {
        get { return timeAttackStage9; }
        set { timeAttackStage9 = value; }
    }
    public int TimeAttackStage10
    {
        get { return timeAttackStage10; }
        set { timeAttackStage10 = value; }
    }

    public int CountOutLoadDead
    {
        get { return countOutLoadDead; }
        set { countOutLoadDead = value; }
    }

    public int CountMissileDead
    {
        get { return countMissileDead; }
        set { countMissileDead = value; }
    }

    public int CountLaserDead
    {
        get { return countLaserDead; }
        set { countLaserDead = value; }
    }

    public int CountUFODead
    {
        get { return countUFODead; }
        set { countUFODead = value; }
    }

    public int CountGameOver
    {
        get { return countGameOver; }
        set { countGameOver = value; }
    }

    public int CountUnknown
    {
        get { return countUnknown; }
        set { countUnknown = value; }
    }

    public int PlayTime
    {
        get { return playTime; }
        set { playTime = value; }
    }

    public int NickName
    {
        get { return nickName; }
        set { nickName = value; }
    }

    public int Developer
    {
        get { return developer; }
        set { developer = value; }
    }

    public void Initialize()
    {
        stage = 0;
        totalTimeAttack = 10000000;
        timeAttackStage1 = 10000000;
        timeAttackStage2 = 10000000;
        timeAttackStage3 = 10000000;
        timeAttackStage4 = 10000000;
        timeAttackStage5 = 10000000;
        timeAttackStage6 = 10000000;
        timeAttackStage7 = 10000000;
        timeAttackStage8 = 10000000;
        timeAttackStage9 = 10000000;
        timeAttackStage10 = 10000000;

        countOutLoadDead = 0;
        countMissileDead = 0;
        countLaserDead = 0;
        countUFODead = 0;
        countGameOver = 0;
        countUnknown = 0;
        playTime = 0;
        nickName = 0;
        developer = 0;

        Debug.Log("데이터베이스 초기화 완료");
    }

    public int GetTotalTimeAttack()
    {
        return (10000000 - timeAttackStage1) + (10000000 - timeAttackStage2) + (10000000 - timeAttackStage3) +
            (10000000 - timeAttackStage4) + (10000000 - timeAttackStage5) + (10000000 - timeAttackStage6) +
            (10000000 - timeAttackStage7) + (10000000 - timeAttackStage8) + (10000000 - timeAttackStage9) + (10000000 - timeAttackStage10);
    }

    public int GetTimeAttack(int number)
    {
        int time = 0;

        switch(number)
        {
            case 0:
                time = timeAttackStage1;
                break;
            case 1:
                time = timeAttackStage2;
                break;
            case 2:
                time = timeAttackStage3;
                break;
            case 3:
                time = timeAttackStage4;
                break;
            case 4:
                time = timeAttackStage5;
                break;
            case 5:
                time = timeAttackStage6;
                break;
            case 6:
                time = timeAttackStage7;
                break;
            case 7:
                time = timeAttackStage8;
                break;
            case 8:
                time = timeAttackStage9;
                break;
            case 9:
                time = timeAttackStage10;
                break;
        }

        return time;
    }
}