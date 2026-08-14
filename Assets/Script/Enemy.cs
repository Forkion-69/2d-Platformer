using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

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

//Shooter
    private bool _isShoooting = false;
    private int _shootingDirection;
    private GameObject _bullet;

//collision vars
    private RaycastHit2D _turnRayCastL;
    private RaycastHit2D _turnRayCastR;
    private RaycastHit2D _groundRayCast;
    private RaycastHit2D _sightRaycastR;
    private bool _isGrounded;

    private void Awake()
    {
        _isfacingRight = true;

        if(currentEnemyType == "Goomba")
        {
            _movementSpeed = enemyStats.goombaSpeed;
            _enemyHealth = enemyStats.goombaHealth;
        }
        else if(currentEnemyType == "Springy")
        {
            jumpInitialVelocity = MathF.Abs(Physics2D.gravity.y) * enemyStats.timeTillJumpApex;
            _movementSpeed = enemyStats.springySpeed;
            _enemyHealth = enemyStats.springyHealth;
        }
        else if(currentEnemyType == "Shooter")
        {
            _bullet = enemyStats.shooterBullet;
            _movementSpeed = enemyStats.shooterSpeed;
            _enemyHealth = enemyStats.shooterHealth;
            _shootingDirection = 1;
        }
        
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {   
        StartCoroutine(nameof(JumpSpringy));
    }

    private void FixedUpdate()
    {
        Move();
        WallHit();
        if(currentEnemyType == "Springy")
            IsGrounded();
        if(currentEnemyType == "Shooter")
            LineOfSightCast();
        
    }

//movement

    private void Move()
    {

        Vector2 TargetVelocity = Vector2.zero;


        if (_isfacingRight && !_isShoooting){
            TargetVelocity = new Vector2(_movementSpeed * Time.fixedDeltaTime, rb.linearVelocityY);
        }
        else if(!_isfacingRight && !_isShoooting)
        {
            TargetVelocity = new Vector2(-_movementSpeed * Time.fixedDeltaTime, rb.linearVelocityY);
        }
        else if(_isShoooting)
        {
            TargetVelocity = new Vector2(0,rb.linearVelocity.y);
        }
        
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
        Debug.DrawRay(new Vector3(boxCastOriginL.x,boxCastOriginL.y,0),Vector3.left * enemyStats.turnDetectionRayLength * 2, Color.crimson,0,false);
        Debug.DrawRay(new Vector3(boxCastOriginR.x,boxCastOriginR.y,0),Vector3.right * enemyStats.turnDetectionRayLength * 2, Color.crimson,0,false);
        }

        if(_turnRayCastL.collider != null)
        {
            _isfacingRight = true;
            transform.Rotate(0f,180f,0f);
            _shootingDirection = 1;
        } 
        if(_turnRayCastR.collider != null)
        { 
            _isfacingRight = false;
            transform.Rotate(0f,-180f,0f);
            _shootingDirection = -1;
        }
    }
    
//Springy

    private IEnumerator JumpSpringy()
    {
        while (currentEnemyType == "Springy")
        {
            yield return new WaitForSeconds(enemyStats.jumpCycle);
            if(_isGrounded)
            {
                //Debug.Log("SHOULD JUMP");
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
    
//Shooter
    private void LineOfSightCast()
    {   

        Vector2 RayCastOriginR = new Vector2(_bodyCol.bounds.max.x, _bodyCol.bounds.center.y);
        _sightRaycastR = Physics2D.Raycast(RayCastOriginR,new Vector2(_shootingDirection,0),enemyStats.shooterRayLength,enemyStats.playerLayer);

        if(enemyStats.ShowDebug)
            Debug.DrawRay(new Vector3(_bodyCol.bounds.max.x, _bodyCol.bounds.center.y,0),new Vector3(_shootingDirection,0,0) * enemyStats.shooterRayLength, Color.red,0,false);

        if(_sightRaycastR.collider != null)
        {
            _isShoooting = true;
            // Debug.Log("GET ON THE GROUND, NOW " + _isShoooting);
        }else{_isShoooting = false;}
    }
    private void Shoot()
    {
        if (_isShoooting && _isfacingRight)
        {
            
        }
        else{return;}
    }

}
