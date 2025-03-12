using UnityEngine;
using Photon.Pun;

/// <summary>
/// Controla el comportamiento de la cámara principal del juego.
/// Se encarga de seguir al jugador local de forma suave.
/// </summary>
public class CameraManager : MonoBehaviour
{
    /// <summary>
    /// Referencia al objeto del jugador que la cámara debe seguir.
    /// </summary>
    public GameObject player;

    /// <summary>
    /// Velocidad de suavizado para el movimiento de la cámara.
    /// </summary>
    [SerializeField] public float smoothSpeed = 5f;

    /// <summary>
    /// Desplazamiento de la cámara respecto a la posición del jugador.
    /// </summary>
    [SerializeField] public Vector3 offset = new Vector3(0, 10, -3);

    /// <summary>
    /// Indica si la cámara debe seguir al jugador.
    /// </summary>
    public bool isFollowing = true;

    /// <summary>
    /// Se ejecuta al iniciar. Espera unos segundos antes de buscar al jugador
    /// para asegurar que se haya instanciado correctamente.
    /// </summary>
    void Start()
    {
        // Esperar un momento para que el jugador se instancie
        Invoke("FindPlayer", 4f);
    }

    /// <summary>
    /// Busca el objeto del jugador local entre todos los jugadores en la escena.
    /// Solo asigna el jugador si es el controlado localmente.
    /// </summary>
    void FindPlayer()
    {
        // Buscar al jugador local
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            PhotonView pv = p.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                player = p;
                break;
            }
        }
    }

    /// <summary>
    /// Actualiza la posición de la cámara cada frame después de que todos los updates hayan ocurrido.
    /// Realiza un seguimiento suave del jugador si está asignado y el seguimiento está activado.
    /// </summary>
    void LateUpdate()
    {
        if (player != null && isFollowing)
        {
            // Calcular la posición objetivo
            Vector3 desiredPosition = player.transform.position + offset;
            // Suavizar el movimiento
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
    }

    /// <summary>
    /// Detiene el seguimiento de la cámara al jugador.
    /// </summary>
    public void StopFollowing()
    {
        isFollowing = false;
    }

    /// <summary>
    /// Reanuda el seguimiento de la cámara al jugador.
    /// </summary>
    public void StartFollowing()
    {
        isFollowing = true;
    }
}
