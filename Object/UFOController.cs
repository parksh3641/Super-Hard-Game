using System.Collections;
using UnityEngine;

public class UFOController : MonoBehaviour
{
    [Header("UFO Settings")]
    public GameObject ufoPrefab;
    private float moveSpeed = 3f;
    public float waitTime = 1f;
    private float fixedY = 1f;

    private GameObject ufoInstance1, ufoInstance2, ufoInstance3; // 최대 3개의 UFO 인스턴스
    private Vector3 startPoint1, startPoint2, startPoint3;
    private Vector3 endPoint1, endPoint2, endPoint3;
    private bool isMoving1 = false, isMoving2 = false, isMoving3 = false;

    [SerializeField] private Vector2 ufoSettings;
    [SerializeField] private Vector2 ufoSettings2;
    [SerializeField] private Vector2 ufoSettings3;

    private void Awake()
    {
        moveSpeed = Random.Range(moveSpeed * 0.8f, moveSpeed * 1.2f);
    }

    /// <summary>
    /// (0,0) 설정값 저장
    /// </summary>
    public void SetUFOSettings(int setting1, int setting2)
    {
        ufoSettings = new Vector2(setting1, setting2);
    }

    public void SetUFOSettings2(int setting1, int setting2)
    {
        ufoSettings2 = new Vector2(setting1, setting2);
    }

    public void SetUFOSettings3(int setting1, int setting2)
    {
        ufoSettings3 = new Vector2(setting1, setting2);
    }

    /// <summary>
    /// UFO1 경로 초기화
    /// </summary>
    public void InitializePath1(Vector3 start, Vector3 end)
    {
        startPoint1 = new Vector3(start.x, fixedY, start.z);
        endPoint1 = new Vector3(end.x, fixedY, end.z);

        if (ufoInstance1 == null)
        {
            ufoInstance1 = Instantiate(ufoPrefab, startPoint1, Quaternion.identity);
            ufoInstance1.SetActive(false);
            Debug.Log($"✅ UFO1 Instance Created at: {startPoint1}");
        }

        StartCoroutine(UFORoutine1());
    }

    /// <summary>
    /// UFO2 경로 초기화
    /// </summary>
    public void InitializePath2(Vector3 start, Vector3 end)
    {
        startPoint2 = new Vector3(start.x, fixedY, start.z);
        endPoint2 = new Vector3(end.x, fixedY, end.z);

        if (ufoInstance2 == null)
        {
            ufoInstance2 = Instantiate(ufoPrefab, startPoint2, Quaternion.identity);
            ufoInstance2.SetActive(false);
            Debug.Log($"✅ UFO2 Instance Created at: {startPoint2}");
        }

        StartCoroutine(UFORoutine2());
    }

    /// <summary>
    /// UFO3 경로 초기화
    /// </summary>
    public void InitializePath3(Vector3 start, Vector3 end)
    {
        startPoint3 = new Vector3(start.x, fixedY, start.z);
        endPoint3 = new Vector3(end.x, fixedY, end.z);

        if (ufoInstance3 == null)
        {
            ufoInstance3 = Instantiate(ufoPrefab, startPoint3, Quaternion.identity);
            ufoInstance3.SetActive(false);
            Debug.Log($"✅ UFO3 Instance Created at: {startPoint3}");
        }

        StartCoroutine(UFORoutine3());
    }

    /// <summary>
    /// UFO1 이동 루틴
    /// </summary>
    IEnumerator UFORoutine1()
    {
        yield return new WaitForSeconds(3f);
        while (true)
        {
            ufoInstance1.transform.position = startPoint1;
            ufoInstance1.SetActive(true);
            ufoInstance1.transform.LookAt(endPoint1);
            Debug.Log($"🚀 UFO1 Activated at: {startPoint1}");

            yield return StartCoroutine(MoveToTarget(ufoInstance1, endPoint1));
            ufoInstance1.SetActive(false);

            yield return new WaitForSeconds(waitTime);
        }
    }

    /// <summary>
    /// UFO2 이동 루틴
    /// </summary>
    IEnumerator UFORoutine2()
    {
        yield return new WaitForSeconds(3f);
        while (true)
        {
            ufoInstance2.transform.position = startPoint2;
            ufoInstance2.SetActive(true);
            ufoInstance2.transform.LookAt(endPoint2);
            Debug.Log($"🚀 UFO2 Activated at: {startPoint2}");

            yield return StartCoroutine(MoveToTarget(ufoInstance2, endPoint2));
            ufoInstance2.SetActive(false);

            yield return new WaitForSeconds(waitTime);
        }
    }

    /// <summary>
    /// UFO3 이동 루틴
    /// </summary>
    IEnumerator UFORoutine3()
    {
        yield return new WaitForSeconds(3f);
        while (true)
        {
            ufoInstance3.transform.position = startPoint3;
            ufoInstance3.SetActive(true);
            ufoInstance3.transform.LookAt(endPoint3);
            Debug.Log($"🚀 UFO3 Activated at: {startPoint3}");

            yield return StartCoroutine(MoveToTarget(ufoInstance3, endPoint3));
            ufoInstance3.SetActive(false);

            yield return new WaitForSeconds(waitTime);
        }
    }

    /// <summary>
    /// 목표 지점으로 이동 (XZ축만 사용, Y축 고정)
    /// </summary>
    IEnumerator MoveToTarget(GameObject ufo, Vector3 target)
    {
        while (Vector3.Distance(
            new Vector3(ufo.transform.position.x, 0, ufo.transform.position.z),
            new Vector3(target.x, 0, target.z)
        ) > 0.1f)
        {
            Vector3 currentPosition = ufo.transform.position;
            Vector3 targetPosition = new Vector3(target.x, fixedY, target.z);

            ufo.transform.position = Vector3.MoveTowards(
                currentPosition,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }
    }
}
