using UnityEngine;
using TMPro;
using Photon.Pun;
using Unity.VisualScripting;

public class PlayerUIManager : MonoBehaviourPun
{
    private TextMeshProUGUI coinsText;
    private TextMeshProUGUI lifeText;
    private int coins = 0;
    private PlayerHealth playerHealth;

    private void Start()
    {
        if (photonView.IsMine)
        {
            // Obtener referencia al PlayerHealth
            playerHealth = GetComponent<PlayerHealth>();
            if (playerHealth == null)
            {
                Debug.LogWarning("No se encontró el componente PlayerHealth");
            }

            // Buscar el texto de monedas por tag
            GameObject coinsTextObj = GameObject.FindGameObjectWithTag("coins_text");
            if (coinsTextObj != null)
            {
                coinsText = coinsTextObj.GetComponent<TextMeshProUGUI>();
                UpdateCoins(0);
            }
            else
            {
                Debug.LogWarning("No se encontró objeto con tag 'coins_text'");
            }

            // Buscar el texto de vida por tag
            GameObject lifeTextObj = GameObject.FindGameObjectWithTag("life_text");
            if (lifeTextObj != null)
            {
                lifeText = lifeTextObj.GetComponent<TextMeshProUGUI>();
                UpdateHealth(playerHealth != null ? playerHealth.currentHealth : 100);
            }
            else
            {
                Debug.LogWarning("No se encontró objeto con tag 'life_text'");
            }
        }
    }

    void Update()
    {
        if (photonView.IsMine && lifeText != null && playerHealth != null)
        {
            lifeText.text = "HP: " + playerHealth.currentHealth.ToString();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine && other.CompareTag("Coin"))
        {
            UpdateCoins(1);
            Destroy(other.gameObject);
        }
    }

    public void UpdateCoins(int amount)
    {
        if (photonView.IsMine && coinsText != null)
        {
            coins += amount * 10;
            coinsText.text = "Coins: " + coins.ToString();
        }
    }

    public void UpdateHealth(int health)
    {
        if (photonView.IsMine && lifeText != null)
        {
            lifeText.text = "HP: " + playerHealth.currentHealth.ToString();
            Debug.Log("Vida actualizada: " + playerHealth.currentHealth.ToString());
        }
    }
}
