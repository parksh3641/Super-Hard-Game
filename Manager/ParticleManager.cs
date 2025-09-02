using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager instance;

    public GameObject particlePrefab; // 파티클 프리팹
    public int poolSize = 30; // 오브젝트 풀 크기
    private Queue<GameObject> particlePool; // 오브젝트 풀

    void Awake()
    {
        instance = this;

        // 오브젝트 풀 초기화
        particlePool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject particle = Instantiate(particlePrefab);
            particle.SetActive(false);
            particle.transform.parent = transform;
            particlePool.Enqueue(particle);
        }
    }

    // 파티클 가져오기
    public GameObject GetParticle(Vector3 position, Quaternion rotation)
    {
        if (particlePool.Count > 0)
        {
            GameObject particle = particlePool.Dequeue();
            particle.transform.position = position;
            particle.transform.rotation = rotation;
            particle.SetActive(true);

            // 파티클 재생 후 자동 비활성화
            StartCoroutine(DeactivateAfterPlay(particle));
            return particle;
        }
        else
        {
            Debug.LogWarning("No available particles in pool!");
            return null;
        }
    }

    // 파티클 비활성화 후 풀에 반환
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
