using System.Collections;
using UnityEngine;
public class InputManager : MonoBehaviour
{
    public GameObject cubePrefab; // ǥ���� ť�� ������
    public float cubeLifetime = 1f; // ť�갡 ����� �������� �ð�
    private GameObject cubeInstance; // �̸� ������ ť�� �ν��Ͻ�
    private Coroutine hideCoroutine;
    private Vector3 moveDirection; // �̵� ����
    public GameObject player;
    Vector3 move;
    Quaternion toRotation;
    float horizontal, vertical;
    bool isKeyboardActive = false;
    bool hasKeyboardInput = false;
    PlayerDataBase playerDataBase;
    private void Awake()
    {
        if (playerDataBase == null) playerDataBase = Resources.Load("PlayerDataBase") as PlayerDataBase;
    }
    void Start()
    {
        // ť�� �̸� ���� �� ��Ȱ��ȭ
        cubeInstance = Instantiate(cubePrefab);
        cubeInstance.SetActive(false);
    }
    void Update()
    {
        if (!GameStateManager.instance.IsPlaying) return;
        //isKeyboardActive = false;
        //hasKeyboardInput = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
        //if (hasKeyboardInput)
        //{
        //    isKeyboardActive = true; // Ű���� �Է� ���� �� ��ġ ����
        //}
        //// ���콺 Ŭ�� �Է� ó��
        //if (!isKeyboardActive && Input.GetMouseButtonDown(0))
        //{
        //    ShowCubeAtClickPosition();
        //}

        // ���콺 ���� Ŭ�� ó��
        if (Input.GetMouseButtonDown(0))
        {
            ShowCubeAtClickPosition(0); // ���� Ŭ��
        }

        // ���콺 ������ Ŭ�� ó��
        if (Input.GetMouseButtonDown(1))
        {
            ShowCubeAtClickPosition(1); // ������ Ŭ��
        }

        // Ű���� WASD �Է� ó��
        if (playerDataBase.Developer == 1)
        {
            HandleKeyboardInput();
        }
        // �̵� ó��
        MovePlayer();
    }

    private void ShowCubeAtClickPosition(int mouseButton)
    {
        // ���콺 Ŭ���� ��ġ�� ȭ�� ��ǥ���� ���� ��ǥ�� ��ȯ
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject.tag == "Player" || hit.collider.gameObject.tag == "UI")
            {
                // �ڽ��� ������Ʈ�� Ŭ���� ��� �ƹ� �۾��� ���� ����
                return;
            }
            // ť�긦 Ŭ���� ��ġ�� �̵� �� Ȱ��ȭ
            cubeInstance.transform.position = hit.point;
            cubeInstance.SetActive(false);
            cubeInstance.SetActive(true);
            // ���� ���� �ڷ�ƾ ���
            if (hideCoroutine != null)
            {
                StopCoroutine(hideCoroutine);
            }
            // ���� �ڷ�ƾ ����
            hideCoroutine = StartCoroutine(HideCubeAfterDelay());
        }
    }

    private IEnumerator HideCubeAfterDelay()
    {
        yield return new WaitForSeconds(cubeLifetime);
        cubeInstance.SetActive(false);
    }

    private void HandleKeyboardInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector3(horizontal, 0, vertical).normalized;
    }

    private void MovePlayer()
    {
        // �̵� ó��
        if (moveDirection != Vector3.zero)
        {
            // �̵�
            move = moveDirection * Time.deltaTime;
            player.transform.position += move;
            // ȸ�� (�̵� �������� ĳ���͸� �ٶ󺸰� ����)
            toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, toRotation, 0.3f); // �ε巴�� ȸ��
        }
    }
}