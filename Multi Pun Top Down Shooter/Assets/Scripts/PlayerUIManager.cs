using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerUIManager : MonoBehaviourPun
{
    public GameObject healthTextObject;
    public  GameObject coinsTextObject;
    
    public TextMeshProUGUI healthText;
    public  TextMeshProUGUI coinsText;
    public int coins = 0;

    private void Awake()
    {
        // Obtener los componentes TextMeshProUGUI de los GameObjects
        if (healthTextObject != null)
            healthText = healthTextObject.GetComponent<TextMeshProUGUI>();
        
        if (coinsTextObject != null)
            coinsText = coinsTextObject.GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            UpdateHealth(100); // Inicializar con vida completa
            UpdateCoins(0);    // Inicializar monedas en 0
        }
    }

    public void UpdateHealth(int currentHealth)
    {
        if (photonView.IsMine && healthText != null)
        {
            healthText.text = "HP: " + currentHealth.ToString();
        }
    }

    public void UpdateCoins(int amount)
    {
        if (photonView.IsMine && coinsText != null)
        {
            coins += amount * 10; // Multiplicar por 10 cada moneda recogida
            coinsText.text = "Coins: " + coins.ToString();
        }
    }
}
