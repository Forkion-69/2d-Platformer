using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private string currentEnemyType;
    public EnemyStats enemyStats;


    [SerializeField]private Collider2D _bodyCol;
    [SerializeField]private Collider2D _feetCol;

    private Rigidbody2D rb;

    private int _enemyHealth;
    
    //move vars
    //common
    
    private float _movementSpeed;
    private bool _isfacingRight;

    //Springy

    private float jumpInitialVelocity;

    //collision vars
    private RaycastHit2D _turnRayCastL;
    private RaycastHit2D _turnRayCastR;
    private RaycastHit2D _groundRayCast;
    private bool _hitWall;
    private bool _isGrounded;

    private void Awake()
    {
        _isfacingRight = true;

        if(currentEnemyType == "Goomba")
        {
            _movementSpeed = enemyStats.goombaSpeed;
            _enemyHealth = enemyStats.goombaHealth;
        }
        if(currentEnemyType == "Springy")
        {
            jumpInitialVelocity = MathF.Abs(Physics2D.gravity.y) * enemyStats.timeTillJumpApex;
            _movementSpeed = enemyStats.springySpeed;
            _enemyHealth = enemyStats.springyHealth;
        }
        
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        StartCoroutine("JumpSpringy");
    }

    private void FixedUpdate()
    {
        Move();
        WallHit();
        IsGrounded();
    }

    //movement

    private void Move()
    {
        Vector2 TargetVelocity = Vector2.zero;


        if (_isfacingRight)
            TargetVelocity = new Vector2(_movementSpeed * Time.fixedDeltaTime, rb.linearVelocityY);
        else if(!_isfacingRight)
            TargetVelocity = new Vector2(-_movementSpeed * Time.fixedDeltaTime, rb.linearVelocityY);

        rb.linearVelocity = TargetVelocity;
    }

    //collision Check

    private void WallHit()
    {
        Vector2 boxCastOriginL = new Vector2(_bodyCol.bounds.min.x, _bodyCol.bounds.center.y);
        Vector2 boxCastOriginR = new Vector2(_bodyCol.bounds.max.x, _bodyCol.bounds.center.y);
        Vector2 boxCastSize = new Vector2(_feetCol.bounds.size.x,enemyStats.turnDetectionRayLength);

        _turnRayCastL = Physics2D.BoxCast(boxCastOriginL,boxCastSize,90f,Vector2.left,enemyStats.turnDetectionRayLength, enemyStats.turnableLayerMask);
        _turnRayCastR = Physics2D.BoxCast(boxCastOriginR,boxCastSize,90f,Vector2.right,enemyStats.turnDetectionRayLength, enemyStats.turnableLayerMask);
        
        if(enemyStats.ShowDebug){
        Debug.DrawRay(new Vector3(boxCastOriginL.x,boxCastOriginL.y,0),Vector3.left * enemyStats.turnDetectionRayLength * 4, Color.red,0,false);
        Debug.DrawRay(new Vector3(boxCastOriginR.x,boxCastOriginR.y,0),Vector3.right * enemyStats.turnDetectionRayLength * 4, Color.red,0,false);
        }
        if(_turnRayCastL.collider != null)
        {
            Debug.Log("HIT WALL ON LEFT");
            _hitWall = true;
            _isfacingRight = true;
        }
        else{_hitWall = false;}
        if(_turnRayCastR.collider != null)

        {   Debug.Log("HIT WALL ON RIGHT");
            _hitWall = true;
            _isfacingRight = false;
        }
        else{_hitWall = false;}
    }

    private IEnumerator JumpSpringy()
    {
        while (currentEnemyType == "Springy")
        {
            yield return new WaitForSeconds(enemyStats.jumpCycle);
            if(_isGrounded)
            {
                Debug.Log("SHOULD JUMP");
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpInitialVelocity);
            }
                
        }
    }

    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(_feetCol.bounds.center.x,_feetCol.bounds.min.y);
        Vector2 boxCastSize = new Vector2(_feetCol.bounds.size.x,enemyStats.groundDetectionRayLength);

        _groundRayCast = Physics2D.BoxCast(boxCastOrigin,boxCastSize,-90f,Vector2.down,enemyStats.groundDetectionRayLength);

        if (_groundRayCast.collider != null)
        {
            _isGrounded = true;
        }else {_isGrounded = false;}
    }
    


}
