using UnityEngine;
using TMPro;
using Photon.Pun;

// Clase que maneja la interfaz de usuario del jugador, incluyendo la visualización
// de la salud y las monedas. Hereda de MonoBehaviourPun para funcionalidad multijugador.
public class PlayerUIManager : MonoBehaviourPun
{
    // Referencias a los objetos de UI que muestran la información
    public GameObject healthTextObject;
    public GameObject coinsTextObject;
    
    // Componentes de texto para mostrar la salud y monedas
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI coinsText;
    public int coins = 0;

    // Se llama cuando se inicializa el script
    // Obtiene las referencias necesarias a los componentes de texto
    private void Awake()
    {
        // Obtener los componentes TextMeshProUGUI de los GameObjects
        if (healthTextObject != null)
            healthText = healthTextObject.GetComponent<TextMeshProUGUI>();
        
        if (coinsTextObject != null)
            coinsText = coinsTextObject.GetComponent<TextMeshProUGUI>();
    }

    // Se llama al inicio del juego
    // Inicializa los valores de salud y monedas si este es el jugador local
    private void Start()
    {
        if (photonView.IsMine)
        {
            UpdateHealth(100); // Inicializar con vida completa
            UpdateCoins(0);    // Inicializar monedas en 0
        }
    }

    // Actualiza el texto de salud en la UI
    // currentHealth: valor actual de la salud del jugador
    public void UpdateHealth(int currentHealth)
    {
        if (photonView.IsMine && healthText != null)
        {
            healthText.text = "HP: " + currentHealth.ToString();
        }
    }

    // Actualiza el contador de monedas en la UI
    // amount: cantidad de monedas a añadir (se multiplica por 10)
    public void UpdateCoins(int amount)
    {
        if (photonView.IsMine && coinsText != null)
        {
            coins += amount * 10; // Multiplicar por 10 cada moneda recogida
            coinsText.text = "Coins: " + coins.ToString();
        }
    }
}
