using System;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemyStats", menuName = "Scriptable Objects/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    [Header("GLOBAL")]

    [Range(0.01f, 5f)] public float turnDetectionRayLength = 0.1f;
    public LayerMask turnableLayerMask;
    public bool ShowDebug;

    [Header ("Goombas Stats")]
    [Range(1f,50f)] public float goombaSpeed = 5f;
    [Range(1f,150f)] public int goombaHealth = 5;
    [Range(1f,10f)] public float goombaDamage = 2f;

    
    [Header("Springys Stats")]
    public LayerMask groundLayerMask;
    [Range(1f,50f)] public float springySpeed = 25f;
    [Range(1f,150f)] public int springyHealth = 10;
    [Range(1f,10f)] public float springyDamage = 4f;
    [Range(0.01f,25f)]public float timeTillJumpApex = 0.35f;
    [Range(0.01f, 20f)] public float groundDetectionRayLength = 0.05f;
    [Range(0.5f, 25f)] public float jumpCycle = 4f;
    

    [Header ("Shooter Stats")]
    [Range(1f,50f)] public float shooterSpeed;
    [Range(1f,150f)] public int shooterHealth;
    [Range(1f,150f)] public float shooterWalkDistance;
    [Range(1f,10f)] public float shooterDamage;
    [Range(0.5f, 20f)] public float shooterRayLength;
}
