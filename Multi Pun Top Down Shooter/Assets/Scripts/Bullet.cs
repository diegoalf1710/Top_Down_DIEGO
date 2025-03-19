using UnityEngine;
using Photon.Pun;

/// <summary>
/// Controla el comportamiento de las balas en el juego multijugador.
/// Hereda de MonoBehaviourPun para la funcionalidad en red.
/// </summary>
public class Bullet : MonoBehaviourPun
{
    #region Variables
    [Header("Bullet Properties")]
    [Tooltip("Velocidad de movimiento de la bala")]
    public float speed;

    [Header("Network Properties")]
    public Photon.Realtime.Player owner;
    #endregion

    #region Unity Methods
    /// <summary>
    /// Se ejecuta al crear la bala. Programa su destrucción automática después de 1 segundo.
    /// Solo se ejecuta en la instancia del propietario de la bala.
    /// </summary>
    void Start()
    {
        if (photonView.IsMine)
        {
            Invoke("DestroyBullet", 1.5f);
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
        if (!photonView.IsMine || other.CompareTag("Player")) return;

        switch (other.tag)
        {
            case "Enemy":
                HandleEnemyCollision(other);
                break;
            case "Wall":
                HandleWallCollision();
                break;
            case "Obstacle":
                HandleObstacleCollision();
                break;
            default:
                DestroyBullet();
                break;
        }
    }
    #endregion

    #region Public Methods
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
    #endregion

    #region Private Methods
    private void HandleEnemyCollision(Collider enemyCollider)
    {
        
        GameObject.Destroy(this.gameObject);
    }

    private void HandleWallCollision()
    {
        Debug.Log("Hit wall");
        DestroyBullet();
    }

    private void HandleObstacleCollision()
    {
        Debug.Log("Hit obstacle");
        DestroyBullet();
    }

    /// <summary>
    /// Destruye la bala en la red.
    /// Solo se ejecuta en la instancia del propietario de la bala.
    /// </summary>
    private void DestroyBullet()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
    #endregion
}