using System.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Random = UnityEngine.Random;

// CannonDOTSManager를 상속받는 Cannon 클래스
public class Cannon : CannonDOTSManager
{
    private Missile[] missilePool; // 기존 미사일 풀 (호환성을 위해 유지)
    private int currentMissileIndex = 0; // 현재 풀에서 사용할 미사일 인덱스
    private float randomAngle;
    private Coroutine launchCoroutine;

    // 원래 Cannon 클래스에 있던 필드는 CannonDOTSManager에서 이미 상속받음
    // missilePrefab, missilePoolSize, launchInterval, randomAngleRange, missileSpeed

    // 기존 Awake 메서드 오버라이드 - 베이스 클래스 호출 포함
    protected override void Awake()
    {
        base.Awake(); // CannonDOTSManager의 Awake 호출
    }

    // 기존 Start 메서드 오버라이드 - 베이스 클래스 호출 포함
    protected override void Start()
    {
        // 기존 미사일 풀 초기화 (호환성을 위해 유지)
        missilePool = new Missile[missilePoolSize];
        for (int i = 0; i < missilePoolSize; i++)
        {
            if (missilePrefab != null)
            {
                missilePool[i] = Instantiate(missilePrefab, transform.position, Quaternion.identity, transform);
                missilePool[i].gameObject.SetActive(false);
            }
        }

        // CannonDOTSManager의 Start 호출 (DOTS 엔티티 시스템 초기화)
        base.Start();
    }

    // 기존 GameStart 메서드 오버라이드 - 베이스 클래스 호출 포함
    protected override void GameStart()
    {
        base.GameStart(); // 기본 GameStart 호출

        if (gameObject.activeInHierarchy)
        {
            // 이미 실행 중인 코루틴이 있으면 중지
            if (launchCoroutine != null)
            {
                StopCoroutine(launchCoroutine);
            }

            // 새 코루틴 시작
            launchCoroutine = StartCoroutine(LaunchMissilesPeriodically());
        }
    }

    // 기존 주기적 미사일 발사 코루틴 (기존 방식으로 수정)
    private IEnumerator LaunchMissilesPeriodically()
    {
        while (true)
        {
            // GameStateManager가 없거나 IsPlaying이 false인 경우 대비
            if (GameStateManager.instance != null && GameStateManager.instance.IsPlaying)
            {
                try
                {
                    // 기존 방식으로 미사일 발사
                    LaunchMissile();
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"미사일 발사 중 오류 발생: {e}");
                }
            }

            // 다음 발사까지 대기
            yield return new WaitForSeconds(Random.Range(launchInterval * 0.8f, launchInterval * 1.2f));
        }
    }

    // 기존 LaunchMissile 메서드 - 기존 방식으로 미사일 발사
    public virtual void LaunchMissile()
    {
        // 기존 방식으로 바로 미사일 발사
        FallbackLaunchMissile(transform.position, transform.rotation);

        // 기존 미사일 풀 인덱스 업데이트
        UpdateMissilePoolIndex();
    }

    // 특정 위치에서 미사일 발사하는 메서드를 오버라이드 (기존 방식 사용)
    public override void LaunchMissileFromPosition(Vector3 position, Quaternion rotation)
    {
        // 기존 방식으로 바로 미사일 발사
        FallbackLaunchMissile(position, rotation);

        // 기존 미사일 풀 인덱스 업데이트
        UpdateMissilePoolIndex();
    }

    // 기존 방식으로 미사일 발사하는 메서드
    private void FallbackLaunchMissile(Vector3 position, Quaternion rotation)
    {
        if (missilePool == null || missilePool.Length == 0 || currentMissileIndex >= missilePool.Length)
        {
            Debug.LogWarning("미사일 풀이 올바르게 초기화되지 않았습니다.");
            return;
        }

        // 미사일 풀에서 미사일 가져오기
        Missile missile = missilePool[currentMissileIndex];
        if (missile != null)
        {
            // 미사일 초기화
            missile.transform.position = position + new Vector3(0, 0.5f, 0);
            randomAngle = Random.Range(-randomAngleRange / 2, randomAngleRange / 2);
            missile.transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y + randomAngle, 0);
            missile.SetSpeed(missileSpeed);
            missile.gameObject.SetActive(true);

            Debug.Log("기존 방식으로 미사일 발사");
        }
    }

    // 미사일 풀 인덱스 업데이트
    private void UpdateMissilePoolIndex()
    {
        if (missilePool != null && missilePool.Length > 0)
        {
            currentMissileIndex = (currentMissileIndex + 1) % missilePoolSize;
        }
    }

    // OnDestroy 오버라이드 - 코루틴 정리 추가
    protected override void OnDestroy()
    {
        if (launchCoroutine != null)
        {
            StopCoroutine(launchCoroutine);
            launchCoroutine = null;
        }

        base.OnDestroy();
    }

    // OnDisable 오버라이드 - 코루틴 정리 추가
    protected override void OnDisable()
    {
        if (launchCoroutine != null)
        {
            StopCoroutine(launchCoroutine);
            launchCoroutine = null;
        }

        base.OnDisable();
    }
}