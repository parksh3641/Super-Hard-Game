using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.AudioSettings;

public class PlayerController : MonoBehaviour
{
    public GameObject model;
    public BoxCollider boxCollider;
    public Animator animator;

    [SerializeField] private float currentSpeed = 0f; // ���� �ӵ�
    public float maxSpeed = 5.8f; // �ִ� �̵� �ӵ�
    public float acceleration = 25f; // ���ӵ�
    public float deceleration = 25f; // ���ӵ�
    public float rotationSpeed = 910f; // ȸ�� �ӵ� (��/��)
    public float size = 5.5f;

    private Vector3 direction;
    private Vector3 scale;
    private Vector3 targetPosition; // ��ǥ ����
    private Quaternion targetRotation; // ��ǥ ȸ��
    private bool isMoving = false; // �̵� ������ ����
    private bool isRotating = false; // ȸ�� ������ ����

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
        // �ʱ� ��ġ�� ���� ��ġ�� ����
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
            // ���콺 Ŭ�� �Է� ó��
            if (Input.GetMouseButtonDown(0))
            {
                SetTargetPosition();
            }

            if (Input.GetMouseButtonDown(1))
            {
                SetTargetPosition();
            }

            // ��ǥ �������� �̵� ó��
            if (isMoving)
            {
                MoveToTarget();
            }
            else
            {
                ApplyDeceleration();
            }

            // ȸ�� ó��
            if (isRotating)
            {
                RotateToTarget();
            }
        }

        UpdateAnimator();
    }

    private void SetTargetPosition()
    {
        // ���콺 Ŭ���� ��ġ�� ȭ�� ��ǥ���� ���� ��ǥ�� ��ȯ
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // UI ���� Ŭ���� ��� �ƹ� �۾��� ���� ����
                return;
            }

            if (hit.collider.gameObject == this.gameObject)
            {
                // �ڽ��� ������Ʈ�� Ŭ���� ��� �ƹ� �۾��� ���� ����
                return;
            }

            // ��Ʈ�� ������ X-Z ��ǥ�� ��ǥ �������� ����
            targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            isMoving = true;

            // ��ǥ ȸ�� ����
            direction = (targetPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                targetRotation = Quaternion.LookRotation(direction);
                isRotating = true;
            }

            // X�� ���⿡ ���� ĳ������ ���� ������ ����
            if (targetPosition.x < transform.position.x)
            {
                // �������� �̵� ��
                scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * -1; // X�� ����
                transform.localScale = scale;
            }
            else if (targetPosition.x > transform.position.x)
            {
                // ���������� �̵� ��
                scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x); // X�� ���󺹱�
                transform.localScale = scale;
            }
        }
    }


    private void MoveToTarget()
    {
        // ���ӵ� ���� (���� �ӵ��� ���������� ����)
        currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);

        // ���� ��ġ���� ��ǥ �������� �̵�
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);

        // ��ǥ ������ �����ϸ� �̵� �ߴ�
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isMoving = false;
        }
    }

    private void MoveWithJoystick(Vector2 joystickInput)
    {
        // ���� ���� ���
        Vector3 moveDirection = new Vector3(joystickInput.x, 0, joystickInput.y).normalized;

        // ���ӵ� ����
        currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);

        // �̵�
        transform.position += moveDirection * currentSpeed * Time.deltaTime;

        // ĳ���� ȸ��
        if (moveDirection != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void ApplyDeceleration()
    {
        // �̵����� ���� �� �ӵ� ����
        if (currentSpeed > 0)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
        }
    }

    private void RotateToTarget()
    {
        // ��ǥ ȸ������ �ε巴�� ȸ��
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // ��ǥ ȸ���� �����ߴ��� Ȯ��
        if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
        {
            isRotating = false;
        }
    }

    private void UpdateAnimator()
    {
        // ���� ĳ������ Y�� ȸ���� ��������
        float rotationY = transform.rotation.eulerAngles.y;

        // ���� ������ Ȯ��
        if ((rotationY >= 320f && rotationY <= 360f) ||
            (rotationY >= 0f && rotationY <= 40f) ||
            (rotationY >= 140f && rotationY <= 220f))
        {
            // ������ ���� ������ ���� ��
            animator.SetBool("Vector", true);
        }
        else
        {
            // ������ ���� ������ ����� ��
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
                int currentIndex = mapController.GetIndex(); // ���� Ÿ���� �ε��� ��������
                GameManager.instance.UpdatePlayerProgress(currentIndex); // GameManager�� ����
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
