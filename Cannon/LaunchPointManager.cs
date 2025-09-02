using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// ���� �߻� �������� DOTS ��� �̻��� �ý����� �����ϴ� Ŭ����
/// </summary>
public class LaunchPointManager : MonoBehaviour
{
    public CannonDOTSManager cannonManager; // DOTS ĳ�� �Ŵ��� ����
    public Transform[] launchPoints; // �߻� ������
    public float launchInterval = 3f; // �߻� ����

    private PlayerDataBase playerDataBase;
    private float nextLaunchTime = 0f;
    private bool initialized = false;

    private void Awake()
    {
        if (playerDataBase == null)
            playerDataBase = Resources.Load("PlayerDataBase") as PlayerDataBase;

        // ĳ�� �Ŵ����� �������� ���� ���, ���� ������Ʈ�� �ڽĿ��� ã��
        if (cannonManager == null)
        {
            cannonManager = GetComponent<CannonDOTSManager>();
            if (cannonManager == null)
                cannonManager = GetComponentInChildren<CannonDOTSManager>();
        }
    }

    private void Start()
    {
        // ���� �õ� �ʱ�ȭ
        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);

        // �߻� ������ ���ǵ��� �ʾҴٸ� �ڽ��� �߻� �������� �߰�
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
            // ���������� ���� �̻��� �ӵ� ���� - ���� CannonSystem ȣ��
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

        // �߻� �ð��� �Ǿ����� Ȯ��
        if (Time.time >= nextLaunchTime)
        {
            LaunchMissileFromRandomPoint();

            // ���� �߻� �ð� ��� (���� ����)
            nextLaunchTime = Time.time + UnityEngine.Random.Range(launchInterval * 0.8f, launchInterval * 1.2f);
        }
    }

    // ������ �߻� �������� �̻��� �߻�
    public void LaunchMissileFromRandomPoint()
    {
        if (launchPoints == null || launchPoints.Length == 0 || cannonManager == null)
            return;

        // ���� �߻� ���� ����
        int randomIndex = UnityEngine.Random.Range(0, launchPoints.Length);
        Transform launchPoint = launchPoints[randomIndex];

        // ���õ� �������� �̻��� �߻�
        if (launchPoint != null)
        {
            LaunchMissileFromPoint(launchPoint);
        }
    }

    // Ư�� �߻� �������� �̻��� �߻�
    public void LaunchMissileFromPoint(Transform launchPoint)
    {
        if (launchPoint == null || cannonManager == null)
            return;

        // �߻� ��ġ�� ȸ���� ����Ͽ� �̻��� �߻�
        cannonManager.LaunchMissileFromPosition(launchPoint.position, launchPoint.rotation);
    }

    // Ư�� ��ġ���� ���� �̻��� �߻�
    public void LaunchMissileFromPosition(Vector3 position, Quaternion rotation)
    {
        if (cannonManager == null)
            return;

        cannonManager.LaunchMissileFromPosition(position, rotation);
    }

    // �̻��� �ӵ� ����
    public void SpeedUp()
    {
        if (cannonManager != null)
        {
            cannonManager.SpeedUp();
        }
    }

    // �̻��� �ӵ� ����
    public void SpeedDown()
    {
        if (cannonManager != null)
        {
            cannonManager.SpeedDown();
        }
    }

    // �߻� ���� ����
    public void SetLaunchInterval(float interval)
    {
        launchInterval = Mathf.Max(0.1f, interval);
    }
}