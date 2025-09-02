using UnityEngine;

public class WindowResize : MonoBehaviour
{
    private int lastWidth;
    private int lastHeight;
    private const float aspectRatio = 16f / 9f;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        lastWidth = Screen.width;
        lastHeight = Screen.height;
    }

    void Update()
    {
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            int newWidth = Screen.width;
            int newHeight = Mathf.RoundToInt(newWidth / aspectRatio);

            if (newHeight > Screen.height)
            {
                newHeight = Screen.height;
                newWidth = Mathf.RoundToInt(newHeight * aspectRatio);
            }

            Screen.SetResolution(newWidth, newHeight, false);

            lastWidth = newWidth;
            lastHeight = newHeight;
        }
    }
}
