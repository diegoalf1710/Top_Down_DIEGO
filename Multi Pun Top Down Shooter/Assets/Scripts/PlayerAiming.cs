using UnityEngine;
using Photon.Pun;

public class PlayerAiming : MonoBehaviourPun
{
    void Update()
    {
        // Verifica si el jugador local es el dueño del PhotonView
        if (photonView.IsMine)
        {
            // Obtener la posición del cursor en el mundo
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Lanzar un rayo desde la cámara hacia la posición del cursor
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Calcular la dirección desde el jugador hacia el punto de impacto del rayo
                Vector3 direction = hit.point - transform.position;

                // Ignorar la componente Y para evitar rotaciones no deseadas en el eje vertical
                direction.y = 0;

                // Rotar el jugador hacia la dirección del cursor
                if (direction != Vector3.zero)
                {
                    // Calcular la rotación objetivo usando la dirección
                    Quaternion targetRotation = Quaternion.LookRotation(direction);

                    // Suavizar la rotación del jugador hacia la dirección del cursor
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
                }
            }
        }
    }
}