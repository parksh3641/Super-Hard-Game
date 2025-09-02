using System.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Random = UnityEngine.Random;

// CannonDOTSManager�� ��ӹ޴� Cannon Ŭ����
public class Cannon : CannonDOTSManager
{
    private Missile[] missilePool; // ���� �̻��� Ǯ (ȣȯ���� ���� ����)
    private int currentMissileIndex = 0; // ���� Ǯ���� ����� �̻��� �ε���
    private float randomAngle;
    private Coroutine launchCoroutine;

    // ���� Cannon Ŭ������ �ִ� �ʵ�� CannonDOTSManager���� �̹� ��ӹ���
    // missilePrefab, missilePoolSize, launchInterval, randomAngleRange, missileSpeed

    // ���� Awake �޼��� �������̵� - ���̽� Ŭ���� ȣ�� ����
    protected override void Awake()
    {
        base.Awake(); // CannonDOTSManager�� Awake ȣ��
    }

    // ���� Start �޼��� �������̵� - ���̽� Ŭ���� ȣ�� ����
    protected override void Start()
    {
        // ���� �̻��� Ǯ �ʱ�ȭ (ȣȯ���� ���� ����)
        missilePool = new Missile[missilePoolSize];
        for (int i = 0; i < missilePoolSize; i++)
        {
            if (missilePrefab != null)
            {
                missilePool[i] = Instantiate(missilePrefab, transform.position, Quaternion.identity, transform);
                missilePool[i].gameObject.SetActive(false);
            }
        }

        // CannonDOTSManager�� Start ȣ�� (DOTS ��ƼƼ �ý��� �ʱ�ȭ)
        base.Start();
    }

    // ���� GameStart �޼��� �������̵� - ���̽� Ŭ���� ȣ�� ����
    protected override void GameStart()
    {
        base.GameStart(); // �⺻ GameStart ȣ��

        if (gameObject.activeInHierarchy)
        {
            // �̹� ���� ���� �ڷ�ƾ�� ������ ����
            if (launchCoroutine != null)
            {
                StopCoroutine(launchCoroutine);
            }

            // �� �ڷ�ƾ ����
            launchCoroutine = StartCoroutine(LaunchMissilesPeriodically());
        }
    }

    // ���� �ֱ��� �̻��� �߻� �ڷ�ƾ (���� ������� ����)
    private IEnumerator LaunchMissilesPeriodically()
    {
        while (true)
        {
            // GameStateManager�� ���ų� IsPlaying�� false�� ��� ���
            if (GameStateManager.instance != null && GameStateManager.instance.IsPlaying)
            {
                try
                {
                    // ���� ������� �̻��� �߻�
                    LaunchMissile();
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"�̻��� �߻� �� ���� �߻�: {e}");
                }
            }

            // ���� �߻���� ���
            yield return new WaitForSeconds(Random.Range(launchInterval * 0.8f, launchInterval * 1.2f));
        }
    }

    // ���� LaunchMissile �޼��� - ���� ������� �̻��� �߻�
    public virtual void LaunchMissile()
    {
        // ���� ������� �ٷ� �̻��� �߻�
        FallbackLaunchMissile(transform.position, transform.rotation);

        // ���� �̻��� Ǯ �ε��� ������Ʈ
        UpdateMissilePoolIndex();
    }

    // Ư�� ��ġ���� �̻��� �߻��ϴ� �޼��带 �������̵� (���� ��� ���)
    public override void LaunchMissileFromPosition(Vector3 position, Quaternion rotation)
    {
        // ���� ������� �ٷ� �̻��� �߻�
        FallbackLaunchMissile(position, rotation);

        // ���� �̻��� Ǯ �ε��� ������Ʈ
        UpdateMissilePoolIndex();
    }

    // ���� ������� �̻��� �߻��ϴ� �޼���
    private void FallbackLaunchMissile(Vector3 position, Quaternion rotation)
    {
        if (missilePool == null || missilePool.Length == 0 || currentMissileIndex >= missilePool.Length)
        {
            Debug.LogWarning("�̻��� Ǯ�� �ùٸ��� �ʱ�ȭ���� �ʾҽ��ϴ�.");
            return;
        }

        // �̻��� Ǯ���� �̻��� ��������
        Missile missile = missilePool[currentMissileIndex];
        if (missile != null)
        {
            // �̻��� �ʱ�ȭ
            missile.transform.position = position + new Vector3(0, 0.5f, 0);
            randomAngle = Random.Range(-randomAngleRange / 2, randomAngleRange / 2);
            missile.transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y + randomAngle, 0);
            missile.SetSpeed(missileSpeed);
            missile.gameObject.SetActive(true);

            Debug.Log("���� ������� �̻��� �߻�");
        }
    }

    // �̻��� Ǯ �ε��� ������Ʈ
    private void UpdateMissilePoolIndex()
    {
        if (missilePool != null && missilePool.Length > 0)
        {
            currentMissileIndex = (currentMissileIndex + 1) % missilePoolSize;
        }
    }

    // OnDestroy �������̵� - �ڷ�ƾ ���� �߰�
    protected override void OnDestroy()
    {
        if (launchCoroutine != null)
        {
            StopCoroutine(launchCoroutine);
            launchCoroutine = null;
        }

        base.OnDestroy();
    }

    // OnDisable �������̵� - �ڷ�ƾ ���� �߰�
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