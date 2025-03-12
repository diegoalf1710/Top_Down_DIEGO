using UnityEngine;
using Photon.Pun;

// Esta clase maneja la rotación del jugador para que mire hacia donde apunta el cursor del mouse.
// Funciona en conjunto con el sistema multijugador de Photon (PUN2) para asegurar que
// solo el jugador local pueda controlar su propia rotación.
public class PlayerAiming : MonoBehaviourPun
{
    // Update se ejecuta una vez por frame.
    // Realiza el cálculo de la rotación del jugador basándose en la posición del cursor del mouse.
    // Solo funciona si el jugador es el dueño local del objeto (photonView.IsMine).
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