using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;

// ���� MonoBehaviour �ڵ�� DOTS �ý��� ���� ���� �� ��ȯ�� ���� �긴��
public class CannonDOTSManager : MonoBehaviour
{
    public Missile missilePrefab; // �̻��� ������
    public int missilePoolSize = 10; // �̻��� Ǯ ũ��
    public float launchInterval = 3f; // �߻� ����
    public float randomAngleRange = 80f; // �߻� ���� ���� ����
    public float missileSpeed = 4.5f;
    public Vector3 launchOffset = new Vector3(0, 0.5f, 0); // �̻��� �߻� ������

    protected Entity cannonEntity;
    protected World defaultWorld;
    protected CannonSystem cannonSystem;
    protected PlayerDataBase playerDataBase;

    // ��ȿ�� �˻縦 ���� ���� �÷���
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
        // OnEnable������ �ʱ�ȭ Ȯ�� (��Ȱ��ȭ �� �ٽ� Ȱ��ȭ�� �� ���)
        if (!isInitialized)
        {
            InitializeDOTSEntities();
        }
    }

    // DOTS ��ƼƼ �ý��� �ʱ�ȭ (��ӹ޴� Ŭ�������� ������ �� �ֵ��� protected)
    protected void InitializeDOTSEntities()
    {
        defaultWorld = World.DefaultGameObjectInjectionWorld;

        if (defaultWorld != null)
        {
            try
            {
                // ĳ�� �ý��� ���� ��������
                cannonSystem = defaultWorld.GetOrCreateSystemManaged<CannonSystem>();

                // ��ƼƼ �Ŵ��� ����
                var entityManager = defaultWorld.EntityManager;

                // ���� ��ƼƼ�� �ִٸ� ����
                if (isEntityCreated && entityManager.Exists(cannonEntity))
                {
                    entityManager.DestroyEntity(cannonEntity);
                }

                // ĳ�� ��ƼƼ ����
                var cannonArchetype = entityManager.CreateArchetype(
                    typeof(CannonComponent)
                );

                cannonEntity = entityManager.CreateEntity(cannonArchetype);
                isEntityCreated = true;

                // ĳ�� ������Ʈ ����
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

                // �̻��� ������ ��ƼƼ�� �� �̻� �ʿ����� ���� (CannonSystem���� ���� ����)

                // �ʱ�ȭ �� ��ġ ������Ʈ (Scene View���� ������� ���ɼ� ���)
                UpdateTransform();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"DOTS ��ƼƼ �ʱ�ȭ �� ���� �߻�: {e}");
                isInitialized = false;
                isEntityCreated = false;
            }
        }
        else
        {
            Debug.LogError("DOTS World�� ã�� �� �����ϴ�.");
            isInitialized = false;
        }

        Invoke("GameStart", 0.5f);
    }

    // �� ������ Ʈ������ ������Ʈ
    protected virtual void Update()
    {
        if (isInitialized && isEntityCreated)
        {
            UpdateTransform();
        }
    }

    // Ʈ������ ���� ������Ʈ
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

    // �⺻ GameStart �޼���
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

    // �ܺο��� ĳ�� ��ƼƼ�� ������ �� �ֵ��� getter �߰�
    public Entity GetCannonEntity()
    {
        if (!isInitialized || !isEntityCreated)
        {
            Debug.LogWarning("DOTS ��ƼƼ�� �ʱ�ȭ���� �ʾҽ��ϴ�.");
            return Entity.Null;
        }
        return cannonEntity;
    }

    // ��� �̻��� �߻� �޼��� (CannonSystem�� ���)
    public void LaunchMissileImmediate()
    {
        if (!isInitialized || !isEntityCreated)
        {
            // �ʱ�ȭ ���� Ȯ�� �� �ʿ�� �ʱ�ȭ �õ�
            if (!isInitialized)
            {
                InitializeDOTSEntities();
                if (!isInitialized) // �ʱ�ȭ ���� ��
                {
                    Debug.LogWarning("DOTS ��ƼƼ �ý����� �ʱ�ȭ���� �ʾ� �߻��� �� �����ϴ�.");
                    return;
                }
            }
        }

        if (defaultWorld != null && defaultWorld.IsCreated && cannonEntity != Entity.Null)
        {
            // ��ƼƼ �Ŵ����� ��ȿ���� Ȯ��
            var entityManager = defaultWorld.EntityManager;
            if (!entityManager.Exists(cannonEntity))
            {
                // ��ƼƼ�� �������� ������ ����� �õ�
                Debug.LogWarning("ĳ�� ��ƼƼ�� ��ȿ���� �ʽ��ϴ�. ������� �õ��մϴ�.");
                InitializeDOTSEntities();
                if (!isEntityCreated)
                {
                    return; // ����� ����
                }
            }

            var cannonSystem = defaultWorld.GetOrCreateSystemManaged<CannonSystem>();
            if (cannonSystem != null)
            {
                cannonSystem.LaunchMissileImmediate(cannonEntity);
            }
        }
    }

    // Ư�� ��ġ���� �̻��� �߻�
    public virtual void LaunchMissileFromPosition(Vector3 position, Quaternion rotation)
    {
        if (!isInitialized || !isEntityCreated)
        {
            InitializeDOTSEntities();
            if (!isInitialized)
            {
                Debug.LogWarning("DOTS ��ƼƼ �ý����� �ʱ�ȭ���� �ʾ� �߻��� �� �����ϴ�.");
                return;
            }
        }

        if (defaultWorld != null && defaultWorld.IsCreated && cannonEntity != Entity.Null)
        {
            var entityManager = defaultWorld.EntityManager;

            if (entityManager.Exists(cannonEntity) && entityManager.HasComponent<CannonComponent>(cannonEntity))
            {
                // ĳ�� ������Ʈ ��������
                var cannonComponent = entityManager.GetComponentData<CannonComponent>(cannonEntity);

                // �ӽ÷� ��ġ�� ȸ�� ����
                cannonComponent.Position = new float3(position.x, position.y, position.z);
                cannonComponent.Rotation = new quaternion(rotation.x, rotation.y, rotation.z, rotation.w);

                // ������Ʈ�� ������Ʈ ����
                entityManager.SetComponentData(cannonEntity, cannonComponent);

                // �̻��� ��� �߻�
                LaunchMissileImmediate();

                // ���� ��ġ�� �ǵ�����
                UpdateTransform();
            }
        }
    }

    // �̻��� �ӵ� ���� �޼��� (���� API�� ȣȯ�� ����)
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

    // �̻��� �ӵ� ���� �޼��� (���� API�� ȣȯ�� ����)
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
        // ��Ȱ��ȭ �� �Ͻ������� ���� ���� (�ٽ� Ȱ��ȭ�� �� ����)
    }

    protected virtual void OnDestroy()
    {
        // ��ƼƼ ����
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