using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private InputSystemActions inputSystem;
    private Vector2 direction;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 capsuleSize = new Vector2(0.5f,1f);
    

    void Awake()
    {
        inputSystem = new InputSystemActions();
        rb = GetComponent<Rigidbody2D>();
        inputSystem.Player.Enable();
        inputSystem.Player.Jump.performed += Jump;
    }
    void FixedUpdate()
    {
        Move();
    }
    
    public void Jump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump");
        if (IsGrounded() && context.performed)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void Move()
    {
        direction = inputSystem.Player.Move.ReadValue<Vector2>();
        rb.linearVelocity = new Vector2(direction.x * movementSpeed, rb.linearVelocityY);
    }

    private bool IsGrounded()
    {
       return Physics2D.OverlapCapsule(groundCheck.position,capsuleSize,CapsuleDirection2D.Horizontal,90,groundLayer);
    }
}
