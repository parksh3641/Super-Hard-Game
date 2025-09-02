using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;

// 기존 MonoBehaviour 코드와 DOTS 시스템 간의 연결 및 변환을 위한 브릿지
public class CannonDOTSManager : MonoBehaviour
{
    public Missile missilePrefab; // 미사일 프리팹
    public int missilePoolSize = 10; // 미사일 풀 크기
    public float launchInterval = 3f; // 발사 간격
    public float randomAngleRange = 80f; // 발사 각도 랜덤 범위
    public float missileSpeed = 4.5f;
    public Vector3 launchOffset = new Vector3(0, 0.5f, 0); // 미사일 발사 오프셋

    protected Entity cannonEntity;
    protected World defaultWorld;
    protected CannonSystem cannonSystem;
    protected PlayerDataBase playerDataBase;

    // 유효성 검사를 위한 상태 플래그
    protected bool isInitialized = false;
    protected bool isEntityCreated = false;

    protected virtual void Awake()
    {
        if (playerDataBase == null)
            playerDataBase = Resources.Load("PlayerDataBase") as PlayerDataBase;
    }

    protected virtual void Start()
    {
        InitializeDOTSEntities();
    }

    protected virtual void OnEnable()
    {
        // OnEnable에서도 초기화 확인 (비활성화 후 다시 활성화될 때 대비)
        if (!isInitialized)
        {
            InitializeDOTSEntities();
        }
    }

    // DOTS 엔티티 시스템 초기화 (상속받는 클래스에서 접근할 수 있도록 protected)
    protected void InitializeDOTSEntities()
    {
        defaultWorld = World.DefaultGameObjectInjectionWorld;

        if (defaultWorld != null)
        {
            try
            {
                // 캐논 시스템 참조 가져오기
                cannonSystem = defaultWorld.GetOrCreateSystemManaged<CannonSystem>();

                // 엔티티 매니저 참조
                var entityManager = defaultWorld.EntityManager;

                // 기존 엔티티가 있다면 제거
                if (isEntityCreated && entityManager.Exists(cannonEntity))
                {
                    entityManager.DestroyEntity(cannonEntity);
                }

                // 캐논 엔티티 생성
                var cannonArchetype = entityManager.CreateArchetype(
                    typeof(CannonComponent)
                );

                cannonEntity = entityManager.CreateEntity(cannonArchetype);
                isEntityCreated = true;

                // 캐논 컴포넌트 설정
                entityManager.SetComponentData(cannonEntity, new CannonComponent
                {
                    LaunchInterval = launchInterval,
                    CurrentLaunchTime = 0f,
                    RandomAngleRange = randomAngleRange,
                    MissileSpeed = playerDataBase != null && playerDataBase.Stage + 1 > 5 ? 5.0f : missileSpeed,
                    MissilePoolSize = missilePoolSize,
                    Position = transform.position,
                    Rotation = transform.rotation,
                    RandomSeed = (uint)System.DateTime.Now.Ticks,
                    LaunchOffset = new float3(launchOffset.x, launchOffset.y, launchOffset.z)
                });

                isInitialized = true;

                // 미사일 프리팹 엔티티는 더 이상 필요하지 않음 (CannonSystem에서 직접 생성)

                // 초기화 후 위치 업데이트 (Scene View에서 변경됐을 가능성 고려)
                UpdateTransform();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"DOTS 엔티티 초기화 중 오류 발생: {e}");
                isInitialized = false;
                isEntityCreated = false;
            }
        }
        else
        {
            Debug.LogError("DOTS World를 찾을 수 없습니다.");
            isInitialized = false;
        }

        Invoke("GameStart", 0.5f);
    }

    // 매 프레임 트랜스폼 업데이트
    protected virtual void Update()
    {
        if (isInitialized && isEntityCreated)
        {
            UpdateTransform();
        }
    }

    // 트랜스폼 정보 업데이트
    protected void UpdateTransform()
    {
        if (defaultWorld != null && isEntityCreated && defaultWorld.IsCreated)
        {
            var entityManager = defaultWorld.EntityManager;

            if (entityManager.Exists(cannonEntity) && entityManager.HasComponent<CannonComponent>(cannonEntity))
            {
                var cannonComponent = entityManager.GetComponentData<CannonComponent>(cannonEntity);
                cannonComponent.Position = transform.position;
                cannonComponent.Rotation = transform.rotation;
                entityManager.SetComponentData(cannonEntity, cannonComponent);
            }
        }
    }

    // 기본 GameStart 메서드
    protected virtual void GameStart()
    {
        if (gameObject.activeInHierarchy && playerDataBase != null && isInitialized && isEntityCreated)
        {
            if (playerDataBase.Stage + 1 > 5 && defaultWorld != null && defaultWorld.IsCreated)
            {
                var entityManager = defaultWorld.EntityManager;

                if (entityManager.Exists(cannonEntity) && entityManager.HasComponent<CannonComponent>(cannonEntity))
                {
                    var cannonComponent = entityManager.GetComponentData<CannonComponent>(cannonEntity);
                    cannonComponent.MissileSpeed = 5.0f;
                    entityManager.SetComponentData(cannonEntity, cannonComponent);
                }
            }
        }
    }

    // 외부에서 캐논 엔티티에 접근할 수 있도록 getter 추가
    public Entity GetCannonEntity()
    {
        if (!isInitialized || !isEntityCreated)
        {
            Debug.LogWarning("DOTS 엔티티가 초기화되지 않았습니다.");
            return Entity.Null;
        }
        return cannonEntity;
    }

    // 즉시 미사일 발사 메서드 (CannonSystem과 통신)
    public void LaunchMissileImmediate()
    {
        if (!isInitialized || !isEntityCreated)
        {
            // 초기화 상태 확인 후 필요시 초기화 시도
            if (!isInitialized)
            {
                InitializeDOTSEntities();
                if (!isInitialized) // 초기화 실패 시
                {
                    Debug.LogWarning("DOTS 엔티티 시스템이 초기화되지 않아 발사할 수 없습니다.");
                    return;
                }
            }
        }

        if (defaultWorld != null && defaultWorld.IsCreated && cannonEntity != Entity.Null)
        {
            // 엔티티 매니저가 유효한지 확인
            var entityManager = defaultWorld.EntityManager;
            if (!entityManager.Exists(cannonEntity))
            {
                // 엔티티가 존재하지 않으면 재생성 시도
                Debug.LogWarning("캐논 엔티티가 유효하지 않습니다. 재생성을 시도합니다.");
                InitializeDOTSEntities();
                if (!isEntityCreated)
                {
                    return; // 재생성 실패
                }
            }

            var cannonSystem = defaultWorld.GetOrCreateSystemManaged<CannonSystem>();
            if (cannonSystem != null)
            {
                cannonSystem.LaunchMissileImmediate(cannonEntity);
            }
        }
    }

    // 특정 위치에서 미사일 발사
    public virtual void LaunchMissileFromPosition(Vector3 position, Quaternion rotation)
    {
        if (!isInitialized || !isEntityCreated)
        {
            InitializeDOTSEntities();
            if (!isInitialized)
            {
                Debug.LogWarning("DOTS 엔티티 시스템이 초기화되지 않아 발사할 수 없습니다.");
                return;
            }
        }

        if (defaultWorld != null && defaultWorld.IsCreated && cannonEntity != Entity.Null)
        {
            var entityManager = defaultWorld.EntityManager;

            if (entityManager.Exists(cannonEntity) && entityManager.HasComponent<CannonComponent>(cannonEntity))
            {
                // 캐논 컴포넌트 가져오기
                var cannonComponent = entityManager.GetComponentData<CannonComponent>(cannonEntity);

                // 임시로 위치와 회전 설정
                cannonComponent.Position = new float3(position.x, position.y, position.z);
                cannonComponent.Rotation = new quaternion(rotation.x, rotation.y, rotation.z, rotation.w);

                // 업데이트된 컴포넌트 설정
                entityManager.SetComponentData(cannonEntity, cannonComponent);

                // 미사일 즉시 발사
                LaunchMissileImmediate();

                // 원래 위치로 되돌리기
                UpdateTransform();
            }
        }
    }

    // 미사일 속도 증가 메서드 (기존 API와 호환성 유지)
    public virtual void SpeedUp()
    {
        if (!isInitialized || !isEntityCreated)
        {
            return;
        }

        if (cannonSystem != null && cannonEntity != Entity.Null)
        {
            cannonSystem.SpeedUp(cannonEntity, 0.5f);
        }
    }

    // 미사일 속도 감소 메서드 (기존 API와 호환성 유지)
    public virtual void SpeedDown()
    {
        if (!isInitialized || !isEntityCreated)
        {
            return;
        }

        if (cannonSystem != null && cannonEntity != Entity.Null)
        {
            cannonSystem.SpeedDown(cannonEntity, 0.5f);
        }
    }

    protected virtual void OnDisable()
    {
        // 비활성화 시 일시적으로 상태 저장 (다시 활성화될 때 재사용)
    }

    protected virtual void OnDestroy()
    {
        // 엔티티 정리
        if (defaultWorld != null && defaultWorld.IsCreated && isEntityCreated && cannonEntity != Entity.Null)
        {
            var entityManager = defaultWorld.EntityManager;
            if (entityManager.Exists(cannonEntity))
            {
                entityManager.DestroyEntity(cannonEntity);
            }
        }

        isInitialized = false;
        isEntityCreated = false;
    }
}