using UnityEngine;
using Photon.Pun;

// Clase que maneja la conexión y gestión de red usando Photon PUN.
// Se encarga de conectar al servidor, unirse o crear salas y gestionar la instanciación de jugadores.
public class NetworkManager : MonoBehaviourPunCallbacks
{
    // Método que se ejecuta al iniciar el script.
    // Inicia la conexión con los servidores de Photon usando la configuración predeterminada.
    void Start()
    {
        // Conectar al servidor de Photon
        PhotonNetwork.ConnectUsingSettings();
    }

    // Se ejecuta cuando el cliente se conecta exitosamente al servidor maestro de Photon.
    // Intenta unirse a una sala aleatoria una vez establecida la conexión.
    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado al servidor de Photon.");
        // Unirse a una sala aleatoria
        PhotonNetwork.JoinRandomRoom();
    }

    // Se ejecuta cuando falla el intento de unirse a una sala aleatoria.
    // Crea una nueva sala si no se encuentra ninguna disponible.
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No se encontró una sala, creando una nueva...");
        // Crear una nueva sala si no hay ninguna disponible
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 4 });
    }

    // Se ejecuta cuando el cliente se une exitosamente a una sala.
    // Obtiene una posición aleatoria y crea la instancia del jugador en la red.
    public override void OnJoinedRoom()
    {
        Debug.Log("Unido a una sala.");
        // Obtener una posición de spawn aleatoria
        Vector3 spawnPosition = SpawnPointManager.Instance.GetRandomSpawnPoint();
        // Instanciar al jugador en el punto de spawn
        PhotonNetwork.Instantiate("Player", spawnPosition, Quaternion.identity);
    }
}
