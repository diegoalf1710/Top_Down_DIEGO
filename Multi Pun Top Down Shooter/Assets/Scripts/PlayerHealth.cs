using UnityEngine;
using Photon.Pun;

public class PlayerHealth : MonoBehaviourPun
{
    public int currentHealth = 100; // Vida inicial del jugador

    // Evento para notificar cambios en la vida
    public System.Action<int> onHealthChanged;

    void Start()
    {
        onHealthChanged?.Invoke(currentHealth); // Notificar el estado inicial de la salud
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (photonView.IsMine)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Max(0, currentHealth); // Evitar valores negativos
            
            // Notificar el cambio de vida
            onHealthChanged?.Invoke(currentHealth);

            if (currentHealth <= 0)
            {
                GameOver();
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // Asumimos que el daño de la bala es 10, puedes ajustar esto según sea necesario
            TakeDamage(10);
            Debug.Log("Impacto de bala recibido.");
        }
    }

    void GameOver()
    {
        // Lógica de fin de juego
        Debug.Log("Game Over");
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject); // Destruir el objeto del jugador en la red
            FindObjectOfType<GameManager>().ShowGameOver(PhotonNetwork.NickName); // Mostrar "Game Over"
        }
    }
}