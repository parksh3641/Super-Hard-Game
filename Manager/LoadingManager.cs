using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using DG.Tweening; // DOTween ���

public class LoadingManager : MonoBehaviour
{
    public Image progressBar; // Progress Bar UI (Image)
    public TMP_Text loadingText; // Loading ���� ǥ�� �ؽ�Ʈ
    public TMP_Text touchToContinueText; // "ȭ���� ��ġ���ּ���" �ؽ�Ʈ

    private bool isLoadComplete = false; // �ε� �Ϸ� ����
    float blinkDuration = 0.5f; // �����̴� ����
    bool isFadingOut = true;    // ������ ���̴� ������ Ȯ��

    AsyncOperation operation;

    private void Start()
    {
        // "ȭ���� ��ġ���ּ���" �ؽ�Ʈ �����
        if (touchToContinueText != null)
            touchToContinueText.gameObject.SetActive(false);

        // �ε��� ���� �����ϰ� �ε� ����
        StartCoroutine(LoadSceneAsync("MainScene")); // �ε��� �� �̸��� �Է�
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // �񵿱� �ε� ����
        operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false; // �ڵ� �� ��ȯ ��Ȱ��ȭ

        // �ε� ����� ������Ʈ
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f); // ����� ���
            progressBar.fillAmount = progress; // Progress Bar ������Ʈ

            if (loadingText != null)
                loadingText.text = $"{progress * 100:0}%"; // �ε� ���� �ؽ�Ʈ ������Ʈ

            // �ε��� �Ϸ�Ǿ��� ��
            if (operation.progress >= 0.9f && !isLoadComplete)
            {
                if (loadingText != null)
                    loadingText.text = "100%";

                if (touchToContinueText != null)
                {
                    touchToContinueText.gameObject.SetActive(true); // "ȭ���� ��ġ���ּ���" �ؽ�Ʈ ǥ��
                    StartBlinkingText(); // �����̴� ȿ�� ����
                }

                isLoadComplete = true; // �ε� �Ϸ� ���� ������Ʈ
            }

            yield return null;
        }
    }

    public void Click()
    {
        if (isLoadComplete && !operation.allowSceneActivation)
        {
            operation.allowSceneActivation = true;
        }
    }

    private void StartBlinkingText()
    {
        touchToContinueText.DOKill();

        // DOTween�� ����� �ؽ�Ʈ �����̱�
        touchToContinueText.DOFade(0, 0.5f) // ������ 0����
            .SetLoops(-1, LoopType.Yoyo)   // ���� �ݺ�, Yoyo�� �պ� ȿ��
            .SetEase(Ease.InOutSine);      // �ε巴�� �����̴� ȿ��
    }
}
