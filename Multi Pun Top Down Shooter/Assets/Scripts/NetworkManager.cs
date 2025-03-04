using UnityEngine;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        // Conectar al servidor de Photon
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado al servidor de Photon.");
        // Unirse a una sala aleatoria
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No se encontró una sala, creando una nueva...");
        // Crear una nueva sala si no hay ninguna disponible
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 4 });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Unido a una sala.");
        // Obtener una posición de spawn aleatoria
        Vector3 spawnPosition = SpawnPointManager.Instance.GetRandomSpawnPoint();
        // Instanciar al jugador en el punto de spawn
        PhotonNetwork.Instantiate("Player", spawnPosition, Quaternion.identity);
    }
}
