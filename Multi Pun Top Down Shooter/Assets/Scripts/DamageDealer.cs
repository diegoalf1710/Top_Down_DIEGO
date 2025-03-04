using UnityEngine;
using Photon.Pun;

public class DamageDealer : MonoBehaviour
{
    public int damageAmount = 10; // Cantidad de daño que hace este objeto

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