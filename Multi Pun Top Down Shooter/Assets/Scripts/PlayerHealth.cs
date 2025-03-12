using UnityEngine;
using Photon.Pun;

// Clase que maneja la salud del jugador en la red
// Gestiona el daño recibido, curación y muerte del personaje
// Utiliza PhotonView para sincronizar el estado de salud entre todos los jugadores
public class PlayerHealth : MonoBehaviourPun
{
    // Salud máxima del jugador
    public int maxHealth = 100;
    // Salud actual del jugador
    public int currentHealth;

    // Evento para notificar cambios en la vida
    public System.Action<int> onHealthChanged;

    // Inicializa la salud del jugador al valor máximo
    private void Start()
    {
        currentHealth = maxHealth;
        onHealthChanged?.Invoke((int)currentHealth); // Notificar el estado inicial de la salud
    }

    // Intenta curar al jugador por la cantidad especificada
    // Retorna true si la curación fue exitosa, false si el jugador ya tiene la salud máxima
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

    // Método llamado a través de la red cuando el jugador recibe daño
    // Solo se procesa si es el jugador local
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

    // Detecta colisiones con objetos marcados como "Bullet" y aplica daño
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // Asumimos que el daño de la bala es 10, puedes ajustar esto según sea necesario
            TakeDamage(10);
            Debug.Log("Impacto de bala recibido.");
        }
    }

    // Maneja la muerte del jugador
    // Destruye el objeto del jugador en la red y muestra la pantalla de fin de juego
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