using UnityEngine;
using Photon.Pun;

public enum PatrolType
{
    LeftRight,
    Square,
    Static
}

public class EnemyShooter : MonoBehaviourPun
{
    public GameObject projectilePrefab; // Prefab del proyectil
    public Transform firePoint; // Punto de origen del disparo
    public float fireRate = 1f; // Tiempo entre disparos (en segundos)
    public float detectionRange = 10f; // Rango de detección del jugador
    public float rotationSpeed = 2f; // Reducido de 5f a 2f para una rotación más suave

    private float nextFireTime = 0f; // Tiempo para el próximo disparo
    private GameObject targetPlayer; // Jugador objetivo

    [Header("Patrol Settings")]
    public PatrolType patrolType = PatrolType.LeftRight;
    public float patrolSpeed = 2f;
    public float patrolDistance = 5f;
    private Vector3 startPosition;
    private Vector3 currentTarget;
    private bool movingRight = true;
    
    [Header("Ground Check")]
    public float groundCheckDistance = 1f;
    public LayerMask groundLayer;
    public float obstacleCheckDistance = 1f;

    private void Start()
    {
        // Asegurarnos de que tenga una rotación inicial correcta
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        
        // Guardar la posición inicial para el patrullaje
        startPosition = transform.position;
        
        // Calcular el punto objetivo inicial para el patrullaje
        currentTarget = startPosition + transform.right * patrolDistance;

        // Asegurar que el PhotonView está configurado correctamente
        if (photonView == null)
        {
            Debug.LogError("PhotonView no encontrado en EnemyShooter");
            return;
        }

        // Solicitar la propiedad del objeto si no la tenemos
        if (!photonView.IsMine)
        {
            photonView.RequestOwnership();
        }
    }

    void Update()
    {
        // Remover la verificación de IsMine para permitir el movimiento local
        FindNearestPlayer();

        if (targetPlayer != null)
        {
            RotateTowardsPlayer();

            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
        else
        {
            //Patrol();
        }
    }

    // Método para encontrar al jugador más cercano
    void FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); // Buscar todos los jugadores
        float nearestDistance = Mathf.Infinity;
        GameObject nearestPlayer = null;

        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < nearestDistance && distance <= detectionRange)
            {
                nearestDistance = distance;
                nearestPlayer = player;
            }
        }

        targetPlayer = nearestPlayer; // Asignar al jugador más cercano como objetivo
    }

    // Método para girar hacia el jugador
    void RotateTowardsPlayer()
    {
        if (targetPlayer != null)
        {
            // Calcular la dirección hacia el jugador en el plano horizontal
            Vector3 targetPosition = targetPlayer.transform.position;
            targetPosition.y = transform.position.y; // Mantener la misma altura
            Vector3 direction = (targetPosition - transform.position).normalized;

            // Calcular la rotación solo en el eje Y
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            
            // Aplicar una rotación más suave
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime * 60f // Multiplicar por 60 para mejor control de la velocidad
            );

            // Actualizar el firePoint si existe
            if (firePoint != null)
            {
                firePoint.rotation = transform.rotation;
            }
        }
    }

    // Método para disparar
    void Shoot()
    {
        if (targetPlayer != null && projectilePrefab != null && firePoint != null)
        {
            Vector3 direction = (targetPlayer.transform.position - firePoint.position).normalized;
            
            // Asegurarse de que el proyectil se instancie en la red solo si somos el dueño
            if (photonView.IsMine)
            {
                GameObject projectile = PhotonNetwork.Instantiate(
                    projectilePrefab.name, 
                    firePoint.position, 
                    Quaternion.LookRotation(direction)
                );

                // Asegurarse de que el proyectil tenga el componente necesario
                Projectile projectileComponent = projectile.GetComponent<Projectile>();
                if (projectileComponent != null)
                {
                    projectileComponent.SetDirection(direction);
                }
            }
        }
    }

    void Patrol()
    {
        if (patrolType == PatrolType.Static)
            return;

        if (patrolType == PatrolType.LeftRight)
        {
            // Calcular la dirección de movimiento
            Vector3 movement = transform.right * (movingRight ? 1 : -1) * patrolSpeed * Time.deltaTime;
            
            if (CanMoveToPosition(transform.position + movement))
            {
                transform.position += movement;

                // Verificar si necesitamos cambiar de dirección
                float distanceFromStart = Vector3.Distance(transform.position, startPosition);
                if (distanceFromStart >= patrolDistance)
                {
                    movingRight = !movingRight;
                    // Girar el enemigo 180 grados
                    transform.Rotate(0, 180, 0);
                }
            }
            else
            {
                movingRight = !movingRight;
                // Girar el enemigo 180 grados
                transform.Rotate(0, 180, 0);
            }
        }
    }

    bool CanMoveToPosition(Vector3 targetPosition)
    {
        // Verificar si hay suelo
        RaycastHit hit;
        if (!Physics.Raycast(targetPosition + Vector3.up, Vector3.down, out hit, groundCheckDistance, groundLayer))
            return false;

        // Verificar obstáculos
        if (Physics.Raycast(transform.position, targetPosition - transform.position, obstacleCheckDistance))
            return false;

        return true;
    }

    void OnDrawGizmos()
    {
        // Dibujar área de detección
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Dibujar rango de patrulla
        if (patrolType == PatrolType.LeftRight)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(startPosition - Vector3.right * patrolDistance, 
                           startPosition + Vector3.right * patrolDistance);
        }

        // Dibujar raycast de detección de suelo
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position + Vector3.up, Vector3.down * groundCheckDistance);
    }
}