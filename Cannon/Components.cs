using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Collections;

// �̻��� ������Ʈ
public struct MissileComponent : IComponentData
{
    public float Speed;
    public float InitialDelay;
    public float CurrentDelay;
    public bool IsActive;
    public bool HasFire;
}

// ĳ�� ������Ʈ
public struct CannonComponent : IComponentData
{
    public float LaunchInterval;
    public float CurrentLaunchTime;
    public float RandomAngleRange;
    public float MissileSpeed;
    public int MissilePoolSize;
    public Entity MissilePrefab;
    public float3 Position;
    public quaternion Rotation;
    public uint RandomSeed;
    public float3 LaunchOffset; // �߻� ���� ������
}

// �±� ������Ʈ (�浹 ������)
public struct PlayerTag : IComponentData { }
public struct SafeZoneTag : IComponentData { }
public struct LastLineTag : IComponentData { }

// �±� Authoring Ŭ������
public class PlayerTagAuthoring : MonoBehaviour
{
    public class Baker : Baker<PlayerTagAuthoring>
    {
        public override void Bake(PlayerTagAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<PlayerTag>(entity);
        }
    }
}

public class SafeZoneTagAuthoring : MonoBehaviour
{
    public class Baker : Baker<SafeZoneTagAuthoring>
    {
        public override void Bake(SafeZoneTagAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<SafeZoneTag>(entity);
        }
    }
}

public class LastLineTagAuthoring : MonoBehaviour
{
    public class Baker : Baker<LastLineTagAuthoring>
    {
        public override void Bake(LastLineTagAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<LastLineTag>(entity);
        }
    }
}

// �÷��̾� �±� ������Ʈ�� ���� �÷��̾� GameObject�� �߰��ϱ� ���� Ŭ����
public class PlayerDOTSTag : MonoBehaviour
{
    private void Awake()
    {
        // PlayerTagAuthoring ������Ʈ�� ���ٸ� �߰�
        if (GetComponent<PlayerTagAuthoring>() == null)
        {
            gameObject.AddComponent<PlayerTagAuthoring>();
        }
    }
}

// SafeZone �±� ������Ʈ�� ���� SafeZone GameObject�� �߰��ϱ� ���� Ŭ����
public class SafeZoneDOTSTag : MonoBehaviour
{
    private void Awake()
    {
        if (GetComponent<SafeZoneTagAuthoring>() == null)
        {
            gameObject.AddComponent<SafeZoneTagAuthoring>();
        }
    }
}

// LastLine �±� ������Ʈ�� ���� LastLine GameObject�� �߰��ϱ� ���� Ŭ����
public class LastLineDOTSTag : MonoBehaviour
{
    private void Awake()
    {
        if (GetComponent<LastLineTagAuthoring>() == null)
        {
            gameObject.AddComponent<LastLineTagAuthoring>();
        }
    }
}