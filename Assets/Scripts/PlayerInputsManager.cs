using UnityEngine;
using System;
using UnityEngine.InputSystem;
public class PlayerInputsManager : MonoBehaviour
{
    public event Action<Vector2> moveInput;
    public event Action<bool> jumpInput; 
    public void ReadMovement(InputAction.CallbackContext context)
    {
        Vector2 move = context.ReadValue<Vector2>();
        moveInput?.Invoke(move);
    }
    public void ReadJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpInput?.Invoke(true);
        }
    }
}
