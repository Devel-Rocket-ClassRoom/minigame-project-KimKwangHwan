using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    public float MoveX { get; private set; }
    public float MoveY { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool JumpHeld { get; private set; }
    public bool DashPressed { get; private set; }
    public bool UseItemPressed { get; private set; }
    public bool InteractPressed { get; private set; }

    public void OnInteract(InputAction.CallbackContext c)
    {
        if (c.started) InteractPressed = true;
    }
    public void OnMove(InputAction.CallbackContext c)
    {
        var v = c.ReadValue<Vector2>();
        MoveX = v.x;
        MoveY = v.y;
    }
    public void OnJump(InputAction.CallbackContext c)
    {
        if (c.started) JumpPressed = true;
        JumpHeld = c.performed;
    }
    public void OnDash(InputAction.CallbackContext c)
    {
        if (c.started) DashPressed = true;
    }
    public void OnUseItem(InputAction.CallbackContext c)
    {
        if (c.started) UseItemPressed = true; 
    }
    void LateUpdate()
    {
        JumpPressed = false;
        DashPressed = false;
        UseItemPressed = false;
        InteractPressed = false;
    }
}
