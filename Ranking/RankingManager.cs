using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankingManager : MonoBehaviour
{
    public static RankingManager instance;

    public Canvas rankingView;

    public RankContent rankContentPrefab;
    public RankContent myRankContent;
    public RectTransform rankContentParent;

    List<RankContent> rankContentList = new List<RankContent>();

    public Image[] topMenuArray;
    public Sprite[] topMenuSprite;

    private int index = -1;
    private int score = 0;
    private bool isDelay = false;

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

        rankingView.enabled = false;

        for (int i = 0; i < 100; i++)
        {
            RankContent monster = Instantiate(rankContentPrefab);
            monster.name = "RankContent_" + i;
            monster.transform.position = Vector3.zero;
            monster.transform.localScale = Vector3.one;
            monster.transform.SetParent(rankContentParent);
            monster.gameObject.SetActive(false);

            rankContentList.Add(monster);
        }

        rankContentParent.anchoredPosition = new Vector2(0, -9999);
    }

    public void OpenRankingView()
    {
        rankingView.enabled = true;

        if(index == -1)
        {
            ChangeTopToggle(1);
        }

        SoundManager.instance.PlaySFX(GameSfxType.OpenRanking);

        Debug.Log("랭킹 열기");
    }

    public void CloseRankingView()
    {
        if (isDelay) return;

        rankingView.enabled = false;
    }

    public void ChangeTopToggle(int number)
    {
        if (index == number) return;
        if (isDelay) return;

        SoundManager.instance.PlaySFX(GameSfxType.OpenPopup);

        isDelay = true;
        index = number;

        for(int i =0; i < topMenuArray.Length; i ++)
        {
            topMenuArray[i].sprite = topMenuSprite[0];
        }

        topMenuArray[number].sprite = topMenuSprite[1];

        switch (number)
        {
            case 0:
                PlayfabManager.instance.GetLeaderboarder("TotalTimeAttack", 0, SetRanking);
                break;
            case 1:
                PlayfabManager.instance.GetLeaderboarder("TimeAttackStage1", 0, SetRanking);
                break;
            case 2:
                PlayfabManager.instance.GetLeaderboarder("TimeAttackStage2", 0, SetRanking);
                break;
            case 3:
                PlayfabManager.instance.GetLeaderboarder("TimeAttackStage3", 0, SetRanking);
                break;
            case 4:
                PlayfabManager.instance.GetLeaderboarder("TimeAttackStage4", 0, SetRanking);
                break;
            case 5:
                PlayfabManager.instance.GetLeaderboarder("TimeAttackStage5", 0, SetRanking);
                break;
            case 6:
                PlayfabManager.instance.GetLeaderboarder("TimeAttackStage6", 0, SetRanking);
                break;
            case 7:
                PlayfabManager.instance.GetLeaderboarder("TimeAttackStage7", 0, SetRanking);
                break;
            case 8:
                PlayfabManager.instance.GetLeaderboarder("TimeAttackStage8", 0, SetRanking);
                break;
        }
    }

    public void SetRanking(GetLeaderboardResult result) //받아온 정보를 정리합니다
    {
        int index = 1;
        bool isMine = false;
        string nickName = "";

        var curBoard = result.Leaderboard;
        List<(int score, string nickName, bool isMine)> sortedList = new List<(int, string, bool)>();

        for (int i = 0; i < rankContentList.Count; i++)
        {
            rankContentList[i].transform.localScale = Vector3.one;
            rankContentList[i].gameObject.SetActive(false);
        }

        foreach (PlayerLeaderboardEntry player in curBoard)
        {
            isMine = false;
            int score = player.StatValue != 0 ? player.StatValue : 0;
            nickName = player.DisplayName ?? player.PlayFabId;

            if (player.PlayFabId.Equals(GameStateManager.instance.CustomId) ||
                (player.DisplayName != null && player.DisplayName.Equals(GameStateManager.instance.NickName)))
            {
                isMine = true;
            }

            sortedList.Add((score, nickName, isMine));
        }

        // 점수가 10000000인 경우 맨 아래로, 나머지는 내림차순 정렬
        sortedList = sortedList
            .OrderBy(entry => entry.score == 10000000 ? 1 : 0) // 10000000이면 마지막으로 보내기
            .ThenByDescending(entry => entry.score) // 나머지는 내림차순 정렬
            .ToList();

        int num = 0;
        foreach (var entry in sortedList)
        {
            if (entry.isMine)
            {
                myRankContent.Initialize(index, entry.nickName, entry.score, true);
            }
            rankContentList[num].Initialize(index, entry.nickName, entry.score, entry.isMine);
            rankContentList[num].gameObject.SetActive(true);

            index++;
            num++;
        }

        rankContentParent.anchoredPosition = new Vector2(0, -9999);
        isDelay = false;
    }
}
