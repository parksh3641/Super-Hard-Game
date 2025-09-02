using UnityEngine;
using System.Collections;

public class LaserController : MonoBehaviour
{
    public GameObject laserObject;       // �������� ��Ÿ���� ������ GameObject
    public BoxCollider boxCollider;

    public SpriteRenderer[] spriteRenderer;
    public Sprite[] spriteArray;

    public AudioSource audioSource;

    private float activationDelay = 1f;   // ������ Ȱ��ȭ �ֱ�
    private float activeDuration = 1.0f;    // ������ Ȱ��ȭ ���� �ð�
    private float scaleDuration = 0.1f;   // ũ�Ⱑ 0���� 1�� Ŀ���� �ð�

    WaitForSeconds waitForSeconds = new WaitForSeconds(0.3f);

    void Start()
    {
        if (laserObject == null)
        {
            Debug.LogError("LaserObject is not assigned!");
            return;
        }

        // BoxCollider ��������
        if (boxCollider == null)
        {
            Debug.LogError("LaserObject does not have a BoxCollider!");
            return;
        }

        // �ʱ� ���� ����
        laserObject.SetActive(false);
        laserObject.transform.localScale = new Vector3(
            laserObject.transform.localScale.x, -3f, laserObject.transform.localScale.z);

        audioSource.Stop();

        // �ݺ� ����
        StartCoroutine(LaserRoutine());
    }

    IEnumerator LaserRoutine()
    {
        while (true)
        {
            boxCollider.enabled = false;

            spriteRenderer[0].sprite = spriteArray[0];
            spriteRenderer[1].sprite = spriteArray[0];

            // 3�� ��� (��Ȱ��ȭ ���� ����)
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
                // ������ Ȱ��ȭ
                laserObject.SetActive(true);
                yield return StartCoroutine(ScaleLaser(-3f, -0.5f, scaleDuration));
                boxCollider.enabled = true; // �浹 Ȱ��ȭ

                // Ȱ��ȭ ���� �ð�
                yield return new WaitForSeconds(activeDuration);

                // ������ ��Ȱ��ȭ
                yield return StartCoroutine(ScaleLaser(-0.5f, -3f, scaleDuration));
                laserObject.SetActive(false);

                boxCollider.enabled = false; // �浹 ��Ȱ��ȭ
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

        // ��Ȯ�� ���� ������ ����
        laserObject.transform.localScale = endScale;
    }
}
