using UnityEngine;
using Photon.Pun;

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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = true;
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        CheckGrounded();
        HandleMovement();
        LimitVerticalSpeed();
    }

    void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

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

    void LimitVerticalSpeed()
    {
        Vector3 velocity = rb.velocity;
        velocity.y = Mathf.Clamp(velocity.y, -maxVerticalSpeed, maxVerticalSpeed);
        rb.velocity = velocity;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}