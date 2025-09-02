using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Collections;

// 미사일 컴포넌트
public struct MissileComponent : IComponentData
{
    public float Speed;
    public float InitialDelay;
    public float CurrentDelay;
    public bool IsActive;
    public bool HasFire;
}

// 캐논 컴포넌트
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
    public float3 LaunchOffset; // 발사 지점 오프셋
}

// 태그 컴포넌트 (충돌 감지용)
public struct PlayerTag : IComponentData { }
public struct SafeZoneTag : IComponentData { }
public struct LastLineTag : IComponentData { }

// 태그 Authoring 클래스들
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

// 플레이어 태그 컴포넌트를 기존 플레이어 GameObject에 추가하기 위한 클래스
public class PlayerDOTSTag : MonoBehaviour
{
    private void Awake()
    {
        // PlayerTagAuthoring 컴포넌트가 없다면 추가
        if (GetComponent<PlayerTagAuthoring>() == null)
        {
            gameObject.AddComponent<PlayerTagAuthoring>();
        }
    }
}

// SafeZone 태그 컴포넌트를 기존 SafeZone GameObject에 추가하기 위한 클래스
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

// LastLine 태그 컴포넌트를 기존 LastLine GameObject에 추가하기 위한 클래스
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