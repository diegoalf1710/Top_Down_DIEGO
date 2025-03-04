using UnityEngine;
using TMPro;
using Photon.Pun;

public class HealthDisplay : MonoBehaviour
{
    [Header("References")]
    private PlayerHealth playerHealth;
    public TextMeshProUGUI healthText;

    [Header("Display Settings")]
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private float lowHealthThreshold = 30f;

    void Start()
    {
        // Buscar el jugador local
        FindLocalPlayer();
    }

    void FindLocalPlayer()
    {
        // Buscar todos los jugadores en la escena
        PlayerHealth[] players = FindObjectsOfType<PlayerHealth>();
        
        foreach (PlayerHealth player in players)
        {
            // Verificar si es el jugador local
            if (player.photonView.IsMine)
            {
                playerHealth = player;
                // Suscribirse al evento de cambio de vida
                playerHealth.onHealthChanged += UpdateHealthDisplay;
                Debug.Log("Jugador local encontrado y suscrito al evento de cambio de vida.");
                break;
            }
        }

        if (playerHealth == null)
        {
            Debug.LogWarning("No se encontró el jugador local");
        }
    }

    private void UpdateHealthDisplay(int currentHealth)
    {
        if (healthText != null)
        {
            // Actualizar el texto con la vida actual
            healthText.text = $"Vida: {currentHealth}";
            Debug.Log($"Vida actualizada: {currentHealth}");
        }
    }

    private void OnDestroy()
    {
        // Desuscribirse del evento cuando se destruye el objeto
        if (playerHealth != null)
        {
            playerHealth.onHealthChanged -= UpdateHealthDisplay;
        }
    }
}