using Photon.Pun;
using UnityEngine;

// Clase que maneja el comportamiento de los proyectiles en el juego.
// Controla el movimiento, la vida útil y el daño que causa al impactar.
public class Projectile : MonoBehaviour
{
    // Velocidad a la que se mueve el proyectil en unidades por segundo
    public float speed = 10f;
    // Tiempo en segundos que el proyectil existe antes de autodestruirse
    public float lifetime = 5f;
    // Vector que determina hacia dónde se mueve el proyectil
    private Vector3 direction;

    // Se ejecuta cuando el proyectil es creado
    // Programa la autodestrucción del proyectil después del tiempo especificado
    void Start()
    {
        Destroy(gameObject, lifetime); // Destruir el proyectil después de un tiempo
    }

    // Se ejecuta cada frame
    // Actualiza la posición del proyectil según su dirección y velocidad
    void Update()
    {
        // Mover el proyectil en la dirección asignada
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    // Establece la dirección en la que el proyectil se moverá
    // newDirection: Vector que indica la nueva dirección del proyectil
    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
    }

    // Se ejecuta cuando el proyectil colisiona con otro objeto
    // Detecta colisiones con jugadores y les aplica daño
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