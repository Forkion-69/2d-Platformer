using UnityEngine;
using UnityEngine.InputSystem;

public class InputManagerScript : MonoBehaviour
{
    public static PlayerInput playerInput;
    
    public static Vector2 movement;
    public static bool jumpWasPressed;
    public static bool jumpWasHeld;
    public static bool jumpWasReleased;
    public static bool runIsHeld;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _runAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        _moveAction = playerInput.actions["Move"];
        _jumpAction = playerInput.actions["Jump"];
        _runAction = playerInput.actions["Sprint"];
    }

    private void Update()
    {
        movement = _moveAction.ReadValue<Vector2>();

        jumpWasPressed = _jumpAction.WasPressedThisFrame();
        jumpWasHeld = _jumpAction.IsPressed();
        jumpWasReleased =_jumpAction.WasReleasedThisFrame();

        runIsHeld = _runAction.IsPressed();
    }

}
