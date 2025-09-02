using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using DG.Tweening; // DOTween 사용

public class LoadingManager : MonoBehaviour
{
    public Image progressBar; // Progress Bar UI (Image)
    public TMP_Text loadingText; // Loading 상태 표시 텍스트
    public TMP_Text touchToContinueText; // "화면을 터치해주세요" 텍스트

    private bool isLoadComplete = false; // 로딩 완료 상태
    float blinkDuration = 0.5f; // 깜빡이는 간격
    bool isFadingOut = true;    // 투명도를 줄이는 중인지 확인

    AsyncOperation operation;

    private void Start()
    {
        // "화면을 터치해주세요" 텍스트 숨기기
        if (touchToContinueText != null)
            touchToContinueText.gameObject.SetActive(false);

        // 로드할 씬을 설정하고 로딩 시작
        StartCoroutine(LoadSceneAsync("MainScene")); // 로드할 씬 이름을 입력
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // 비동기 로드 시작
        operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false; // 자동 씬 전환 비활성화

        // 로딩 진행률 업데이트
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f); // 진행률 계산
            progressBar.fillAmount = progress; // Progress Bar 업데이트

            if (loadingText != null)
                loadingText.text = $"{progress * 100:0}%"; // 로딩 상태 텍스트 업데이트

            // 로딩이 완료되었을 때
            if (operation.progress >= 0.9f && !isLoadComplete)
            {
                if (loadingText != null)
                    loadingText.text = "100%";

                if (touchToContinueText != null)
                {
                    touchToContinueText.gameObject.SetActive(true); // "화면을 터치해주세요" 텍스트 표시
                    StartBlinkingText(); // 깜빡이는 효과 시작
                }

                isLoadComplete = true; // 로딩 완료 상태 업데이트
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

        // DOTween을 사용해 텍스트 깜빡이기
        touchToContinueText.DOFade(0, 0.5f) // 투명도를 0으로
            .SetLoops(-1, LoopType.Yoyo)   // 무한 반복, Yoyo로 왕복 효과
            .SetEase(Ease.InOutSine);      // 부드럽게 깜빡이는 효과
    }
}
