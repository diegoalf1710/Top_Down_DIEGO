using UnityEngine;
using Photon.Pun;

public class InteractiveItem : MonoBehaviourPun
{
    [Header("Item Settings")]
    public string itemName = "Default Item";
    public float interactionRadius = 2f;
    public bool oneTimeUse = true;
    
    [Header("Visual Feedback")]
    public bool rotateItem = true;
    public float rotationSpeed = 50f;

    [Header("Interaction Settings")]
    public string playerTag = "Player";
    public bool requireKeyPress = true;
    public KeyCode interactionKey = KeyCode.E;
    
    protected bool playerInRange = false;
    protected GameObject playerObject = null;

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

    public virtual void OnInteract(GameObject player)
    {
        Debug.Log($"Player interacted with {itemName}");
        if (oneTimeUse)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
            playerObject = other.gameObject;
            OnPlayerEnterRange(other.gameObject);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            playerObject = null;
            OnPlayerExitRange(other.gameObject);
        }
    }

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

    protected virtual void OnPlayerExitRange(GameObject player)
    {
        HideInteractionPrompt();
    }

    protected virtual void ShowInteractionPrompt()
    {
        Debug.Log($"Presiona {interactionKey} para interactuar con {itemName}");
        // Aquí puedes mostrar un UI prompt
    }

    protected virtual void HideInteractionPrompt()
    {
        // Aquí puedes ocultar el UI prompt
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }

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
