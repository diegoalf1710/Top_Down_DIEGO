using UnityEngine;
using Photon.Pun;

public class PlayerHealth : MonoBehaviourPun
{
    public int maxHealth = 100;
    public int currentHealth;

    // Evento para notificar cambios en la vida
    public System.Action<int> onHealthChanged;

    private void Start()
    {
        currentHealth = maxHealth;
        onHealthChanged?.Invoke((int)currentHealth); // Notificar el estado inicial de la salud
    }

    public bool Heal(int amount)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            Debug.Log($"Player healed for {amount}. Current health: {currentHealth}");
            return true;
        }
        return false;
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (photonView.IsMine)
        {
            currentHealth = Mathf.Max(currentHealth - damage, 0);
            onHealthChanged?.Invoke((int)currentHealth); // Notificar el cambio de vida

            if (currentHealth <= 0)
            {
                Die();
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

    private void Die()
    {
        Debug.Log("Player died!");
        // Lógica de fin de juego
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject); // Destruir el objeto del jugador en la red
            FindObjectOfType<GameManager>().ShowGameOver(PhotonNetwork.NickName); // Mostrar "Game Over"
        }
    }
}