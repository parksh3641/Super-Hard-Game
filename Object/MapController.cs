using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public int index = 0;

    MeshRenderer meshRenderer;

    public Material[] materials;

    public bool isLeft = true;
    public bool isRight = true;
    public bool isTop = true;
    public bool isBottom = true;

    public Cannon[] cannonList;

    public CheckCollider leftCollider;
    public CheckCollider rightCollider;
    public CheckCollider topCollider;
    public CheckCollider bottomCollider;

    public GameObject left;
    public GameObject right;
    public GameObject top;
    public GameObject bottom;

    public GameObject laser;
    public GameObject safeZone;


    public void Awake()
    {
        if (GetComponent<MeshRenderer>() != null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        safeZone.SetActive(false);

        Invoke("Initialize", 0.5f);
    }

    private void Start()
    {
        GameManager.instance.AddMapController(this);
    }

    public void OnEnable()
    {
        Invoke("Initialize", 0.5f);

        GameManager.OnGameStart += OnGameStart;
    }

    public void OnDisable()
    {
        GameManager.OnGameStart -= OnGameStart;
    }

    public void OnGameStart()
    {

    }

    private void Initialize()
    {
        isLeft = leftCollider.isCreate;
        isRight = rightCollider.isCreate;
        isTop = topCollider.isCreate;
        isBottom = bottomCollider.isCreate;

        leftCollider.gameObject.SetActive(false);
        rightCollider.gameObject.SetActive(false);
        topCollider.gameObject.SetActive(false);
        bottomCollider.gameObject.SetActive(false);

        left.SetActive(isLeft);
        right.SetActive(isRight);
        top.SetActive(isTop);
        bottom.SetActive(isBottom);
    }

    public void SpeedUp()
    {
        for (int i = 0; i < cannonList.Length; i++)
        {
            cannonList[i].SpeedUp();
        }
    }

    public void SpeedDown()
    {
        for (int i = 0; i < cannonList.Length; i++)
        {
            cannonList[i].SpeedDown();
        }
    }

    public void SetLaser(int number)
    {
        if (meshRenderer != null && materials.Length > 0)
        {
            if (PlayerPrefs.GetInt("Stage") <= 3)
            {
                meshRenderer.material = materials[0];
            }
            else
            {
                meshRenderer.material = materials[1];
            }
        }

        laser.gameObject.SetActive(true);
        laser.transform.localScale = new Vector3(1, 0.3f, 1);

        switch (number)
        {
            case 0:
                laser.gameObject.SetActive(false);
                break;
            case 1:
                laser.transform.rotation = Quaternion.Euler(90, 90, 0);
                laser.transform.localPosition = new Vector3(-3, 0.1f, 0);
                break;
            case 2:
                laser.transform.rotation = Quaternion.Euler(90, 90, 0);
                laser.transform.localPosition = new Vector3(0, 0.1f, 0);
                break;
            case 3:
                laser.transform.rotation = Quaternion.Euler(90, 90, 0);
                laser.transform.localPosition = new Vector3(3, 0.1f, 0);
                break;
            case 4:
                laser.transform.rotation = Quaternion.Euler(90, 0, 0);
                laser.transform.localPosition = new Vector3(0, 0.1f, 3);
                break;
            case 5:
                laser.transform.rotation = Quaternion.Euler(90, 0, 0);
                laser.transform.localPosition = new Vector3(0, 0.1f, 0);
                break;
            case 6:
                laser.transform.rotation = Quaternion.Euler(90, 0, 0);
                laser.transform.localPosition = new Vector3(0, 0.1f, -3);
                break;
            case 7:
                laser.transform.rotation = Quaternion.Euler(90, 45, 0);
                laser.transform.localPosition = new Vector3(0, 0.1f, 0);
                laser.transform.localScale = new Vector3(1.35f, 0.3f, 1);
                break;
            case 8:
                laser.transform.rotation = Quaternion.Euler(90, 135, 0);
                laser.transform.localPosition = new Vector3(0, 0.1f, 0);
                laser.transform.localScale = new Vector3(1.35f, 0.3f, 1);
                break;
            default:
                laser.gameObject.SetActive(false);
                break;
        }
    }

    public void SetSafeZone()
    {
        safeZone.SetActive(true);

        transform.tag = "SafeZone";
    }

    public void SetIndex(int number)
    {
        index = number;
    }

    public int GetIndex()
    {
        return index;
    }
}
