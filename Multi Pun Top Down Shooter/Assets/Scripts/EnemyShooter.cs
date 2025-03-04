using UnityEngine;
using Photon.Pun;

public class EnemyShooter : MonoBehaviourPun
{
    public GameObject projectilePrefab; // Prefab del proyectil
    public Transform firePoint; // Punto de origen del disparo
    public float fireRate = 1f; // Tiempo entre disparos (en segundos)
    public float detectionRange = 10f; // Rango de detección del jugador
    public float rotationSpeed = 5f; // Velocidad de rotación hacia el jugador

    private float nextFireTime = 0f; // Tiempo para el próximo disparo
    private GameObject targetPlayer; // Jugador objetivo

    void Update()
    {
        if (!photonView.IsMine) // Solo el dueño del PhotonView puede disparar
            return;

        FindNearestPlayer(); // Buscar al jugador más cercano

        if (targetPlayer != null)
        {
            RotateTowardsPlayer(); // Girar hacia el jugador

            if (Time.time >= nextFireTime)
            {
                Shoot(); // Disparar hacia el jugador
                nextFireTime = Time.time + fireRate; // Actualizar el tiempo del próximo disparo
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
            // Calcular la dirección hacia el jugador
            Vector3 direction = (targetPlayer.transform.position - transform.position).normalized;

            // Calcular la rotación necesaria para mirar hacia el jugador
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            // Suavizar la rotación hacia el jugador
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            // Asegurarse de que el firePoint también apunte hacia el jugador
            if (firePoint != null)
            {
                firePoint.rotation = lookRotation;
            }
        }
    }

    // Método para disparar
    void Shoot()
    {
        if (targetPlayer != null && projectilePrefab != null && firePoint != null)
        {
            // Calcular la dirección hacia el jugador
            Vector3 direction = (targetPlayer.transform.position - firePoint.position).normalized;

            // Instanciar el proyectil en la red
            GameObject projectile = PhotonNetwork.Instantiate(projectilePrefab.name, firePoint.position, firePoint.rotation);
            projectile.GetComponent<Projectile>().SetDirection(direction); // Establecer la dirección del proyectil
        }
    }
}