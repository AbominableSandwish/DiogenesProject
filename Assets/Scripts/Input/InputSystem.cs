using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystem : MonoBehaviour
{
    private CameraSystem _camera = null;

    private void Start()
    {
        _camera = FindAnyObjectByType<CameraSystem>();
    }

    public void Move(InputAction.CallbackContext contex)
    {
        _camera.Move(contex.ReadValue<Vector2>());
    }
}
