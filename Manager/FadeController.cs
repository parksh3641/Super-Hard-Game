using System.Collections;
using UnityEngine;

public class FadeController : MonoBehaviour
{
    public GameObject fade;
    public Material fadeMaterial; // ������ ��Ƽ����
    public float fadeDuration = 2f; // ���̵� ȿ�� ���� �ð�

    private Coroutine fadeCoroutine; // �ڷ�ƾ ����� ����

    void Start()
    {
        fade.SetActive(true);

        // ���� �� �ʱ�ȭ (optional)
        if (fadeMaterial != null)
        {
            fadeMaterial.SetFloat("_Radius", 0f);
        }

        StartFade();
    }

    public void StartFade()
    {
        // �̹� ���� ���� �ڷ�ƾ�� ������ ����
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        // �� �ڷ�ƾ ����
        fadeCoroutine = StartCoroutine(FadeRadius(0f, 1f));
    }

    private IEnumerator FadeRadius(float startValue, float endValue)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration); // 0~1 ���̷� ����
            float currentValue = Mathf.Lerp(startValue, endValue, t);

            if (fadeMaterial != null)
            {
                fadeMaterial.SetFloat("_Radius", currentValue); // ���̴��� _Radius �� ����
            }

            yield return null; // ���� �����ӱ��� ���
        }

        // ������ �� Ȯ���� ����
        if (fadeMaterial != null)
        {
            fadeMaterial.SetFloat("_Radius", endValue);
        }

        fade.SetActive(false);
    }
}
