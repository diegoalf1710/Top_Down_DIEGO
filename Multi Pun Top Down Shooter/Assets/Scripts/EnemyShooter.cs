using UnityEngine;
using Photon.Pun;
using System.Collections;

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

    [Header("Configuración de Audio")]
    public AudioSource shootSound;

    [Header("Configuración de VFX")]
    public ParticleSystem deathParticleSystem;

    private float nextFireTime = 0f;
    private GameObject targetPlayer;

    private void Start()
    {
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

    void FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
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

        targetPlayer = nearestPlayer;
    }

    void RotateTowardsPlayer()
    {
        if (targetPlayer != null)
        {
            Vector3 targetPosition = targetPlayer.transform.position;
            targetPosition.y = transform.position.y;
            Vector3 direction = (targetPosition - transform.position).normalized;
            
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime * 60f
            );

            if (firePoint != null)
            {
                firePoint.rotation = transform.rotation;
            }
        }
    }

    void Shoot()
    {
        if (targetPlayer == null || projectilePrefab == null || firePoint == null) return;

        Vector3 direction = (targetPlayer.transform.position - firePoint.position).normalized;
        
        try
        {
            GameObject projectile = PhotonNetwork.Instantiate(
                projectilePrefab.name, 
                firePoint.position, 
                Quaternion.LookRotation(direction)
            );

            if (shootSound != null)
            {
                shootSound.Play();
            }

            Projectile projectileComponent = projectile.GetComponent<Projectile>();
            if (projectileComponent != null)
            {
                projectileComponent.SetDirection(direction);
                Debug.Log("Proyectil disparado hacia: " + direction);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al instanciar el proyectil: " + e.Message);
        }
    }

    public void PlayDeathEffect()
    {
        if (deathParticleSystem != null)
        {
            deathParticleSystem.Play();
            Debug.Log("Efecto de muerte reproducido.");
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet_2"))
        {
            Debug.Log("Hity");
            PlayDeathEffect();
            StartCoroutine(DestroyAfterDelay(0.5f)); // 1 segundos de retraso
        }
    }
}