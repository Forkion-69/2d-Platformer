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

    [Header("Ground/Collision Checks")]
    public LayerMask groundCheckLayer;
    public float groundDetectionRayLength = 0.02f;
    public bool showDebugIsGroundedBox;

}
