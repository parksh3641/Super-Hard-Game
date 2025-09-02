#if  UNITY_ANDORID || UNITY_IOS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class AdmobBanner : MonoBehaviour
{
    // ���� �׽�Ʈ ��� ���� ID
    private string androidId = "ca-app-pub-6754544778509872/2233061454";
    private string iosId = "ca-app-pub-6754544778509872/8060103207";

    private string adUnitId = "";

    private BannerView _bannerView;

    public void Start()
    {
#if UNITY_ANDORID
        adUnitId = androidId;
#elif UNITY_IOS
        adUnitId = iosId;
#endif
        // �ʱ�ȭ
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            Debug.Log("AdMob initialized for test banner ads");
        });
    }

    public void LoadAd() // ���� �ε�
    {
        if (_bannerView == null)
        {
            CreateBannerView();
        }

        // �׽�Ʈ ���� ��û ����
        var adRequest = new AdRequest.Builder().Build();

        Debug.Log("Loading test banner ad.");
        _bannerView.LoadAd(adRequest);
    }

    public void CreateBannerView() // ���� �����ֱ�
    {
        Debug.Log("Creating test banner view");

        if (_bannerView != null)
        {
            DestroyAd();
        }

        // ������ ��� ������ ����
        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        // �׽�Ʈ ���� ID�� ��� ����
        _bannerView = new BannerView(adUnitId, adaptiveSize, AdPosition.Bottom);

        // �̺�Ʈ ������ ���
        ListenToAdEvents();
    }

    private void ListenToAdEvents()
    {
        // ���� �ε� ����
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Test banner view loaded successfully");
        };

        // ���� �ε� ����
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Test banner view failed to load: " + error);
        };

        // ���� Ŭ��
        _bannerView.OnAdClicked += () =>
        {
            Debug.Log("Test banner view was clicked.");
        };

        // ��ü ȭ�� ����
        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Test banner view full screen content opened.");
        };

        // ��ü ȭ�� ����
        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Test banner view full screen content closed.");
        };
    }

    public void DestroyAd() // ���� ����
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying test banner ad.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }

    private void OnDisable()
    {
        DestroyAd();
    }
}
#endif