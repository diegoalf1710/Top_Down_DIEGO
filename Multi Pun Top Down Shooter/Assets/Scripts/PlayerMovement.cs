using UnityEngine;
using Photon.Pun;

// Clase que maneja el movimiento del jugador en una red multijugador usando Photon
// Controla el movimiento básico, la detección del suelo y las físicas del personaje
public class PlayerMovement : MonoBehaviourPun
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float groundCheckDistance = 0.4f;
    public LayerMask groundLayer;
    public float maxVerticalSpeed = 10f; // Límite de velocidad vertical
    public float fallMultiplier = 2.5f;  // Multiplicador de caída

    [Header("Movement Smoothing")]
    public float movementSmoothing = 0.05f;

    private Rigidbody rb;
    private Vector3 currentVelocity;
    private bool isGrounded;

    // Inicializa los componentes necesarios y configura el Rigidbody
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = true;
    }

    // Se ejecuta a intervalos fijos para manejar las físicas del movimiento
    // Solo se ejecuta en la instancia local del jugador
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        CheckGrounded();
        HandleMovement();
        LimitVerticalSpeed();
    }

    // Verifica si el jugador está tocando el suelo usando un raycast
    void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    // Procesa el movimiento del jugador basado en el input horizontal y vertical
    // Aplica suavizado al movimiento y una gravedad aumentada al caer
    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 targetVelocity = new Vector3(moveX, 0, moveZ).normalized * speed;
        targetVelocity.y = rb.velocity.y;

        if (!isGrounded)
        {
            // Aplicar mayor gravedad al caer
            rb.AddForce(Vector3.down * fallMultiplier, ForceMode.Acceleration);
        }

        rb.velocity = Vector3.SmoothDamp(
            rb.velocity,
            targetVelocity,
            ref currentVelocity,
            isGrounded ? movementSmoothing : movementSmoothing * 2
        );
    }

    // Limita la velocidad vertical del jugador para evitar caídas o saltos excesivamente rápidos
    void LimitVerticalSpeed()
    {
        Vector3 velocity = rb.velocity;
        velocity.y = Mathf.Clamp(velocity.y, -maxVerticalSpeed, maxVerticalSpeed);
        rb.velocity = velocity;
    }

    // Dibuja una línea de debug que muestra el raycast de detección del suelo
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}