using UnityEngine;
using Photon.Pun;

/// <summary>
/// Gestiona la recolección de monedas para el jugador en el juego multijugador.
/// Hereda de MonoBehaviourPun para la funcionalidad en red.
/// </summary>
public class CoinCollector : MonoBehaviourPun
{
    /// <summary>
    /// Referencia al administrador de UI del jugador para actualizar el contador de monedas.
    /// </summary>
    private PlayerUIManager uiManager;

    /// <summary>
    /// Se ejecuta al iniciar. Obtiene la referencia al PlayerUIManager si es el jugador local.
    /// </summary>
    private void Start()
    {
        if (photonView.IsMine)
        {
            uiManager = GetComponent<PlayerUIManager>();
        }
    }

    /// <summary>
    /// Detecta cuando el jugador colisiona con una moneda.
    /// Si es el jugador local, actualiza el contador de monedas y destruye la moneda en la red.
    /// </summary>
    /// <param name="other">El colisionador del objeto con el que se produce el contacto</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!photonView.IsMine) return;

        if (other.CompareTag("Coin"))
        {
            uiManager.UpdateCoins(1);
            PhotonNetwork.Destroy(other.gameObject);
        }
    }
}
