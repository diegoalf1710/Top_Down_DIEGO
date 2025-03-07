using UnityEngine;
using Photon.Pun;

public class CoinCollector : MonoBehaviourPun
{
    private PlayerUIManager uiManager;

    private void Start()
    {
        if (photonView.IsMine)
        {
            uiManager = GetComponent<PlayerUIManager>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!photonView.IsMine) return;

        if (other.CompareTag("Coin"))
        {
            uiManager.UpdateCoins(1);
            PhotonNetwork.Destroy(other.gameObject);
        }
    }
}
