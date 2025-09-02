#if  UNITY_ANDORID || UNITY_IOS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using System;

public class AdmobScreen : MonoBehaviour
{
    private string androidId = "ca-app-pub-6754544778509872/3388987180";
    private string iosId = "ca-app-pub-6754544778509872/4973531592";

    // ���� �׽�Ʈ ���� ���� ID
    private string adUnitId = "";

    private InterstitialAd interstitialAd;

    public void Start()
    {
#if UNITY_ANDORID
        adUnitId = androidId;
#elif UNITY_IOS
        adUnitId = iosId;
#endif

        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            Debug.Log("AdMob initialized for test interstitial ads");
            // �ʱ�ȭ �Ϸ� �� �ٷ� �ε�
            LoadInterstitialAd();
        });
    }

    public void LoadInterstitialAd() // ���� �ε�
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        Debug.Log("Loading the test interstitial ad.");

        // �׽�Ʈ ���� ��û ����
        var adRequest = new AdRequest.Builder().Build();

        InterstitialAd.Load(adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Test interstitial ad failed to load: " + error);
                    return;
                }

                Debug.Log("Test interstitial ad loaded successfully");
                interstitialAd = ad;

                // �ε�� ���� �̺�Ʈ �ڵ鷯 ���
                RegisterEventHandlers(interstitialAd);
            });
    }

    public void ShowAd(int number = 0, Action onAdCompleted = null) // ���� ����
    {
        Debug.Log("Attempting to show interstitial ad for reward type: " + number);

        // �����Ϳ����� ���� ǥ�� ���� �Ѿ
        if (Application.platform == RuntimePlatform.WindowsEditor || 
            Application.platform == RuntimePlatform.OSXEditor || 
            Application.platform == RuntimePlatform.WindowsPlayer)
        {
            GetReward(number);
            onAdCompleted?.Invoke();
            Debug.Log("Skipping interstitial ad in editor");
            return;
        }

        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            interstitialAd.Show();

            // ���� �Ϸ� �� ���� �� �ݹ� ó��
            GetReward(number);
            onAdCompleted?.Invoke();
        }
        else
        {
            Debug.LogWarning("Interstitial ad is not ready yet. Reloading...");
            LoadInterstitialAd();

            // ���� �غ���� �ʾ����� �׽�Ʈ �������� ���� ���� �� �̺�Ʈ ȣ��
            GetReward(number);
            onAdCompleted?.Invoke();
        }
    }

    // GetReward �޼��尡 ���ٸ� �߰��ؾ� �մϴ�
    private void GetReward(int rewardType)
    {
        // ���� Ÿ�Կ� ���� ó��
        switch (rewardType)
        {
            case 0:
                // �⺻ ���� �Ǵ� ���� ����
                break;
            case 1:
                // ���� Ÿ�� 1 ó��
                break;
            case 2:
                // ���� Ÿ�� 2 ó��
                break;
            // �߰� ���� Ÿ�Ե�...
            default:
                Debug.Log("Unknown reward type: " + rewardType);
                break;
        }
    }

    private void RegisterEventHandlers(InterstitialAd ad) // ���� �̺�Ʈ
    {
        // ������ ���� null üũ�� �����ϰ� ó��
        if (ad == null) return;

        // ���� ���� ��
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Test interstitial ad recorded an impression.");
        };

        // ���� Ŭ�� ��
        ad.OnAdClicked += () =>
        {
            Debug.Log("Test interstitial ad was clicked.");
        };

        // ���� ��ü ȭ�� ����
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Test interstitial ad full screen content opened.");
        };

        // ���� ��ü ȭ�� ���� - �� ���� �ڵ� �ε�
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Test interstitial ad full screen content closed. Reloading...");
            LoadInterstitialAd();
        };

        // ���� ��ü ȭ�� ǥ�� ����
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Test interstitial ad failed to open: " + error);
            // ���� �� ��ε�
            LoadInterstitialAd();
        };
    }

    private void OnDestroy()
    {
        // ���ҽ� ����
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }
    }
}
#endif