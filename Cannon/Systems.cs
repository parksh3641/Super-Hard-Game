using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using System.Collections.Generic;

// ĳ�� �ý��� (�̻��� �߻�)
public partial class CannonSystem : SystemBase
{
    private EntityCommandBufferSystem ecbSystem;
    private GameStateManager gameStateManager;
    private EntityArchetype missileArchetype;

    protected override void OnCreate()
    {
        ecbSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
        RequireForUpdate<CannonComponent>();

        // �̻��� ��ŰŸ�� �̸� ����
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

        // ĳ�� ������Ʈ�� ���� ��� ��ƼƼ�� ���� ó��
        var cannonEntities = SystemAPI.QueryBuilder().WithAll<CannonComponent>().Build().ToEntityArray(Allocator.Temp);

        foreach (var entity in cannonEntities)
        {
            var cannonComponent = EntityManager.GetComponentData<CannonComponent>(entity);

            // �߻� Ÿ�̸� ������Ʈ
            cannonComponent.CurrentLaunchTime += deltaTime;

            if (cannonComponent.CurrentLaunchTime >= cannonComponent.LaunchInterval)
            {
                cannonComponent.CurrentLaunchTime = 0f;

                // ���� �õ� ������Ʈ
                var random = Unity.Mathematics.Random.CreateFromIndex(cannonComponent.RandomSeed++);

                // ���� �߻� ���ݿ� �ణ�� ������ �߰�
                float randomInterval = random.NextFloat(cannonComponent.LaunchInterval * 0.8f, cannonComponent.LaunchInterval * 1.2f);
                cannonComponent.CurrentLaunchTime -= randomInterval;

                // �̻��� ���� (������ ��� ��ŰŸ�� ���)
                var missile = ecb.CreateEntity(missileArchetype);

                // �߻� ��ġ �� ���� ����
                float randomAngle = random.NextFloat(-cannonComponent.RandomAngleRange / 2, cannonComponent.RandomAngleRange / 2);

                // ȸ�� ���
                quaternion baseRotation = cannonComponent.Rotation;
                quaternion randomRotation = quaternion.Euler(0, math.radians(randomAngle), 0);
                quaternion finalRotation = math.mul(baseRotation, randomRotation);

                // �߻� ���� ������ ����
                float3 launchPosition = cannonComponent.Position;

                // ������ ȸ�� ����
                float3 offset = cannonComponent.LaunchOffset;
                float3 rotatedOffset = math.rotate(baseRotation, offset);

                launchPosition += rotatedOffset;

                ecb.SetComponent(missile, new LocalTransform
                {
                    Position = launchPosition,
                    Rotation = finalRotation,
                    Scale = 1f
                });

                // �̻��� �ӵ� ����
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

            // ������Ʈ�� ĳ�� ������Ʈ ����
            EntityManager.SetComponentData(entity, cannonComponent);
        }

        cannonEntities.Dispose();

        ecbSystem.AddJobHandleForProducer(Dependency);
    }

    // �̻��� �ӵ� ����
    public void SpeedUp(Entity cannonEntity, float amount = 0.5f)
    {
        if (!EntityManager.Exists(cannonEntity) || !EntityManager.HasComponent<CannonComponent>(cannonEntity))
            return;

        var cannon = EntityManager.GetComponentData<CannonComponent>(cannonEntity);
        cannon.MissileSpeed += amount;
        EntityManager.SetComponentData(cannonEntity, cannon);
    }

    // �̻��� �ӵ� ����
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

    // ��� �̻��� �߻� �޼���
    public void LaunchMissileImmediate(Entity cannonEntity)
    {
        // ��ƼƼ ��ȿ�� �˻� �߰�
        if (!EntityManager.Exists(cannonEntity) || !EntityManager.HasComponent<CannonComponent>(cannonEntity))
        {
            Debug.LogWarning("ĳ�� ��ƼƼ�� �������� �ʰų� ��ȿ���� �ʽ��ϴ�.");
            return;
        }

        var cannon = EntityManager.GetComponentData<CannonComponent>(cannonEntity);

        try
        {
            // �̻��� ��ŰŸ������ ���� �̻��� ����
            Entity missile = EntityManager.CreateEntity(missileArchetype);

            // ���� �õ� ������Ʈ
            var random = Unity.Mathematics.Random.CreateFromIndex(cannon.RandomSeed++);

            // �߻� ��ġ �� ���� ����
            float randomAngle = random.NextFloat(-cannon.RandomAngleRange / 2, cannon.RandomAngleRange / 2);

            // ȸ�� ���
            quaternion baseRotation = cannon.Rotation;
            quaternion randomRotation = quaternion.Euler(0, math.radians(randomAngle), 0);
            quaternion finalRotation = math.mul(baseRotation, randomRotation);

            // �߻� ���� ������ ����
            float3 launchPosition = cannon.Position;
            float3 offset = cannon.LaunchOffset;
            float3 rotatedOffset = math.rotate(baseRotation, offset);
            launchPosition += rotatedOffset;

            // ��ȯ ������Ʈ ����
            EntityManager.SetComponentData(missile, new LocalTransform
            {
                Position = launchPosition,
                Rotation = finalRotation,
                Scale = 1f
            });

            // �̻��� �ӵ� ����
            float randomSpeed = random.NextFloat(cannon.MissileSpeed * 0.8f, cannon.MissileSpeed * 1.2f);
            EntityManager.SetComponentData(missile, new MissileComponent
            {
                Speed = randomSpeed,
                InitialDelay = 0.3f,
                CurrentDelay = 0f,
                IsActive = true,
                HasFire = false
            });

            // ������Ʈ�� ĳ�� ������Ʈ ����
            EntityManager.SetComponentData(cannonEntity, cannon);

            // GameObject Ȱ��ȭ�ϱ� ���� �̺�Ʈ �߻�
            // �� �κ��� �����δ� �������� ���� ��������, ���� ������� ��ü�� ���Դϴ�.
        }
        catch (System.Exception e)
        {
            Debug.LogError($"�̻��� ���� �� ���� �߻�: {e}");
        }
    }
}

// �̻��� �ý��� (�̻��� �̵� �� �浹 ó��)
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

        // �浹 ��� ���� ����
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

        // �÷��̾� ��ġ �������� (�浹 �˻��)
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

        // �̻��� ��ƼƼ ��������
        var missileEntities = SystemAPI.QueryBuilder().WithAll<MissileComponent, LocalTransform>().Build().ToEntityArray(Allocator.Temp);

        foreach (var entity in missileEntities)
        {
            if (!EntityManager.Exists(entity))
                continue;

            var missile = EntityManager.GetComponentData<MissileComponent>(entity);
            var transform = EntityManager.GetComponentData<LocalTransform>(entity);

            // �ʱ� �����ð� ó��
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

            // �̻��� �̵�
            float3 forward = new float3(math.forward(transform.Rotation).x, 0, math.forward(transform.Rotation).z);
            forward = math.normalize(forward);
            transform.Position += forward * missile.Speed * deltaTime;

            // �÷��̾���� �Ÿ��� ���� �Ҳ� ȿ�� Ȱ��ȭ ���� ����
            float sqrDistance = math.distancesq(transform.Position, playerPosition);
            missile.HasFire = sqrDistance <= 100f;

            // ������Ʈ ������Ʈ
            EntityManager.SetComponentData(entity, missile);
            EntityManager.SetComponentData(entity, transform);

            // �浹 ���� (������ ������ ���� �Ÿ� ������� ó��)

            // �÷��̾� �浹
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

            // SafeZone �浹 (����ȭ�� ����)
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

            // LastLine �浹 �˻� ���� (����ȭ��)
            // �� ������ �������� üũ
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