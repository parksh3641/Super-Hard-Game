using UnityEngine;

public class NetworkConnect : MonoBehaviour
{
    public static NetworkConnect instance;

    // public static bool isConnect = false;
    public bool isConnect = false;

    void Awake()
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
    }

    public bool CheckConnectInternet()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            isConnect = false;
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            isConnect = true;
        }
        else
        {
            isConnect = true;
        }
        return isConnect;
    }
}