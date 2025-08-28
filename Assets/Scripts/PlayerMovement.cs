using UnityEngine;
using Unity.Netcode;
public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    private Rigidbody rb;
    public PlayerInputsManager inputsManager;
    [Header("Movement Values")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float jumpForce = 5f;
    private Vector2 currentMove;
    [Header("Raycast Ground Check")]
    [SerializeField] private bool isJumping = false;
    [SerializeField] private bool isGrounded;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.2f;
    [SerializeField] private LayerMask layer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        inputsManager = GetComponent<PlayerInputsManager>();
    }
    private void OnEnable()
    {
        if (inputsManager != null)
        {
            inputsManager.moveInput += OnMove;
            inputsManager.jumpInput += OnJump;
        }
    }

    private void OnDisable()
    {
        if (inputsManager != null)
        {
            inputsManager.moveInput -= OnMove;
            inputsManager.jumpInput -= OnJump;
        }
    }
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
            cam.SetTarget(transform);
        }
    }
    private void OnMove(Vector2 move)
    {
        currentMove = move;
    }

    private void OnJump(bool jump)
    {
        isJumping = jump;
    }

    void Update()
    {
        if (!IsOwner) return;

        CheckGround();
    }
    private void FixedUpdate()
    {
        if (!IsOwner) return;

        MoveFixed();

        JumpFixed();
    }
    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, layer);
    }

    private void MoveFixed()
    {
        Vector3 move = new Vector3(currentMove.x, 0, currentMove.y) * speed;
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
    }

    private void JumpFixed()
    {
        if (isGrounded && isJumping)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = false;
        }
    }
}