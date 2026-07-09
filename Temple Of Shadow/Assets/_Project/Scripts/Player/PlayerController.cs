using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private PlayerStats stats;
    private PlayerHealth health;

    [Header("Movement")]
    private float moveSpeed = 3.5f;
    private float jumpForce = 7f;
    [SerializeField] private int baseMaxJumpCount = 2;
    private int maxJumpCount;
    [SerializeField] private Transform visual;
    [SerializeField] private Transform attackRoot;

    private int jumpCount;

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
        
        stats = GetComponent<PlayerStats>();
        health = GetComponent<PlayerHealth>();

        animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        if (stats != null)
        {
            stats.StatsChanged += ApplyStats;
        }

        ApplyStats();
    }

    private void OnDisable()
    {
        if (stats != null)
        {
            stats.StatsChanged -= ApplyStats;
        }
    }

    private void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        CheckGround();
        HandleJump();
        HandleFlip();
        UpdateAnimator();

        if (Input.GetKeyDown(KeyCode.R))
        {
            LogCurrentStats();
        }
    
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


        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && jumpCount < maxJumpCount)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );

            jumpCount++;
            SoundManager.Instance?.PlaySFX("jump");
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
        bool wasGrounded = isGrounded;

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        if (!wasGrounded && isGrounded)
        {
            jumpCount = 0;
        }
    }

    private void ApplyStats()
    {
        if (stats == null)
        {
            return;
        }

        moveSpeed = stats.MoveSpeed;
        jumpForce = stats.JumpForce;
        maxJumpCount = baseMaxJumpCount + stats.BonusJumpCount;

        if (jumpCount > maxJumpCount)
        {
            jumpCount = maxJumpCount;
        }
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

    private void LogCurrentStats()
    {
        if (stats == null)
        {
            Debug.LogWarning("Cannot log player stats because PlayerStats is missing.");
            return;
        }

        string characterName = stats.CharacterData != null ? stats.CharacterData.characterName : "None";
        int currentHp = health != null ? health.GetCurrentHp() : 0;
        int maxHp = health != null ? health.GetMaxHp() : stats.MaxHP;

        Debug.Log(
            "Current Player Stats\n" +
            $"Character: {characterName}\n" +
            $"HP: {currentHp}/{maxHp}\n" +
            $"MaxHP: {stats.MaxHP}\n" +
            $"Damage: {stats.Damage}\n" +
            $"Armor: {stats.Armor}\n" +
            $"MoveSpeed: {stats.MoveSpeed}\n" +
            $"AttackSpeed: {stats.AttackSpeed}\n" +
            $"AttackInterval: {stats.AttackInterval}\n" +
            $"JumpForce: {stats.JumpForce}\n" +
            $"BonusJumpCount: {stats.BonusJumpCount}"
        );
    }
}
