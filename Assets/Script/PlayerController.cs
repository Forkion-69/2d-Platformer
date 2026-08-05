using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private InputSystemActions inputSystem;
    private Vector2 direction;
    private bool canDash = true;
    private bool isDashing;
    [SerializeField] private float movementSpeed = 0.1f;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float dashForce;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 capsuleSize = new Vector2(0.5f,1f);

    void Awake()
    {
        inputSystem = new InputSystemActions();
        rb = GetComponent<Rigidbody2D>();
        inputSystem.Player.Enable();
        inputSystem.Player.Jump.performed += Jump;
        inputSystem.Player.Sprint.performed += Sprint;
        inputSystem.Player.Sprint.canceled += Sprint;
    }
    void OnDisable()
    {
        inputSystem.Player.Disable();
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
        float currentStateSpeed = 0.1f;
        if(isDashing)
            currentStateSpeed = dashSpeed;
        else
            currentStateSpeed = movementSpeed;

        rb.position += new Vector2(direction.x * currentStateSpeed,0);
        rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocityX, -8, 8), Mathf.Clamp(rb.linearVelocityY, -10, 8));
    }
    public void Sprint(InputAction.CallbackContext context)
    {   
        if(canDash && context.performed){
            Debug.Log(context);
            rb.AddForce(direction * dashForce, ForceMode2D.Impulse);
            StartCoroutine("DashTimer");  
        }
        if (context.performed)
            isDashing = true;
        else if (context.canceled)
            isDashing = false;
    }

    private IEnumerator DashTimer()
    {
        canDash = false;
        yield return new WaitForSeconds(0.8f);
        canDash = true;
    }
    private bool IsGrounded()
    {
       return Physics2D.OverlapCapsule(groundCheck.position,capsuleSize,CapsuleDirection2D.Horizontal,90,groundLayer);
    }
}
