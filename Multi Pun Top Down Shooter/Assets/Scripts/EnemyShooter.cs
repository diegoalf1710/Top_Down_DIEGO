using UnityEngine;
using Photon.Pun;

/// <summary>
/// Define los tipos de patrulla que puede realizar el enemigo
/// </summary>
public enum PatrolType
{
    LeftRight,   // Patrulla de izquierda a derecha
    Square,      // Patrulla en cuadrado
    Static       // Sin movimiento
}

/// <summary>
/// Controla el comportamiento de los enemigos que disparan en el juego.
/// Incluye detección de jugadores, sistema de patrulla y disparo automático.
/// </summary>
public class EnemyShooter : MonoBehaviourPun
{
    [Header("Configuración de Disparo")]
    /// <summary>
    /// Prefab del proyectil que dispara el enemigo
    /// </summary>
    public GameObject projectilePrefab;

    /// <summary>
    /// Punto desde donde se originan los disparos
    /// </summary>
    public Transform firePoint;

    /// <summary>
    /// Tiempo entre disparos en segundos
    /// </summary>
    public float fireRate = 1f;

    /// <summary>
    /// Distancia máxima a la que el enemigo puede detectar jugadores
    /// </summary>
    public float detectionRange = 10f;

    /// <summary>
    /// Velocidad de rotación del enemigo
    /// </summary>
    public float rotationSpeed = 2f;

    [Header("Configuración de Patrulla")]
    /// <summary>
    /// Tipo de patrulla que realizará el enemigo
    /// </summary>
    public PatrolType patrolType = PatrolType.LeftRight;

    /// <summary>
    /// Velocidad de movimiento durante la patrulla
    /// </summary>
    public float patrolSpeed = 2f;

    /// <summary>
    /// Distancia máxima que recorre el enemigo durante la patrulla
    /// </summary>
    public float patrolDistance = 5f;

    [Header("Verificación de Suelo")]
    /// <summary>
    /// Distancia para verificar si hay suelo debajo
    /// </summary>
    public float groundCheckDistance = 1f;

    /// <summary>
    /// Capa que define el suelo para las comprobaciones
    /// </summary>
    public LayerMask groundLayer;

    /// <summary>
    /// Distancia para verificar obstáculos al frente
    /// </summary>
    public float obstacleCheckDistance = 1f;

    [Header("Configuración de Salud")]
    /// <summary>
    /// Salud máxima del enemigo
    /// </summary>
    public float maxHealth = 100f;

    // Variables privadas
    private float nextFireTime = 0f;
    private GameObject targetPlayer;
    private Vector3 startPosition;
    private Vector3 currentTarget;
    private bool movingRight = true;
    private float currentHealth;

    private void Start()
    {
        // Inicializar la salud
        currentHealth = maxHealth;

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
        FindNearestPlayer();

        if (targetPlayer != null)
        {
            RotateTowardsPlayer();

            // Verificar si el jugador está dentro del rango y si podemos disparar
            float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.transform.position);
            if (distanceToPlayer <= detectionRange && Time.time >= nextFireTime)
            {
                // Verificar si estamos mirando hacia el jugador
                Vector3 directionToPlayer = (targetPlayer.transform.position - transform.position).normalized;
                float dot = Vector3.Dot(transform.forward, directionToPlayer);
                
                // Si estamos mirando aproximadamente hacia el jugador (dot > 0.8 significa ángulo < ~37 grados)
                if (dot > 0.8f)
                {
                    Shoot();
                    nextFireTime = Time.time + fireRate;
                }
            }
        }
        else
        {
            Patrol();
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
        if (targetPlayer == null || projectilePrefab == null || firePoint == null) return;

        // Calcular dirección hacia el jugador
        Vector3 direction = (targetPlayer.transform.position - firePoint.position).normalized;
        
        // Crear el proyectil usando PhotonNetwork.Instantiate
        try
        {
            GameObject projectile = PhotonNetwork.Instantiate(
                projectilePrefab.name, 
                firePoint.position, 
                Quaternion.LookRotation(direction)
            );

            // Asegurarse de que el proyectil tenga el componente necesario y configurarlo
            Projectile projectileComponent = projectile.GetComponent<Projectile>();
            if (projectileComponent != null)
            {
                projectileComponent.SetDirection(direction);
                Debug.Log("Proyectil disparado hacia: " + direction);
            }
            else
            {
                Debug.LogWarning("El proyectil no tiene el componente Projectile");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al instanciar el proyectil: " + e.Message);
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
                    transform.Rotate(0, 180*Time.deltaTime, 0);
                }
            }
            else
            {
                movingRight = !movingRight;
                // Girar el enemigo 180 grados
                transform.Rotate(0, 180*Time.deltaTime, 0);
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

    [PunRPC]
    public void TakeDamage(float damage)
    {
        // Removemos la verificación IsMine para que todos los clientes actualicen la salud
        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage. Current health: {currentHealth}");

        if (currentHealth <= 0 && photonView.IsMine)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy destroyed!");
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;

        if (other.CompareTag("Bullet"))
        {
            // Llamar al RPC de daño
            photonView.RPC("TakeDamage", RpcTarget.All, 10f);
            
            // Destruir la bala
            if (other.gameObject.GetComponent<PhotonView>())
            {
                PhotonNetwork.Destroy(other.gameObject);
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine) return;

        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("Bullet hit enemy!");
            // Llamar al RPC de daño
            photonView.RPC("TakeDamage", RpcTarget.All, 10f);
            
            // Destruir la bala
            if (collision.gameObject.GetComponent<PhotonView>())
            {
                PhotonNetwork.Destroy(collision.gameObject);
            }
            else
            {
                Destroy(collision.gameObject);
            }
        }
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