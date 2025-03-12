using UnityEngine;
using Photon.Pun;

// Esta clase maneja el sistema de disparo del jugador en red.
// Hereda de MonoBehaviourPun para tener acceso a la funcionalidad de red de Photon.
public class PlayerShooting : MonoBehaviourPun
{
    // Punto desde donde se disparan las balas
    // Este Transform debe ser asignado en el Inspector y representa la posición de origen del disparo
    public Transform firePoint;

    // Velocidad con la que se moverá la bala una vez disparada
    // Puede ser modificado desde el Inspector de Unity
    public float bulletSpeed = 10f;

    // Prefab que representa la bala que será instanciada
    // IMPORTANTE: Este prefab debe estar ubicado en la carpeta Resources del proyecto
    public GameObject bulletPrefab;

    // Se ejecuta cada frame para verificar si el jugador quiere disparar
    void Update()
    {
        // Verifica si el jugador local es el dueño del PhotonView y si presiona el botón de disparo
        if (photonView.IsMine && Input.GetButtonDown("Fire1"))
        {
            Shoot(); // Llama al método para disparar
        }
    }

    // Método que maneja la lógica de disparo
    // Crea una nueva bala en la red y la inicializa con los parámetros necesarios
    void Shoot()
    {
        // Instancia la bala en la red usando PhotonNetwork.Instantiate
        // Solo el jugador local (dueño del PhotonView) ejecuta esta lógica
        GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, firePoint.rotation);

        // Inicializa la bala con la velocidad y el dueño (jugador que disparó)
        bullet.GetComponent<Bullet>().Initialize(bulletSpeed, photonView.Owner);
    }
}