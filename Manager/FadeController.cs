using System.Collections;
using UnityEngine;

public class FadeController : MonoBehaviour
{
    public GameObject fade;
    public Material fadeMaterial; // 적용할 머티리얼
    public float fadeDuration = 2f; // 페이드 효과 지속 시간

    private Coroutine fadeCoroutine; // 코루틴 저장용 변수

    void Start()
    {
        fade.SetActive(true);

        // 시작 시 초기화 (optional)
        if (fadeMaterial != null)
        {
            fadeMaterial.SetFloat("_Radius", 0f);
        }

        StartFade();
    }

    public void StartFade()
    {
        // 이미 실행 중인 코루틴이 있으면 중지
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        // 새 코루틴 시작
        fadeCoroutine = StartCoroutine(FadeRadius(0f, 1f));
    }

    private IEnumerator FadeRadius(float startValue, float endValue)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration); // 0~1 사이로 제한
            float currentValue = Mathf.Lerp(startValue, endValue, t);

            if (fadeMaterial != null)
            {
                fadeMaterial.SetFloat("_Radius", currentValue); // 쉐이더의 _Radius 값 설정
            }

            yield return null; // 다음 프레임까지 대기
        }

        // 마지막 값 확실히 설정
        if (fadeMaterial != null)
        {
            fadeMaterial.SetFloat("_Radius", endValue);
        }

        fade.SetActive(false);
    }
}
