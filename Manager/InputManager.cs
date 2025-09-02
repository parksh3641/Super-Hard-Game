using System.Collections;
using UnityEngine;
public class InputManager : MonoBehaviour
{
    public GameObject cubePrefab; // 표시할 큐브 프리팹
    public float cubeLifetime = 1f; // 큐브가 사라질 때까지의 시간
    private GameObject cubeInstance; // 미리 생성된 큐브 인스턴스
    private Coroutine hideCoroutine;
    private Vector3 moveDirection; // 이동 방향
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
        // 큐브 미리 생성 및 비활성화
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
        //    isKeyboardActive = true; // 키보드 입력 시작 → 터치 차단
        //}
        //// 마우스 클릭 입력 처리
        //if (!isKeyboardActive && Input.GetMouseButtonDown(0))
        //{
        //    ShowCubeAtClickPosition();
        //}

        // 마우스 왼쪽 클릭 처리
        if (Input.GetMouseButtonDown(0))
        {
            ShowCubeAtClickPosition(0); // 왼쪽 클릭
        }

        // 마우스 오른쪽 클릭 처리
        if (Input.GetMouseButtonDown(1))
        {
            ShowCubeAtClickPosition(1); // 오른쪽 클릭
        }

        // 키보드 WASD 입력 처리
        if (playerDataBase.Developer == 1)
        {
            HandleKeyboardInput();
        }
        // 이동 처리
        MovePlayer();
    }

    private void ShowCubeAtClickPosition(int mouseButton)
    {
        // 마우스 클릭한 위치를 화면 좌표에서 월드 좌표로 변환
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject.tag == "Player" || hit.collider.gameObject.tag == "UI")
            {
                // 자신의 오브젝트를 클릭한 경우 아무 작업도 하지 않음
                return;
            }
            // 큐브를 클릭한 위치로 이동 및 활성화
            cubeInstance.transform.position = hit.point;
            cubeInstance.SetActive(false);
            cubeInstance.SetActive(true);
            // 기존 숨김 코루틴 취소
            if (hideCoroutine != null)
            {
                StopCoroutine(hideCoroutine);
            }
            // 숨김 코루틴 시작
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
        // 이동 처리
        if (moveDirection != Vector3.zero)
        {
            // 이동
            move = moveDirection * Time.deltaTime;
            player.transform.position += move;
            // 회전 (이동 방향으로 캐릭터를 바라보게 설정)
            toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, toRotation, 0.3f); // 부드럽게 회전
        }
    }
}