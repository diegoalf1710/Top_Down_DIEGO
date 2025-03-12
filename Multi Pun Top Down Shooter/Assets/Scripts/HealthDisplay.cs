using UnityEngine;
using TMPro;
using Photon.Pun;

/// <summary>
/// Controla la visualización de la salud del jugador en la interfaz de usuario.
/// Muestra la salud actual y cambia el color según el nivel de vida.
/// </summary>
public class HealthDisplay : MonoBehaviour
{
    [Header("References")]
    /// <summary>
    /// Referencia al componente de salud del jugador local
    /// </summary>
    public PlayerHealth playerHealth;

    /// <summary>
    /// Componente de texto que muestra la salud actual
    /// </summary>
    public TextMeshProUGUI healthText;

    [Header("Display Settings")]
    /// <summary>
    /// Color que se muestra cuando el jugador tiene la salud completa
    /// </summary>
    [SerializeField] private Color fullHealthColor = Color.green;

    /// <summary>
    /// Color que se muestra cuando el jugador tiene poca salud
    /// </summary>
    [SerializeField] private Color lowHealthColor = Color.red;

    /// <summary>
    /// Umbral de salud por debajo del cual se considera "poca salud"
    /// </summary>
    [SerializeField] private float lowHealthThreshold = 30f;

    /// <summary>
    /// Se ejecuta al iniciar. Busca al jugador local y configura la actualización periódica.
    /// </summary>
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

    /// <summary>
    /// Busca al jugador local en la escena y se suscribe a sus eventos de salud.
    /// Si no encuentra al jugador, seguirá buscando periódicamente.
    /// </summary>
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

    /// <summary>
    /// Actualiza la visualización de la salud en la interfaz.
    /// Cambia el texto y el color según la cantidad de vida actual.
    /// </summary>
    /// <param name="currentHealth">Cantidad actual de salud del jugador</param>
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

    /// <summary>
    /// Se ejecuta al destruir el objeto. Limpia las suscripciones a eventos.
    /// </summary>
    private void OnDestroy()
    {
        // Desuscribirse del evento cuando se destruye el objeto
        if (playerHealth != null)
        {
            playerHealth.onHealthChanged -= UpdateHealthDisplay;
        }
    }
}