using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private Transform visual;
    [SerializeField] private Transform attackRoot;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    public float xInput;
    public bool isGrounded;
    public float yVelocity => rb.linearVelocity.y;
    private bool canMove = true;
    public bool CanMove => canMove;

    public float FacingDirection { get; private set; } = 1f;
    public bool IsGrounded => isGrounded;

    private static readonly int XSpeed = Animator.StringToHash("xSpeed");
    private static readonly int YVelocity = Animator.StringToHash("yVelocity");
    private static readonly int IsGroundedHash = Animator.StringToHash("isGrounded");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        CheckGround();
        HandleJump();
        HandleFlip();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (!canMove) return;

        rb.linearVelocity = new Vector2(
            xInput * moveSpeed,
            rb.linearVelocity.y
        );
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void HandleFlip()
    {
        if (xInput > 0)
        {
            FacingDirection = 1f;
            visual.localScale = new Vector3(1, 1, 1);
            attackRoot.localScale = new Vector3(1, 1, 1);
        }
        else if (xInput < 0)
        {
            FacingDirection = -1f;
            visual.localScale = new Vector3(-1, 1, 1);
            attackRoot.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    private void UpdateAnimator()
    {
        animator.SetFloat(XSpeed, Mathf.Abs(rb.linearVelocity.x));
        animator.SetFloat(YVelocity, rb.linearVelocity.y);
        animator.SetBool(IsGroundedHash, isGrounded);
    }

    public void SetCanMove(bool value)
    {
        canMove = value;

        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    private void OnDrawGizmos()
    {
        if (groundCheck == null) return;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }

    public void SetJumpForce(float newJumpForce)
    {
        jumpForce = newJumpForce;
    }
}