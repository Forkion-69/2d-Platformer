using System;
using UnityEngine;
using UnityEngine.Rendering;


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
    private RaycastHit2D _headRayHit;
    private bool _isGrounded;
    private bool _headBumped;

    //jump vars
    public float VerticalVelocity{get; private set;}
    private bool _isJumping;
    private bool _isFastFalling;
    private bool _isFaliing;
    private float _fastFallTime;
    private float _fastFallReleaseSpeed;
    private int _numberOfJumpUsed;

    //Apex vars
    private float _apexPoint;
    private float _timePastApexThreshold;
    private bool _isPastApexThreshold;

    //buffer time
    private float _jumpBufferTime;
    private bool _jumpReleasedDuringBuffer;

    //Coyoute timer
    private float _coyoteTimer;


    void Awake()
    {
        _isFacingRight = true;

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CountTimer();
        JumpChecks();
    }

    void FixedUpdate()
    {
        CollisionChecks();
        Jump();

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
            transform.Rotate(0f,180f,0f);
        }
        else
        {
            _isFacingRight = false;
            transform.Rotate(0f,-180f,0f);
        }
    }
    #endregion

    #region Jumping

    private void JumpChecks()
    {
        //WHEN JUMP IS PRESSED
        if (InputManagerScript.jumpWasPressed)
        {
            _jumpBufferTime = MoveStats.jumpBufferTime;
            _jumpReleasedDuringBuffer = false;
        }
        //WHEN JUMP IS RELEASED
        if (InputManagerScript.jumpWasReleased)
        {
            if(_jumpBufferTime > 0f)
            {
                _jumpReleasedDuringBuffer = true;
            }
            if(_isJumping && VerticalVelocity > 0f)
            {
                if (_isPastApexThreshold)
                {
                    _isPastApexThreshold = false;
                    _isFastFalling =  true;
                    _fastFallTime = MoveStats.timeForUpwardsCancel;
                    VerticalVelocity = 0f;
                }
                else
                {
                    _isFastFalling = true;
                    _fastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }
        //INITIATE JUMP WITH JUMP BUFFERING AND COYOTE TIME
        if (_jumpBufferTime > 0f && !_isJumping && (_isGrounded || _coyoteTimer > 0f))
        {
            InitiateJump(1);
            if (_jumpReleasedDuringBuffer)
            {
                _isFastFalling = true;
                _fastFallReleaseSpeed = VerticalVelocity;
            }
        }

        //DOUBLE JUMP
        else if(_jumpBufferTime > 0f && _isJumping && _numberOfJumpUsed < MoveStats.numberOfJumps)
        {
            _isFastFalling = false;
            InitiateJump(1);
        }
        //AIR JUMP AFTER COYOTE HAS ELASPED

        else if (_jumpBufferTime > 0f && _isFaliing && _numberOfJumpUsed < MoveStats.numberOfJumps - 1)
        {
            InitiateJump(2);
            _isFastFalling = false;
        }

        //LANDED
        if((_isJumping ||_isFaliing) && _isGrounded && VerticalVelocity <= 0f)
        {
            _isJumping = false;
            _isFastFalling = false;
            _isFaliing = false;
            _fastFallTime = 0;
            _numberOfJumpUsed = 0;
            _isPastApexThreshold = false;

            VerticalVelocity = Physics2D.gravity.y;
        }
    }

    private void InitiateJump(int numberOfJumpUsed)
    {
        if(_isJumping == false)
        {
            _isJumping = true;
        }

        _jumpBufferTime = 0f;
        _numberOfJumpUsed += numberOfJumpUsed;
        VerticalVelocity = MoveStats.jumpInitialVelocity;

    }

    private void Jump()
    {
        //APPLY GRAVITY WHILE JUMPING
        if (_isJumping)
        {
            //CHECK FOR HEAD BUMP
            if (_headBumped)
            {
                _isFastFalling = true;
            }
        }
        //GRAVITY ON ASCENDING
        if(VerticalVelocity >= 0)
        {
            //APEX CONTROL
            _apexPoint = Mathf.InverseLerp(MoveStats.jumpInitialVelocity,0f,VerticalVelocity);
            if(_apexPoint > MoveStats.apexThreshold)
            {
                if(!_isPastApexThreshold)
                {
                    _isPastApexThreshold = true;
                    _timePastApexThreshold = 0;
                }
                else if (_isPastApexThreshold)
                {
                    _timePastApexThreshold += Time.fixedDeltaTime;
                    if(_timePastApexThreshold < MoveStats.apexHangTime)
                    {
                        VerticalVelocity = 0f;
                    }
                    else
                    {
                        VerticalVelocity = -0.01f;
                    }
                }
            }
            //GRAVITY ON ASCENDIND BUT NOT PAST APEX THRESHOLD
            else
            {
                VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime;
                if(_isPastApexThreshold)
                    _isPastApexThreshold = false;
            }
        }
        //GRAVITY ON DESCENDING
        else if (!_isFastFalling)
        {
            VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime;
        }
        else if(VerticalVelocity < 0)
        {
            if (!_isFaliing)
            {
                _isFaliing = true;
            }
        }
        //JUMP CUT

        if (_isFastFalling)
        {
            if(_fastFallTime > MoveStats.timeForUpwardsCancel)
            {
                VerticalVelocity += MoveStats.Gravity * MoveStats.gravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (_fastFallTime < MoveStats.timeForUpwardsCancel)
            {
                VerticalVelocity = Mathf.Lerp(_fastFallReleaseSpeed,0,(_fastFallTime/MoveStats.timeForUpwardsCancel));
            }

            _fastFallTime += Time.fixedDeltaTime;
        }
        // NORMAL GRAVITY WHILE FALLING
        if(!_isGrounded && !_isJumping)
        {
            if (!_isFaliing)
            {
                _isFaliing = true;
            }

            VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime;
        }
        //CLAMP FALL SPPED
        VerticalVelocity = Mathf.Clamp(VerticalVelocity, -MoveStats.maxFallSpeed,MoveStats.maxPositiveVelocity);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, VerticalVelocity);
    }

    #endregion

    #region Collision Checks

    private void CountTimer()
    {
        _jumpBufferTime -= Time.deltaTime;

        if (!_isGrounded)
          _coyoteTimer -= Time.deltaTime;  
        else{_coyoteTimer = MoveStats.jumpCoyoteTime;}
        
    }

    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(_feetCol.bounds.center.x, _feetCol.bounds.min.y);
        Vector2 boxCastSize = new Vector2(_feetCol.bounds.size.x,MoveStats.groundDetectionRayLength);

        _groundRayHit = Physics2D.BoxCast(boxCastOrigin,boxCastSize,0f,Vector2.down,MoveStats.groundDetectionRayLength, MoveStats.groundCheckLayer);

        if(_groundRayHit.collider != null)
        {
            _isGrounded = true;
        }
        else{_isGrounded = false;}

    }

    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(_feetCol.bounds.center.x, _bodyCol.bounds.max.y);
        Vector2 boxCastSize = new Vector2(_feetCol.bounds.size.x * MoveStats.headWidth, MoveStats.headDetettectionRayLength);

        _headRayHit = Physics2D.BoxCast(boxCastOrigin,boxCastSize,0f,Vector2.up,MoveStats.headDetettectionRayLength, MoveStats.groundCheckLayer);

        if(_headRayHit.collider != null)
        {
            _headBumped = true;
        }
        else{_headBumped = false;}
    }

    private void CollisionChecks()
    {
        IsGrounded();
        BumpedHead();
    }

    #endregion
}
