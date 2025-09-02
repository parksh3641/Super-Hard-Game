using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSwitcher : MonoBehaviour
{
    public Image targetImage;   // ������ UI Image
    public Sprite sprite1;      // ù ��° ��������Ʈ
    public Sprite sprite2;      // �� ��° ��������Ʈ
    public float switchInterval = 0.5f; // ���� ���� (��)

    private void Start()
    {
        StartCoroutine(SwitchSprite());
    }

    private IEnumerator SwitchSprite()
    {
        while (true)
        {
            targetImage.sprite = sprite1;
            yield return new WaitForSeconds(switchInterval);

            targetImage.sprite = sprite2;
            yield return new WaitForSeconds(switchInterval);
        }
    }
}
