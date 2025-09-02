using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSwitcher : MonoBehaviour
{
    public Image targetImage;   // 변경할 UI Image
    public Sprite sprite1;      // 첫 번째 스프라이트
    public Sprite sprite2;      // 두 번째 스프라이트
    public float switchInterval = 0.5f; // 변경 간격 (초)

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
