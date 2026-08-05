using UnityEngine;


public class PlayerScript : MonoBehaviour
{
    
    [Header("References")]
    public PlayerStatsScript MoveStats;
    
    
    [SerializeField]private Collider2D _feetCol;
    [SerializeField]private Collider2D _bodyCol;

    private Rigidbody2D rb;

    // move Vars

    private Vector2 _playerVelocity;
    private bool _isFacingRight;

    //collison/ground checks
    private RaycastHit2D _groundRayHit;
    private bool _isGrounded;

    void Awake()
    {
        _isFacingRight = true;

        rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        CollisionChecks();

        if (_isGrounded)
        {
            Move(MoveStats.groundAcceleration, MoveStats.groundDeceleration,InputManagerScript.movement);
        }
        else
        {
            Move(MoveStats.airAcceleration, MoveStats.airDeceleration,InputManagerScript.movement);
        }
    }
    #region Movement

    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        if(moveInput != Vector2.zero)
        {
            //check if he needs to turn
            TurnCheck(moveInput);

            Vector2 TargetVelocity = Vector2.zero;

            if(InputManagerScript.runIsHeld)
            {
                TargetVelocity = new Vector2(moveInput.x,0f) * MoveStats.maxRunSpeed;
            }
            else{TargetVelocity = new Vector2(moveInput.x,0f) * MoveStats.maxWalkSpeed;}

            _playerVelocity = Vector2.Lerp(_playerVelocity,TargetVelocity, acceleration * Time.deltaTime);
            rb.linearVelocity = new Vector2(_playerVelocity.x,rb.linearVelocity.y);
        }
        else if(moveInput == Vector2.zero)
        {
            _playerVelocity = Vector2.Lerp(_playerVelocity,Vector2.zero,deceleration * Time.deltaTime);
            rb.linearVelocity = new Vector2(_playerVelocity.x, rb.linearVelocity.y);
        }
    }

    private void TurnCheck(Vector2 moveInput)
    {
        if(_isFacingRight && moveInput.x < 0)
            Turn(false);
        else if(!_isFacingRight && moveInput.x > 0)
            Turn(true);
    }

    private void Turn(bool turnRight)
    {
        if(turnRight)
        {
            _isFacingRight = true;
            transform.Rotate(0f,100f,0f);
        }
        else
        {
            _isFacingRight = false;
            transform.Rotate(0f,-100f,0f);
        }
    }
    #endregion

    #region Collision Checks

    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(_feetCol.bounds.center.x, _feetCol.bounds.min.y);
        Vector2 boxCastSize = new Vector2(_feetCol.bounds.size.x,MoveStats.groundDetectionRayLength);

        _groundRayHit = Physics2D.BoxCast(boxCastOrigin,boxCastSize,0f,Vector2.down,MoveStats.groundDetectionRayLength, MoveStats.groundCheckLayer);

        #region DONT TOUCH DEBUG
        if(_groundRayHit.collider != null)
            _isGrounded = true;
        else
            _isGrounded = false;

        if (MoveStats.showDebugIsGroundedBox)
        {
            Color rayColor;
            if(_isGrounded)
                rayColor = Color.green;
            else{rayColor = Color.red;}

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x/2,boxCastOrigin.y),Vector2.down * MoveStats.groundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x/2,boxCastOrigin.y),Vector2.down * MoveStats.groundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x/2,boxCastOrigin.y - MoveStats.groundDetectionRayLength),Vector2.right * boxCastSize.x, rayColor);
        }
        #endregion
    }
    private void CollisionChecks()
    {
        IsGrounded();
    }

    #endregion


}
