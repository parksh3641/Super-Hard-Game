using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager instance;

    public GameObject particlePrefab; // ��ƼŬ ������
    public int poolSize = 30; // ������Ʈ Ǯ ũ��
    private Queue<GameObject> particlePool; // ������Ʈ Ǯ

    void Awake()
    {
        instance = this;

        // ������Ʈ Ǯ �ʱ�ȭ
        particlePool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject particle = Instantiate(particlePrefab);
            particle.SetActive(false);
            particle.transform.parent = transform;
            particlePool.Enqueue(particle);
        }
    }

    // ��ƼŬ ��������
    public GameObject GetParticle(Vector3 position, Quaternion rotation)
    {
        if (particlePool.Count > 0)
        {
            GameObject particle = particlePool.Dequeue();
            particle.transform.position = position;
            particle.transform.rotation = rotation;
            particle.SetActive(true);

            // ��ƼŬ ��� �� �ڵ� ��Ȱ��ȭ
            StartCoroutine(DeactivateAfterPlay(particle));
            return particle;
        }
        else
        {
            Debug.LogWarning("No available particles in pool!");
            return null;
        }
    }

    // ��ƼŬ ��Ȱ��ȭ �� Ǯ�� ��ȯ
    private IEnumerator<WaitForSeconds> DeactivateAfterPlay(GameObject particle)
    {
        ParticleSystem ps = particle.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            yield return new WaitForSeconds(ps.main.duration);
        }
        particle.SetActive(false);
        particlePool.Enqueue(particle);
    }
}
