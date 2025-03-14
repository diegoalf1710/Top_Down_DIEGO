using UnityEngine;
using Photon.Pun;

/// <summary>
/// Controla el comportamiento de los enemigos que disparan en el juego.
/// Se enfoca en la detección y disparo hacia jugadores cercanos.
/// </summary>
public class EnemyShooter : MonoBehaviourPun
{
    [Header("Configuración de Disparo")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public float detectionRange = 10f;
    public float rotationSpeed = 2f;

    [Header("Configuración de Salud")]
    public float maxHealth = 100f;

    private float nextFireTime = 0f;
    private GameObject targetPlayer;
    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        if (photonView == null)
        {
            Debug.LogError("PhotonView no encontrado en EnemyShooter");
            return;
        }

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

            float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.transform.position);
            if (distanceToPlayer <= detectionRange && Time.time >= nextFireTime)
            {
                Vector3 directionToPlayer = (targetPlayer.transform.position - transform.position).normalized;
                float dot = Vector3.Dot(transform.forward, directionToPlayer);
                
                if (dot > 0.8f)
                {
                    Shoot();
                    nextFireTime = Time.time + fireRate;
                }
            }
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
    }
}