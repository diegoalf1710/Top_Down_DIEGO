using UnityEngine;
using Photon.Pun;

/// <summary>
/// Clase base para todos los objetos interactivos del juego.
/// Proporciona funcionalidad básica para interacción con el jugador y retroalimentación visual.
/// </summary>
public class InteractiveItem : MonoBehaviourPun
{
    [Header("Item Settings")]
    /// <summary>
    /// Nombre identificativo del objeto interactivo
    /// </summary>
    public string itemName = "Default Item";

    /// <summary>
    /// Radio en unidades dentro del cual el jugador puede interactuar con el objeto
    /// </summary>
    public float interactionRadius = 2f;

    /// <summary>
    /// Si es true, el objeto se destruirá después de una interacción
    /// </summary>
    public bool oneTimeUse = true;
    
    [Header("Visual Feedback")]
    /// <summary>
    /// Determina si el objeto rota continuamente sobre sí mismo
    /// </summary>
    public bool rotateItem = true;

    /// <summary>
    /// Velocidad de rotación del objeto en grados por segundo
    /// </summary>
    public float rotationSpeed = 50f;

    [Header("Interaction Settings")]
    /// <summary>
    /// Tag que identifica al jugador para la interacción
    /// </summary>
    public string playerTag = "Player";

    /// <summary>
    /// Si es true, requiere presionar una tecla para interactuar
    /// </summary>
    public bool requireKeyPress = true;

    /// <summary>
    /// Tecla que debe presionar el jugador para interactuar
    /// </summary>
    public KeyCode interactionKey = KeyCode.E;
    
    /// <summary>
    /// Indica si hay un jugador dentro del radio de interacción
    /// </summary>
    protected bool playerInRange = false;

    /// <summary>
    /// Referencia al objeto del jugador que está en rango
    /// </summary>
    protected GameObject playerObject = null;

    /// <summary>
    /// Se ejecuta al iniciar. Configura el collider si no existe.
    /// </summary>
    protected virtual void Start()
    {
        // Asegurarnos de que tiene un collider y está en trigger
        if (GetComponent<Collider>() == null)
        {
            SphereCollider collider = gameObject.AddComponent<SphereCollider>();
            collider.radius = interactionRadius;
            collider.isTrigger = true;
        }
    }

    /// <summary>
    /// Actualiza el objeto cada frame. Maneja la rotación y la interacción con tecla.
    /// </summary>
    protected virtual void Update()
    {
        if (rotateItem)
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }

        // Verificar interacción con tecla
        if (playerInRange && requireKeyPress && Input.GetKeyDown(interactionKey))
        {
            OnInteract(playerObject);
        }
    }

    /// <summary>
    /// Se ejecuta cuando el jugador interactúa con el objeto.
    /// </summary>
    /// <param name="player">Objeto del jugador que interactúa</param>
    public virtual void OnInteract(GameObject player)
    {
        Debug.Log($"Player interacted with {itemName}");
        if (oneTimeUse)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Se ejecuta cuando un objeto entra en el radio de interacción.
    /// </summary>
    /// <param name="other">Collider del objeto que entró en el radio</param>
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
            playerObject = other.gameObject;
            OnPlayerEnterRange(other.gameObject);
        }
    }

    /// <summary>
    /// Se ejecuta cuando un objeto sale del radio de interacción.
    /// </summary>
    /// <param name="other">Collider del objeto que salió del radio</param>
    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            playerObject = null;
            OnPlayerExitRange(other.gameObject);
        }
    }

    /// <summary>
    /// Se ejecuta cuando el jugador entra en el radio de interacción.
    /// </summary>
    /// <param name="player">Objeto del jugador que entró en rango</param>
    protected virtual void OnPlayerEnterRange(GameObject player)
    {
        // Si no requiere tecla, interactúa automáticamente
        if (!requireKeyPress)
        {
            OnInteract(player);
        }
        else
        {
            ShowInteractionPrompt();
        }
    }

    /// <summary>
    /// Se ejecuta cuando el jugador sale del radio de interacción.
    /// </summary>
    /// <param name="player">Objeto del jugador que salió del rango</param>
    protected virtual void OnPlayerExitRange(GameObject player)
    {
        HideInteractionPrompt();
    }

    /// <summary>
    /// Muestra el indicador visual de interacción al jugador.
    /// </summary>
    protected virtual void ShowInteractionPrompt()
    {
        Debug.Log($"Presiona {interactionKey} para interactuar con {itemName}");
        // Aquí puedes mostrar un UI prompt
    }

    /// <summary>
    /// Oculta el indicador visual de interacción.
    /// </summary>
    protected virtual void HideInteractionPrompt()
    {
        // Aquí puedes ocultar el UI prompt
    }

    /// <summary>
    /// Dibuja gizmos en el editor para visualizar el radio de interacción.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }

    /// <summary>
    /// Maneja la interacción en 2D con monedas.
    /// </summary>
    /// <param name="other">Collider2D del objeto que entró en contacto</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerUIManager playerUI = other.GetComponent<PlayerUIManager>();
            
            if (CompareTag("Coin") && playerUI != null)
            {
                playerUI.UpdateCoins(1);
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
