using UnityEngine;
using TMPro;
using Photon.Pun;

public class HealthDisplay : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;
    public TextMeshProUGUI healthText;

    [Header("Display Settings")]
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private float lowHealthThreshold = 30f;

    void Start()
    {
        // Buscar el jugador local
        FindLocalPlayer();
        // Intentar encontrar el jugador cada 0.5 segundos si no se encuentra inicialmente
        if (playerHealth == null)
        {
            InvokeRepeating("FindLocalPlayer", 0.5f, 0.5f);
        }
    }

    void FindLocalPlayer()
    {
        if (playerHealth != null) return; // Si ya tenemos el jugador, no hacer nada

        // Buscar todos los jugadores en la escena
        PlayerHealth[] players = FindObjectsOfType<PlayerHealth>();
        
        foreach (PlayerHealth player in players)
        {
            // Verificar si es el jugador local
            if (player.photonView.IsMine)
            {
                playerHealth = player;
                playerHealth.onHealthChanged -= UpdateHealthDisplay; // Eliminar suscripción previa si existe
                playerHealth.onHealthChanged += UpdateHealthDisplay; // Suscribirse al evento
                
                // Actualizar el display inmediatamente con la vida actual
                UpdateHealthDisplay(player.currentHealth);
                
                // Cancelar la búsqueda repetida si estaba activa
                CancelInvoke("FindLocalPlayer");
                Debug.Log("Jugador local encontrado y display actualizado");
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
        if (healthText == null) return;

        // Actualizar el texto
        healthText.text = $"Vida: {currentHealth}";

        // Calcular el color basado en la vida actual
        float healthPercentage = currentHealth / 100f; // Asumiendo que 100 es la vida máxima
        healthText.color = Color.Lerp(lowHealthColor, fullHealthColor, healthPercentage);

        if (currentHealth <= lowHealthThreshold)
        {
            healthText.color = lowHealthColor;
        }
        else
        {
            healthText.color = fullHealthColor;
        }

        Debug.Log($"Vida actualizada: {currentHealth}, Color: {healthText.color}");
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