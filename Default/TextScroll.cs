using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextScroll : MonoBehaviour
{
    RectTransform main;

    public GameObject credit;

    public LocalizationContent creditText;

    public float scrollSpeed = 50f;
    public float startY = -1100f;  
    public float endY = 1000f;

    private void Awake()
    {
        main = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        StartCredits();
    }

    public void StartCredits()
    {
        creditText.localizationName = "Credit_Info";
        creditText.ReLoad();

        main.anchoredPosition = new Vector2(main.anchoredPosition.x, startY);
        StartCoroutine(ScrollCredits());
    }

    private IEnumerator ScrollCredits()
    {
        while (main.anchoredPosition.y < endY)
        {
            main.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
            yield return null;
        }

        credit.SetActive(false);
    }
}
