using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.AudioSettings;

public class PlayerController : MonoBehaviour
{
    public GameObject model;
    public BoxCollider boxCollider;
    public Animator animator;

    [SerializeField] private float currentSpeed = 0f; // 현재 속도
    public float maxSpeed = 5.8f; // 최대 이동 속도
    public float acceleration = 25f; // 가속도
    public float deceleration = 25f; // 감속도
    public float rotationSpeed = 910f; // 회전 속도 (도/초)
    public float size = 5.5f;

    private Vector3 direction;
    private Vector3 scale;
    private Vector3 targetPosition; // 목표 지점
    private Quaternion targetRotation; // 목표 회전
    private bool isMoving = false; // 이동 중인지 여부
    private bool isRotating = false; // 회전 중인지 여부

    [SerializeField]
    private bool safeZone = false;
    [SerializeField]
    private bool isGameOver = false;
    [SerializeField]
    private bool invincibility = false;

    [Header("Particle")]
    public ParticleSystem bombParticle;

    [Header("Joystick")]
    public Joystick joystick;

    Vector2 joystickInput;
    Vector2 gamepadInput;

    private bool isMobile = false;
    private bool isUsingGamepad = false;

    private void Awake()
    {
        safeZone = false;
        isGameOver = false;

        boxCollider.enabled = true;

        bombParticle.gameObject.SetActive(false);

        size = model.transform.localScale.x;
    }

    void Start()
    {
        // 초기 위치를 현재 위치로 설정
        targetPosition = transform.position;
        targetRotation = transform.rotation;

        maxSpeed = GameStateManager.instance.PlayerMoveSpeed;
        acceleration = GameStateManager.instance.PlayerAcceleration;
        deceleration = GameStateManager.instance.PlayerAcceleration;
        rotationSpeed = GameStateManager.instance.PlayerRotationalSpeed;

        isMobile = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer);
    }

    public void SaveSetting()
    {
        GameStateManager.instance.PlayerMoveSpeed = maxSpeed;
        GameStateManager.instance.PlayerAcceleration = acceleration;
        GameStateManager.instance.PlayerRotationalSpeed = rotationSpeed;
    }

    public void ResetSetting()
    {
        maxSpeed = 5.8f;
        acceleration = 25f;
        deceleration = 25f;
        rotationSpeed = 910;  

        GameStateManager.instance.PlayerMoveSpeed = maxSpeed;
        GameStateManager.instance.PlayerAcceleration = acceleration;
        GameStateManager.instance.PlayerRotationalSpeed = rotationSpeed;
    }

    public void SpeedUp()
    {
        maxSpeed += 0.1f;
    }

    public void SpeedDown()
    {
        maxSpeed -= 0.1f;

        if(maxSpeed <= 0.5)
        {
            maxSpeed = 0.5f;
        }
    }

    public void AccelerationUp()
    {
        acceleration += 0.5f;
        deceleration += 0.5f;
    }

    public void AccelerationDown()
    {
        acceleration -= 0.5f;
        deceleration -= 0.5f;

        if (acceleration <= 1)
        {
            acceleration = 1;
            deceleration = 1;
        }
    }

    public void RotationalSpeedUp()
    {
        rotationSpeed += 10f;
    }

    public void RotationalSpeedDown()
    {
        rotationSpeed -= 10f;

        if (rotationSpeed <= 10)
        {
            rotationSpeed = 10;
        }
    }

    public void PlayerSizeUp()
    {
        size += 0.5f;

        model.transform.localScale = new Vector3(size, size, size);
    }

    public void PlayerSizeDown()
    {
        size -= 0.5f;

        if(size <= 1)
        {
            size = 1;
        }

        model.transform.localScale = new Vector3(size, size, size);
    }


    public void Invincibility()
    {
        if(!invincibility)
        {
            invincibility = true;
        }
        else
        {
            invincibility = false;
        }
    }

    void Update()
    {
        if (!GameStateManager.instance.IsPlaying) return;
        if (isGameOver) return;

        //isUsingGamepad = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f;

        if (isMobile)
        {
            joystickInput = joystick.GetJoystickInput();
            if (joystickInput.magnitude > 0.1f)
            {
                MoveWithJoystick(joystickInput);
            }
            else
            {
                ApplyDeceleration();
            }
        }
        //else if (isUsingGamepad)
        //{
        //    gamepadInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        //    if (gamepadInput.magnitude > 0.1f)
        //    {
        //        MoveWithJoystick(gamepadInput);
        //    }
        //    else
        //    {
        //        ApplyDeceleration();
        //    }
        //}
        else
        {
            // 마우스 클릭 입력 처리
            if (Input.GetMouseButtonDown(0))
            {
                SetTargetPosition();
            }

            if (Input.GetMouseButtonDown(1))
            {
                SetTargetPosition();
            }

            // 목표 지점으로 이동 처리
            if (isMoving)
            {
                MoveToTarget();
            }
            else
            {
                ApplyDeceleration();
            }

            // 회전 처리
            if (isRotating)
            {
                RotateToTarget();
            }
        }

        UpdateAnimator();
    }

    private void SetTargetPosition()
    {
        // 마우스 클릭한 위치를 화면 좌표에서 월드 좌표로 변환
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // UI 위를 클릭한 경우 아무 작업도 하지 않음
                return;
            }

            if (hit.collider.gameObject == this.gameObject)
            {
                // 자신의 오브젝트를 클릭한 경우 아무 작업도 하지 않음
                return;
            }

            // 히트된 지점의 X-Z 좌표를 목표 지점으로 설정
            targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            isMoving = true;

            // 목표 회전 설정
            direction = (targetPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                targetRotation = Quaternion.LookRotation(direction);
                isRotating = true;
            }

            // X축 방향에 따라 캐릭터의 로컬 스케일 반전
            if (targetPosition.x < transform.position.x)
            {
                // 왼쪽으로 이동 중
                scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * -1; // X축 반전
                transform.localScale = scale;
            }
            else if (targetPosition.x > transform.position.x)
            {
                // 오른쪽으로 이동 중
                scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x); // X축 원상복구
                transform.localScale = scale;
            }
        }
    }


    private void MoveToTarget()
    {
        // 가속도 적용 (현재 속도를 점진적으로 증가)
        currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);

        // 현재 위치에서 목표 지점으로 이동
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);

        // 목표 지점에 도달하면 이동 중단
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isMoving = false;
        }
    }

    private void MoveWithJoystick(Vector2 joystickInput)
    {
        // 방향 벡터 계산
        Vector3 moveDirection = new Vector3(joystickInput.x, 0, joystickInput.y).normalized;

        // 가속도 적용
        currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);

        // 이동
        transform.position += moveDirection * currentSpeed * Time.deltaTime;

        // 캐릭터 회전
        if (moveDirection != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void ApplyDeceleration()
    {
        // 이동하지 않을 때 속도 감소
        if (currentSpeed > 0)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
        }
    }

    private void RotateToTarget()
    {
        // 목표 회전으로 부드럽게 회전
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // 목표 회전에 도달했는지 확인
        if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
        {
            isRotating = false;
        }
    }

    private void UpdateAnimator()
    {
        // 현재 캐릭터의 Y축 회전값 가져오기
        float rotationY = transform.rotation.eulerAngles.y;

        // 각도 범위를 확인
        if ((rotationY >= 320f && rotationY <= 360f) ||
            (rotationY >= 0f && rotationY <= 40f) ||
            (rotationY >= 140f && rotationY <= 220f))
        {
            // 지정된 각도 범위에 있을 때
            animator.SetBool("Vector", true);
        }
        else
        {
            // 지정된 각도 범위를 벗어났을 때
            animator.SetBool("Vector", false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Start")
        {
            GameManager.instance.GameStart();
        }

        if (other.gameObject.tag == "Missile")
        {
            GameOver(1);
        }

        if (other.gameObject.tag == "UFO")
        {
            GameOver(3);
        }

        if (other.gameObject.CompareTag("Map"))
        {
            MapController mapController = other.gameObject.GetComponent<MapController>();
            if (mapController != null)
            {
                int currentIndex = mapController.GetIndex(); // 현재 타일의 인덱스 가져오기
                GameManager.instance.UpdatePlayerProgress(currentIndex); // GameManager로 전달
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "SafeZone")
        {
            safeZone = true;
        }
        else if (other.gameObject.tag == "Map")
        {
            safeZone = false;
        }
        else if (other.gameObject.tag == "Goal")
        {
            safeZone = true;

            isGameOver = true;
            GameManager.instance.GameClear();
        }
        else if(other.gameObject.tag == "Laser")
        {
            GameOver(2);
        }
        else if (other.gameObject.tag == "Dead" || other.gameObject.tag == "LastLine")
        {
            FastGameOver();
        }
    }

    public void GameOver(int number)
    {
        if (invincibility) return;

        if (safeZone) return;

        if (isGameOver) return;

        SetGameOver(number);
    }

    public void FastGameOver()
    {
        if (invincibility) return;

        if (isGameOver) return;

        SetGameOver(0);
    }

    void SetGameOver(int number)
    {
        model.transform.localPosition = new Vector3(0, 0, -4);
        model.transform.rotation = Quaternion.Euler(-25, transform.eulerAngles.y, transform.eulerAngles.z);

        boxCollider.enabled = false;

        bombParticle.gameObject.SetActive(true);

        animator.SetBool("Dead", true);

        isGameOver = true;
        GameManager.instance.GameOver(number);
    }
}
