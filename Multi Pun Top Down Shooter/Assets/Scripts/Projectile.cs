using Photon.Pun;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f; // Velocidad del proyectil
    public float lifetime = 5f; // Tiempo de vida del proyectil
    private Vector3 direction; // Dirección del proyectil

    void Start()
    {
        Destroy(gameObject, lifetime); // Destruir el proyectil después de un tiempo
    }

    void Update()
    {
        // Mover el proyectil en la dirección asignada
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    // Método para establecer la dirección del proyectil
    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Si el proyectil choca con un jugador, causar daño
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.photonView.RPC("TakeDamage", RpcTarget.All, 10); // Causar 10 de daño
            }
        }

        Destroy(gameObject); // Destruir el proyectil al chocar
    }
}