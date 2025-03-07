using UnityEngine;
using TMPro;
using Photon.Pun;

public class GameManager : MonoBehaviourPun
{
    public GameObject gameOverPanel; // Panel de "Game Over"
    public TextMeshProUGUI gameOverText; // Texto de "Game Over"
    public PlayerUIManager playerUIManager; // Referencia al UI Manager

    void Start()
    {
        gameOverPanel.SetActive(false); // Ocultar el panel al inicio
        if (playerUIManager != null)
        {
            playerUIManager.UpdateHealth(100); // Inicializar UI con vida completa
        }
    }

    public void ShowGameOver(string playerName)
    {
        if (photonView.IsMine)
        {
            gameOverPanel.SetActive(true);
            gameOverText.text = playerName + " ha sido eliminado";
        }
    }
}