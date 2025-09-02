using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using System.Collections.Generic;

// 캐논 시스템 (미사일 발사)
public partial class CannonSystem : SystemBase
{
    private EntityCommandBufferSystem ecbSystem;
    private GameStateManager gameStateManager;
    private EntityArchetype missileArchetype;

    protected override void OnCreate()
    {
        ecbSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
        RequireForUpdate<CannonComponent>();

        // 미사일 아키타입 미리 생성
        missileArchetype = EntityManager.CreateArchetype(
            typeof(MissileComponent),
            typeof(LocalTransform)
        );
    }

    protected override void OnStartRunning()
    {
        gameStateManager = GameObject.FindObjectOfType<GameStateManager>();
    }

    protected override void OnUpdate()
    {
        if (gameStateManager == null || !gameStateManager.IsPlaying)
            return;

        var deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = ecbSystem.CreateCommandBuffer();

        // 캐논 컴포넌트를 가진 모든 엔티티에 대해 처리
        var cannonEntities = SystemAPI.QueryBuilder().WithAll<CannonComponent>().Build().ToEntityArray(Allocator.Temp);

        foreach (var entity in cannonEntities)
        {
            var cannonComponent = EntityManager.GetComponentData<CannonComponent>(entity);

            // 발사 타이머 업데이트
            cannonComponent.CurrentLaunchTime += deltaTime;

            if (cannonComponent.CurrentLaunchTime >= cannonComponent.LaunchInterval)
            {
                cannonComponent.CurrentLaunchTime = 0f;

                // 랜덤 시드 업데이트
                var random = Unity.Mathematics.Random.CreateFromIndex(cannonComponent.RandomSeed++);

                // 실제 발사 간격에 약간의 랜덤성 추가
                float randomInterval = random.NextFloat(cannonComponent.LaunchInterval * 0.8f, cannonComponent.LaunchInterval * 1.2f);
                cannonComponent.CurrentLaunchTime -= randomInterval;

                // 미사일 생성 (프리팹 대신 아키타입 사용)
                var missile = ecb.CreateEntity(missileArchetype);

                // 발사 위치 및 각도 설정
                float randomAngle = random.NextFloat(-cannonComponent.RandomAngleRange / 2, cannonComponent.RandomAngleRange / 2);

                // 회전 계산
                quaternion baseRotation = cannonComponent.Rotation;
                quaternion randomRotation = quaternion.Euler(0, math.radians(randomAngle), 0);
                quaternion finalRotation = math.mul(baseRotation, randomRotation);

                // 발사 지점 오프셋 적용
                float3 launchPosition = cannonComponent.Position;

                // 오프셋 회전 적용
                float3 offset = cannonComponent.LaunchOffset;
                float3 rotatedOffset = math.rotate(baseRotation, offset);

                launchPosition += rotatedOffset;

                ecb.SetComponent(missile, new LocalTransform
                {
                    Position = launchPosition,
                    Rotation = finalRotation,
                    Scale = 1f
                });

                // 미사일 속도 설정
                float randomSpeed = random.NextFloat(cannonComponent.MissileSpeed * 0.8f, cannonComponent.MissileSpeed * 1.2f);
                ecb.SetComponent(missile, new MissileComponent
                {
                    Speed = randomSpeed,
                    InitialDelay = 0.3f,
                    CurrentDelay = 0f,
                    IsActive = true,
                    HasFire = false
                });
            }

            // 업데이트된 캐논 컴포넌트 저장
            EntityManager.SetComponentData(entity, cannonComponent);
        }

        cannonEntities.Dispose();

        ecbSystem.AddJobHandleForProducer(Dependency);
    }

    // 미사일 속도 증가
    public void SpeedUp(Entity cannonEntity, float amount = 0.5f)
    {
        if (!EntityManager.Exists(cannonEntity) || !EntityManager.HasComponent<CannonComponent>(cannonEntity))
            return;

        var cannon = EntityManager.GetComponentData<CannonComponent>(cannonEntity);
        cannon.MissileSpeed += amount;
        EntityManager.SetComponentData(cannonEntity, cannon);
    }

    // 미사일 속도 감소
    public void SpeedDown(Entity cannonEntity, float amount = 0.5f)
    {
        if (!EntityManager.Exists(cannonEntity) || !EntityManager.HasComponent<CannonComponent>(cannonEntity))
            return;

        var cannon = EntityManager.GetComponentData<CannonComponent>(cannonEntity);
        cannon.MissileSpeed -= amount;

        if (cannon.MissileSpeed <= 0)
        {
            cannon.MissileSpeed = 0.5f;
        }

        EntityManager.SetComponentData(cannonEntity, cannon);
    }

    // 즉시 미사일 발사 메서드
    public void LaunchMissileImmediate(Entity cannonEntity)
    {
        // 엔티티 유효성 검사 추가
        if (!EntityManager.Exists(cannonEntity) || !EntityManager.HasComponent<CannonComponent>(cannonEntity))
        {
            Debug.LogWarning("캐논 엔티티가 존재하지 않거나 유효하지 않습니다.");
            return;
        }

        var cannon = EntityManager.GetComponentData<CannonComponent>(cannonEntity);

        try
        {
            // 미사일 아키타입으로 직접 미사일 생성
            Entity missile = EntityManager.CreateEntity(missileArchetype);

            // 랜덤 시드 업데이트
            var random = Unity.Mathematics.Random.CreateFromIndex(cannon.RandomSeed++);

            // 발사 위치 및 각도 설정
            float randomAngle = random.NextFloat(-cannon.RandomAngleRange / 2, cannon.RandomAngleRange / 2);

            // 회전 계산
            quaternion baseRotation = cannon.Rotation;
            quaternion randomRotation = quaternion.Euler(0, math.radians(randomAngle), 0);
            quaternion finalRotation = math.mul(baseRotation, randomRotation);

            // 발사 지점 오프셋 적용
            float3 launchPosition = cannon.Position;
            float3 offset = cannon.LaunchOffset;
            float3 rotatedOffset = math.rotate(baseRotation, offset);
            launchPosition += rotatedOffset;

            // 변환 컴포넌트 설정
            EntityManager.SetComponentData(missile, new LocalTransform
            {
                Position = launchPosition,
                Rotation = finalRotation,
                Scale = 1f
            });

            // 미사일 속도 설정
            float randomSpeed = random.NextFloat(cannon.MissileSpeed * 0.8f, cannon.MissileSpeed * 1.2f);
            EntityManager.SetComponentData(missile, new MissileComponent
            {
                Speed = randomSpeed,
                InitialDelay = 0.3f,
                CurrentDelay = 0f,
                IsActive = true,
                HasFire = false
            });

            // 업데이트된 캐논 컴포넌트 저장
            EntityManager.SetComponentData(cannonEntity, cannon);

            // GameObject 활성화하기 위한 이벤트 발생
            // 이 부분은 실제로는 동작하지 않을 것이지만, 기존 방식으로 대체될 것입니다.
        }
        catch (System.Exception e)
        {
            Debug.LogError($"미사일 생성 중 오류 발생: {e}");
        }
    }
}

// 미사일 시스템 (미사일 이동 및 충돌 처리)
public partial class MissileSystem : SystemBase
{
    private EntityCommandBufferSystem ecbSystem;
    private EntityQuery playerQuery;
    private EntityQuery safeZoneQuery;
    private EntityQuery lastLineQuery;
    private ParticleManager particleManager;
    private SoundManager soundManager;

    protected override void OnCreate()
    {
        ecbSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
        RequireForUpdate<MissileComponent>();

        // 충돌 대상 쿼리 설정
        playerQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerTag>());
        safeZoneQuery = GetEntityQuery(ComponentType.ReadOnly<SafeZoneTag>());
        lastLineQuery = GetEntityQuery(ComponentType.ReadOnly<LastLineTag>());
    }

    protected override void OnStartRunning()
    {
        particleManager = GameObject.FindObjectOfType<ParticleManager>();
        soundManager = GameObject.FindObjectOfType<SoundManager>();
    }

    protected override void OnUpdate()
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = ecbSystem.CreateCommandBuffer();

        // 플레이어 위치 가져오기 (충돌 검사용)
        float3 playerPosition = float3.zero;
        if (!playerQuery.IsEmpty)
        {
            var playerEntities = playerQuery.ToEntityArray(Allocator.Temp);
            if (playerEntities.Length > 0 && EntityManager.HasComponent<LocalTransform>(playerEntities[0]))
            {
                playerPosition = EntityManager.GetComponentData<LocalTransform>(playerEntities[0]).Position;
            }
            playerEntities.Dispose();
        }

        // 미사일 엔티티 가져오기
        var missileEntities = SystemAPI.QueryBuilder().WithAll<MissileComponent, LocalTransform>().Build().ToEntityArray(Allocator.Temp);

        foreach (var entity in missileEntities)
        {
            if (!EntityManager.Exists(entity))
                continue;

            var missile = EntityManager.GetComponentData<MissileComponent>(entity);
            var transform = EntityManager.GetComponentData<LocalTransform>(entity);

            // 초기 지연시간 처리
            if (missile.CurrentDelay < missile.InitialDelay)
            {
                missile.CurrentDelay += deltaTime;
                if (missile.CurrentDelay >= missile.InitialDelay)
                {
                    missile.Speed = missile.Speed;
                }
                EntityManager.SetComponentData(entity, missile);
                continue;
            }

            // 미사일 이동
            float3 forward = new float3(math.forward(transform.Rotation).x, 0, math.forward(transform.Rotation).z);
            forward = math.normalize(forward);
            transform.Position += forward * missile.Speed * deltaTime;

            // 플레이어와의 거리에 따라 불꽃 효과 활성화 여부 결정
            float sqrDistance = math.distancesq(transform.Position, playerPosition);
            missile.HasFire = sqrDistance <= 100f;

            // 컴포넌트 업데이트
            EntityManager.SetComponentData(entity, missile);
            EntityManager.SetComponentData(entity, transform);

            // 충돌 감지 (간단한 구현을 위해 거리 기반으로 처리)

            // 플레이어 충돌
            if (sqrDistance < 1f)
            {
                if (particleManager != null)
                {
                    quaternion rotation = transform.Rotation;
                    particleManager.GetParticle(
                        new Vector3(transform.Position.x, transform.Position.y, transform.Position.z),
                        new Quaternion(rotation.value.x, rotation.value.y, rotation.value.z, rotation.value.w)
                    );
                }

                if (soundManager != null)
                {
                    soundManager.PlaySFX(GameSfxType.Bomb);
                }

                ecb.DestroyEntity(entity);
                continue;
            }

            // SafeZone 충돌 (간략화된 구현)
            if (!safeZoneQuery.IsEmpty && math.distance(transform.Position, playerPosition) < 10f)
            {
                if (particleManager != null)
                {
                    quaternion rotation = transform.Rotation;
                    particleManager.GetParticle(
                        new Vector3(transform.Position.x, transform.Position.y, transform.Position.z),
                        new Quaternion(rotation.value.x, rotation.value.y, rotation.value.z, rotation.value.w)
                    );
                }

                ecb.DestroyEntity(entity);
                continue;
            }

            // LastLine 충돌 검사 로직 (간략화됨)
            // 맵 밖으로 나갔는지 체크
            if (math.length(transform.Position) > 100f)
            {
                ecb.DestroyEntity(entity);
                continue;
            }
        }

        missileEntities.Dispose();
        ecbSystem.AddJobHandleForProducer(Dependency);
    }
}