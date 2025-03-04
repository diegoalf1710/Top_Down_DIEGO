using UnityEngine;
using Photon.Pun;

public class PlayerShooting : MonoBehaviourPun
{
    // Punto desde donde se disparan las balas
    public Transform firePoint;

    // Velocidad de la bala
    public float bulletSpeed = 10f;

    // Prefab de la bala (debe estar en la carpeta Resources)
    public GameObject bulletPrefab;

    void Update()
    {
        // Verifica si el jugador local es el dueño del PhotonView y si presiona el botón de disparo
        if (photonView.IsMine && Input.GetButtonDown("Fire1"))
        {
            Shoot(); // Llama al método para disparar
        }
    }

    void Shoot()
    {
        // Instancia la bala en la red usando PhotonNetwork.Instantiate
        // Solo el jugador local (dueño del PhotonView) ejecuta esta lógica
        GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, firePoint.rotation);

        // Inicializa la bala con la velocidad y el dueño (jugador que disparó)
        bullet.GetComponent<Bullet>().Initialize(bulletSpeed, photonView.Owner);
    }
}