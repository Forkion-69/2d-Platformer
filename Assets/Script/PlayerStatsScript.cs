
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsScript", menuName = "Scriptable Objects/PlayerStatsScript")]
public class PlayerStatsScript : ScriptableObject
{
    [Header("Walk")]
    [Range(0f,100f)]public float maxWalkSpeed = 12.5f;
    [Range(0.25f,50f)] public float groundAcceleration = 5f;
    [Range(0.25f,50f)] public float groundDeceleration = 20f;
    [Range(0.25f,50f)] public float airAcceleration = 5f;
    [Range(0.25f,50f)] public float airDeceleration = 5f;

    [Header("Run")]
    [Range(0f,100f)] public float maxRunSpeed = 20f;

    [Header("Jump")]
    [Range(4f,100f)] public float jumpHeight = 6.5f;
    [Range(0.125f,10f)] public float jumpHeightCompensationFactor = 1.054f;
    [Range(0.01f,5f)] public float gravityOnReleaseMultiplier = 2f;
    public float timeTillJumpApex = 0.35f;
    [Range(5f,50f)] public float maxFallSpeed = 26f;
    [Range(20f,100f)] public float maxPositiveVelocity = 50f;
    [Range(1,5)]public int numberOfJumps = 2;

    [Header("Jump Cut")]
    [Range(0.01f,2f)] public float timeForUpwardsCancel = 0.027f;

    [Header("Jump Apex")]
    [Range(0.5f,1f)]public float apexThreshold = 0.97f;
    [Range(0.01f, 1f)]public float apexHangTime = 0.075f;

    [Header("Jump Buffer")]
    [Range(0,1f)] public float jumpBufferTime = 0.125f;

    [Header("Coyote Time")]
    [Range (0,1f)] public float jumpCoyoteTime = 0.1f;

    [Header ("Debug")]

    public bool showDebugIsGroundedBox;

    [Header ("Jump Visualization Tool")]
    public bool showWalkJumpArc = false;
    public bool showRunJumpArc = false;
    public bool stopOnCollision = true;
    public bool drawRight = true;
    [Range(5,100)] public int ArcResolution = 20;
    [Range(0,500)] public int VisualizationSteps = 90;

    [Header("Ground/Collision Checks")]
    public LayerMask groundCheckLayer;
    public float groundDetectionRayLength = 0.02f;
    public float headDetettectionRayLength = 0.02f;
    public float headWidth = 0.75f;

    [Header ("Gravity Calculations")]

    public float Gravity {get; private set;}

    public float jumpInitialVelocity {get; private set;}

    public float adjustedHeight {get; private set;}

    void OnValidate()
    {
        CalculateValues();
    }

    void OnEnable()
    {
        CalculateValues();
    }

    private void CalculateValues()
    {
        adjustedHeight = jumpHeight * jumpHeightCompensationFactor;
        Gravity = -(2f * adjustedHeight)/Mathf.Pow(timeTillJumpApex,2f);
        jumpInitialVelocity = MathF.Abs(Gravity) * timeTillJumpApex;
    }

}
