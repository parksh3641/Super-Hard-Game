using UnityEngine;

public class MouseSensitivityManager : MonoBehaviour
{
    float mouseX, mouseY = 0;

    Vector3 newMousePosition;

    void Update()
    {
        mouseX = Input.GetAxis("Mouse X") * GameStateManager.instance.MouseSensitivity;
        mouseY = Input.GetAxis("Mouse Y") * GameStateManager.instance.MouseSensitivity;;
    }
}
