using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamepadUIController : MonoBehaviour
{
    public Button[] buttonArray; // 버튼 배열
    public int defaultSelectedIndex = 0; // 기본 선택 인덱스

    private int selectedIndex; // 현재 선택된 버튼 인덱스
    private float inputCooldown = 0.2f; // 입력 간격 제한
    private float lastInputTime = 0f;

    void Start()
    {
        selectedIndex = Mathf.Clamp(defaultSelectedIndex, 0, buttonArray.Length - 1); // 기본 선택값 적용
        SelectButton(selectedIndex);
    }

    void Update()
    {
        float vertical = Input.GetAxisRaw("Vertical");
        bool upPressed = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
        bool downPressed = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);

        if (Time.time - lastInputTime > inputCooldown) // 입력 간격 제한
        {
            if (vertical > 0.1f || upPressed) // 위로 이동
            {
                ChangeSelection(-1);
                lastInputTime = Time.time;
            }
            else if (vertical < -0.1f || downPressed) // 아래로 이동
            {
                ChangeSelection(1);
                lastInputTime = Time.time;
            }
        }

        if (Input.GetButtonDown("Submit") || Input.GetKeyDown(KeyCode.Return)) // 버튼 클릭
        {
            ClickSelectedButton();
        }
    }

    private void ChangeSelection(int direction)
    {
        selectedIndex = (selectedIndex + direction + buttonArray.Length) % buttonArray.Length;
        SelectButton(selectedIndex);
    }

    private void SelectButton(int index)
    {
        if (buttonArray.Length == 0) return;

        Debug.Log(index + "번 선택됨");

        Button button = buttonArray[index];
        EventSystem.current.SetSelectedGameObject(button.gameObject);

        button.OnSelect(null);
    }

    private void ClickSelectedButton()
    {
        if (buttonArray.Length == 0) return;

        Button selectedButton = buttonArray[selectedIndex];
        if (selectedButton != null)
        {
            selectedButton.onClick.Invoke();
        }
    }
}
