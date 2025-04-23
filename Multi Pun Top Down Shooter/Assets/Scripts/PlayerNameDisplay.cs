using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerNameDisplay : MonoBehaviourPun
{
    private TextMeshProUGUI playerNameText;

    void Start()
    {
        // Buscar el componente TextMeshProUGUI en los hijos del Canvas
        playerNameText = GetComponentInChildren<TextMeshProUGUI>();
        
        if (playerNameText == null)
        {
            Debug.LogError("No se encontró el componente TextMeshProUGUI en los hijos del jugador");
            return;
        }

        if (photonView.IsMine)
        {
            playerNameText.text = "Jugador Local";
            playerNameText.color = Color.white;
            Debug.Log("Nombre asignado: " + playerNameText.text);
        }
        else
        {
            playerNameText.text = "Otro Jugador";
            Debug.Log("Nombre asignado: " + playerNameText.text);
        }
    }
}