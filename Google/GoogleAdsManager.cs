#if  UNITY_ANDORID || UNITY_IOS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoogleAdsManager : MonoBehaviour
{
    public static GoogleAdsManager instance;

    public AdmobBanner banner;
    public AdmobReward reward;
    public AdmobScreen screen;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
#endif