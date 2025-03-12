using UnityEngine;
using Photon.Pun;

/// <summary>
/// Gestiona la aplicación de daño a los jugadores en el juego multijugador.
/// Se encarga de detectar colisiones y aplicar daño a los jugadores afectados.
/// </summary>
public class DamageDealer : MonoBehaviour
{
    /// <summary>
    /// Cantidad de daño que este objeto aplica al colisionar con un jugador.
    /// </summary>
    public int damageAmount = 10;

    /// <summary>
    /// Se ejecuta cuando este objeto colisiona con otro collider.
    /// Detecta si el objeto golpeado es un jugador y le aplica el daño correspondiente.
    /// </summary>
    /// <param name="collision">Información sobre la colisión ocurrida</param>
    void OnCollisionEnter(Collision collision)
    {
        // Verificar si el objeto golpeado es un jugador
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            // Llamar al método TakeDamage en el jugador
            playerHealth.photonView.RPC("TakeDamage", RpcTarget.All, damageAmount);
        }
    }
}