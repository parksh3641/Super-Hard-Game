using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// 여러 발사 지점에서 DOTS 기반 미사일 시스템을 관리하는 클래스
/// </summary>
public class LaunchPointManager : MonoBehaviour
{
    public CannonDOTSManager cannonManager; // DOTS 캐논 매니저 참조
    public Transform[] launchPoints; // 발사 지점들
    public float launchInterval = 3f; // 발사 간격

    private PlayerDataBase playerDataBase;
    private float nextLaunchTime = 0f;
    private bool initialized = false;

    private void Awake()
    {
        if (playerDataBase == null)
            playerDataBase = Resources.Load("PlayerDataBase") as PlayerDataBase;

        // 캐논 매니저가 설정되지 않은 경우, 현재 오브젝트나 자식에서 찾기
        if (cannonManager == null)
        {
            cannonManager = GetComponent<CannonDOTSManager>();
            if (cannonManager == null)
                cannonManager = GetComponentInChildren<CannonDOTSManager>();
        }
    }

    private void Start()
    {
        // 랜덤 시드 초기화
        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);

        // 발사 지점이 정의되지 않았다면 자신을 발사 지점으로 추가
        if (launchPoints == null || launchPoints.Length == 0)
        {
            launchPoints = new Transform[1] { transform };
        }

        initialized = true;

        Invoke("GameStart", 0.5f);
    }

    private void GameStart()
    {
        if (gameObject.activeInHierarchy && playerDataBase != null)
        {
            // 스테이지에 따른 미사일 속도 설정 - 직접 CannonSystem 호출
            if (playerDataBase.Stage + 1 > 5 && cannonManager != null)
            {
                Entity cannonEntity = cannonManager.GetCannonEntity();
                World defaultWorld = World.DefaultGameObjectInjectionWorld;

                if (defaultWorld != null && cannonEntity != Entity.Null)
                {
                    var entityManager = defaultWorld.EntityManager;

                    if (entityManager.HasComponent<CannonComponent>(cannonEntity))
                    {
                        var cannonComponent = entityManager.GetComponentData<CannonComponent>(cannonEntity);
                        cannonComponent.MissileSpeed = 5.0f;
                        entityManager.SetComponentData(cannonEntity, cannonComponent);
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (!initialized || cannonManager == null || GameStateManager.instance == null || !GameStateManager.instance.IsPlaying)
            return;

        // 발사 시간이 되었는지 확인
        if (Time.time >= nextLaunchTime)
        {
            LaunchMissileFromRandomPoint();

            // 다음 발사 시간 계산 (랜덤 간격)
            nextLaunchTime = Time.time + UnityEngine.Random.Range(launchInterval * 0.8f, launchInterval * 1.2f);
        }
    }

    // 랜덤한 발사 지점에서 미사일 발사
    public void LaunchMissileFromRandomPoint()
    {
        if (launchPoints == null || launchPoints.Length == 0 || cannonManager == null)
            return;

        // 랜덤 발사 지점 선택
        int randomIndex = UnityEngine.Random.Range(0, launchPoints.Length);
        Transform launchPoint = launchPoints[randomIndex];

        // 선택된 지점에서 미사일 발사
        if (launchPoint != null)
        {
            LaunchMissileFromPoint(launchPoint);
        }
    }

    // 특정 발사 지점에서 미사일 발사
    public void LaunchMissileFromPoint(Transform launchPoint)
    {
        if (launchPoint == null || cannonManager == null)
            return;

        // 발사 위치와 회전을 사용하여 미사일 발사
        cannonManager.LaunchMissileFromPosition(launchPoint.position, launchPoint.rotation);
    }

    // 특정 위치에서 직접 미사일 발사
    public void LaunchMissileFromPosition(Vector3 position, Quaternion rotation)
    {
        if (cannonManager == null)
            return;

        cannonManager.LaunchMissileFromPosition(position, rotation);
    }

    // 미사일 속도 증가
    public void SpeedUp()
    {
        if (cannonManager != null)
        {
            cannonManager.SpeedUp();
        }
    }

    // 미사일 속도 감소
    public void SpeedDown()
    {
        if (cannonManager != null)
        {
            cannonManager.SpeedDown();
        }
    }

    // 발사 간격 설정
    public void SetLaunchInterval(float interval)
    {
        launchInterval = Mathf.Max(0.1f, interval);
    }
}