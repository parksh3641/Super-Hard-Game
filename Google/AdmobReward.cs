#if  UNITY_ANDORID || UNITY_IOS
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine.Events;

public class AdmobReward : MonoBehaviour
{
    private string androidId = "ca-app-pub-6754544778509872/1383375604";
    private string iosId = "ca-app-pub-6754544778509872/3660449920";

    // �׽�Ʈ ���� ���� ID
    private string adUnitId = ""; // ���ۿ��� �����ϴ� �׽�Ʈ ���� ID

    private RewardedAd rewardedAd;

    void Start()
    {
#if UNITY_ANDORID
        adUnitId = androidId;
#elif UNITY_IOS
        adUnitId = iosId;
#endif

        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        // �ʱ�ȭ
        MobileAds.Initialize((InitializationStatus initStatus) => {
            Debug.Log("AdMob initialized for test ads");
        });
        LoadRewardedAd();
    }

    public void LoadRewardedAd()
    {
        // ���� ������ ������ ����
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }
        Debug.Log("Loading test rewarded ad.");
        // �׽�Ʈ ���� ��û ����
        AdRequest adRequest = new AdRequest.Builder().Build();
        // �׽�Ʈ ���� �ε�
        RewardedAd.Load(adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Test rewarded ad failed to load: " + error);
                    return;
                }
                Debug.Log("Test rewarded ad loaded successfully");
                rewardedAd = ad;
                // ���� �̺�Ʈ ���
                RegisterEventHandlers(ad);
            });
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        // ������ ���� �� �̺�Ʈ
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Ad closed, reloading...");
            LoadRewardedAd();
        };
        // ���� ǥ�� ���� �̺�Ʈ
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Ad failed to show: " + error);
            LoadRewardedAd();
        };
    }

    public void ShowAd(int number, Action onAdCompleted = null)
    {
        Debug.Log("Attempting to show test ad for reward type: " + number);
        
        if (Application.platform == RuntimePlatform.WindowsEditor || 
            Application.platform == RuntimePlatform.OSXEditor || 
            Application.platform == RuntimePlatform.WindowsPlayer)
        {
            GetReward(number);
            onAdCompleted?.Invoke();
            Debug.Log("Skipping interstitial ad in editor or Windows build");
            return;
        }

        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log("Test ad watched successfully! Rewarding player.");
                GetReward(number);
                // ���� ��û �Ϸ� �� �̺�Ʈ ȣ��
                onAdCompleted?.Invoke();
            });
        }
        else
        {
            Debug.LogWarning("Test ad not ready. Reloading...");
            LoadRewardedAd();
            // �ٷ� ���� ���� (�׽�Ʈ ����)
            GetReward(number);
            // ������ �غ���� �ʾ����� �׽�Ʈ �������� �̺�Ʈ ȣ��
            onAdCompleted?.Invoke();
        }
    }

    void GetReward(int number)
    {
        Debug.Log("Giving reward for type: " + number);
        switch (number)
        {
            case 0:
                break;
            default:
                Debug.Log("Unknown reward type: " + number);
                break;
        }
    }
}
#endif