using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : SingletonND<InputManager>
{
    private Controls controls;

        private void Awake()
    {
        controls = new Controls();

        controls.Player.Jump.started += ctx => Jump(ctx);
        controls.Player.Jump.canceled += ctx => StopJump(ctx);

        controls.Player.Shoot.started += ctx => shoot = true;
        controls.Player.Shoot.canceled += ctx => shoot = false;


        DontDestroyOnLoad(Instance.gameObject);
    }



    public delegate void JumpEvent();
    public event JumpEvent OnJump; 
    private void Jump(InputAction.CallbackContext context)
    {
        if (OnJump != null) OnJump();
    }
    public delegate void StopJumpEvent();
    public event StopJumpEvent OnJumpCanceled;
    private void StopJump(InputAction.CallbackContext context)
    {
        if (OnJumpCanceled != null) OnJumpCanceled();
    }



    public Vector2 GetPlayerMovement()
    {
        return controls.Player.Move.ReadValue<Vector2>();
    }

    public Vector2 GetMousePosition()
    {
        return controls.Player.Mouse.ReadValue<Vector2>();
    }

    public Vector2 GetLook()
    {
        return controls.Player.Look.ReadValue<Vector2>();
    }

    bool shoot;
    public bool GetShoot()
    {
        return shoot;
    }

    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
}
