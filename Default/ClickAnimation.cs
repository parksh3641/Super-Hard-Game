using System.Collections;
using UnityEngine;

public class ClickAnimation : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // 스프라이트를 표시할 SpriteRenderer
    public float totalDuration = 1.5f;    // 전체 알파 변화 시간
    public float delayBeforeFade = 0.2f;  // 사라지기 전 대기 시간

    void OnEnable()
    {
        StopAllCoroutines();
        SetAlpha(1f); // 등장 시 alpha 1로 초기화
        StartCoroutine(FadeOut());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator FadeOut()
    {
        // 0.2초 동안 alpha 값을 유지
        yield return new WaitForSeconds(delayBeforeFade);

        float fadeDuration = totalDuration - delayBeforeFade; // 실제 alpha 변화 시간
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            SetAlpha(alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SetAlpha(0f); // 최종적으로 완전히 투명하게 설정
    }

    void SetAlpha(float alpha)
    {
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }
}
