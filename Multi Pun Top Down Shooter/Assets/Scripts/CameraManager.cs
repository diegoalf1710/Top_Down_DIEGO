using UnityEngine;
using Photon.Pun;

public class CameraManager : MonoBehaviour
{
    public GameObject player;
    [SerializeField] public float smoothSpeed = 5f;
    [SerializeField] public Vector3 offset = new Vector3(0, 10, -3);
    public bool isFollowing = true;

    void Start()
    {
        // Esperar un momento para que el jugador se instancie
        Invoke("FindPlayer", 4f);
    }

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

    public void StopFollowing()
    {
        isFollowing = false;
    }

    public void StartFollowing()
    {
        isFollowing = true;
    }
}
