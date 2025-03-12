using UnityEngine;
using TMPro;
using Photon.Pun;

/// <summary>
/// Gestiona el estado general del juego y las condiciones de finalización.
/// Controla la interfaz de usuario relacionada con el fin del juego.
/// Hereda de MonoBehaviourPun para la funcionalidad en red.
/// </summary>
public class GameManager : MonoBehaviourPun
{
    /// <summary>
    /// Panel que se muestra cuando termina el juego para un jugador
    /// </summary>
    public GameObject gameOverPanel;

    /// <summary>
    /// Componente de texto que muestra el mensaje de fin de juego
    /// </summary>
    public TextMeshProUGUI gameOverText;

    /// <summary>
    /// Referencia al gestor de interfaz de usuario del jugador
    /// </summary>
    public PlayerUIManager playerUIManager;

    /// <summary>
    /// Se ejecuta al iniciar. Configura el estado inicial del juego.
    /// Oculta el panel de fin de juego e inicializa la interfaz de usuario.
    /// </summary>
    void Start()
    {
        gameOverPanel.SetActive(false);
        if (playerUIManager != null)
        {
            playerUIManager.UpdateHealth(100);
        }
    }

    /// <summary>
    /// Muestra el panel de fin de juego con el nombre del jugador eliminado.
    /// Solo se ejecuta en la instancia local del jugador.
    /// </summary>
    /// <param name="playerName">Nombre del jugador que ha sido eliminado</param>
    public void ShowGameOver(string playerName)
    {
        if (photonView.IsMine)
        {
            gameOverPanel.SetActive(true);
            gameOverText.text = playerName + " ha sido eliminado";
        }
    }
}