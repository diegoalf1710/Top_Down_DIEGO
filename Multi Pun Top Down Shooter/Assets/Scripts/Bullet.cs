using UnityEngine;
using Photon.Pun;

/// <summary>
/// Controla el comportamiento de las balas en el juego multijugador.
/// Hereda de MonoBehaviourPun para la funcionalidad en red.
/// </summary>
public class Bullet : MonoBehaviourPun
{
    /// <summary>
    /// Velocidad de movimiento de la bala.
    /// </summary>
    public float speed;

    /// <summary>
    /// Referencia al jugador que disparó la bala.
    /// </summary>
    public Photon.Realtime.Player owner;

    /// <summary>
    /// Inicializa la bala con los parámetros especificados.
    /// </summary>
    /// <param name="bulletSpeed">Velocidad inicial de la bala</param>
    /// <param name="bulletOwner">Jugador que disparó la bala</param>
    public void Initialize(float bulletSpeed, Photon.Realtime.Player bulletOwner)
    {
        speed = bulletSpeed; 
        owner = bulletOwner; 
    }

    /// <summary>
    /// Se ejecuta al crear la bala. Programa su destrucción automática después de 1 segundo.
    /// Solo se ejecuta en la instancia del propietario de la bala.
    /// </summary>
    void Start()
    {
        if (photonView.IsMine)
        {
            Invoke("DestroyBullet", 1f);
        }
    }

    /// <summary>
    /// Actualiza la posición de la bala cada frame.
    /// Solo se ejecuta en la instancia del propietario de la bala.
    /// </summary>
    void Update()
    {
        if (photonView.IsMine)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Maneja las colisiones de la bala con otros objetos.
    /// Detecta impactos con enemigos y aplica daño.
    /// </summary>
    /// <param name="other">Colisionador del objeto impactado</param>
    void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine)
        {
            if (!other.CompareTag("Player"))
            {
                DestroyBullet();
            }

            if (other.CompareTag("Enemy"))
            {
                EnemyShooter enemyShooter = other.gameObject.GetComponent<EnemyShooter>();
                if (enemyShooter != null)
                {
                    enemyShooter.photonView.RPC("TakeDamage", RpcTarget.All, 10f);
                    Debug.Log("Hit enemy and applying damage");
                }
                DestroyBullet();
            }
        }
    }

    /// <summary>
    /// Destruye la bala en la red.
    /// Solo se ejecuta en la instancia del propietario de la bala.
    /// </summary>
    void DestroyBullet()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}