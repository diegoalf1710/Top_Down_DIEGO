using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent agent;
    
    [Header("Enemy Settings")]
    public float enemySpeed = 3f;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public bool isAggressive = true;

    [Header("Movement Smoothing")]
    public float accelerationTime = 0.5f;
    public float turnSpeed = 120f;
    public float smoothRotationSpeed = 8f;

    private Transform playerTarget;
    private bool playerInRange;
    private float currentSpeed;
    private Vector3 smoothVelocity;
}
