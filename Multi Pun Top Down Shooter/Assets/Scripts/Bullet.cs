using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    // Velocidad de la bala
    public float speed;

    // Dueño de la bala (jugador que la disparó)
    public Photon.Realtime.Player owner;

    // Método para inicializar la bala
    public void Initialize(float bulletSpeed, Photon.Realtime.Player bulletOwner)
    {
        speed = bulletSpeed; // Asigna la velocidad
        owner = bulletOwner; // Asigna el dueño
    }

    void Start()
    {
        // Solo el dueño del PhotonView ejecuta esta lógica
        if (photonView.IsMine)
        {
            // Programa la destrucción de la bala después de 1 segundo
            Invoke("DestroyBullet", 1f);
        }
    }

    void Update()
    {
        // Solo el dueño del PhotonView ejecuta esta lógica
        if (photonView.IsMine)
        {
            // Mueve la bala hacia adelante en su dirección actual
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Solo el dueño del PhotonView ejecuta esta lógica
        if (photonView.IsMine)
        {
            // Verifica si la bala chocó con algo que no sea el jugador
            if (!other.CompareTag("Player"))
            {
                DestroyBullet(); // Destruye la bala
            }

            // Verificar si golpeó a un enemigo
            if (other.CompareTag("Enemy"))
            {
                // Obtener el componente EnemyShooter y verificar que existe
                EnemyShooter enemyShooter = other.gameObject.GetComponent<EnemyShooter>();
                if (enemyShooter != null)
                {
                    enemyShooter.photonView.RPC("TakeDamage", RpcTarget.All, 10f);
                    Debug.Log("Hit enemy and applying damage");
                }
                DestroyBullet(); // Destruye la bala
            }
        }
    }

    void DestroyBullet()
    {
        // Solo el dueño del PhotonView ejecuta esta lógica
        if (photonView.IsMine)
        {
            // Destruye la bala en la red
            PhotonNetwork.Destroy(gameObject);
        }
    }
}