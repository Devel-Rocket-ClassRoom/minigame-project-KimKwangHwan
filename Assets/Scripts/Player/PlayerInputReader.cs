using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    public float MoveX { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool JumpHeld { get; private set; }
    public bool AttackPressed { get; private set; }

    private void Awake()
    {

    }

    public void OnMove(InputAction.CallbackContext c) => MoveX = c.ReadValue<Vector2>().x;
    public void OnJump(InputAction.CallbackContext c)
    {
        if (c.started) JumpPressed = true;
        JumpHeld = c.performed;
    }

    void LateUpdate()
    {
        JumpPressed = false;
        AttackPressed = false;
    }
}
