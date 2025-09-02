using UnityEngine;
using System.Collections;

public class LaserController : MonoBehaviour
{
    public GameObject laserObject;       // 레이저를 나타내는 별도의 GameObject
    public BoxCollider boxCollider;

    public SpriteRenderer[] spriteRenderer;
    public Sprite[] spriteArray;

    public AudioSource audioSource;

    private float activationDelay = 1f;   // 레이저 활성화 주기
    private float activeDuration = 1.0f;    // 레이저 활성화 유지 시간
    private float scaleDuration = 0.1f;   // 크기가 0에서 1로 커지는 시간

    WaitForSeconds waitForSeconds = new WaitForSeconds(0.3f);

    void Start()
    {
        if (laserObject == null)
        {
            Debug.LogError("LaserObject is not assigned!");
            return;
        }

        // BoxCollider 가져오기
        if (boxCollider == null)
        {
            Debug.LogError("LaserObject does not have a BoxCollider!");
            return;
        }

        // 초기 상태 설정
        laserObject.SetActive(false);
        laserObject.transform.localScale = new Vector3(
            laserObject.transform.localScale.x, -3f, laserObject.transform.localScale.z);

        audioSource.Stop();

        // 반복 시작
        StartCoroutine(LaserRoutine());
    }

    IEnumerator LaserRoutine()
    {
        while (true)
        {
            boxCollider.enabled = false;

            spriteRenderer[0].sprite = spriteArray[0];
            spriteRenderer[1].sprite = spriteArray[0];

            // 3초 대기 (비활성화 상태 유지)
            yield return new WaitForSeconds(Random.Range(activationDelay * 0.8f, activationDelay * 1.2f) - 0.3f);

            spriteRenderer[0].sprite = spriteArray[1];
            spriteRenderer[1].sprite = spriteArray[1];

            yield return waitForSeconds;

            spriteRenderer[0].sprite = spriteArray[0];
            spriteRenderer[1].sprite = spriteArray[0];

            if (GameStateManager.instance.Sfx)
            {
                audioSource.volume = GameStateManager.instance.SfxValue;
                audioSource.Play();
            }

            if (GameStateManager.instance.IsPlaying)
            {
                // 레이저 활성화
                laserObject.SetActive(true);
                yield return StartCoroutine(ScaleLaser(-3f, -0.5f, scaleDuration));
                boxCollider.enabled = true; // 충돌 활성화

                // 활성화 유지 시간
                yield return new WaitForSeconds(activeDuration);

                // 레이저 비활성화
                yield return StartCoroutine(ScaleLaser(-0.5f, -3f, scaleDuration));
                laserObject.SetActive(false);

                boxCollider.enabled = false; // 충돌 비활성화
            }
        }
    }

    IEnumerator ScaleLaser(float startY, float endY, float duration)
    {
        float elapsed = 0f;
        Vector3 startScale = new Vector3(laserObject.transform.localScale.x, startY, laserObject.transform.localScale.z);
        Vector3 endScale = new Vector3(laserObject.transform.localScale.x, endY, laserObject.transform.localScale.z);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            laserObject.transform.localScale = Vector3.Lerp(startScale, endScale, elapsed / duration);
            yield return null;
        }

        // 정확한 최종 스케일 적용
        laserObject.transform.localScale = endScale;
    }
}
