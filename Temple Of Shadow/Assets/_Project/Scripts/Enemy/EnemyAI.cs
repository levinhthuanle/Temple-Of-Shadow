using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private EnemyAttack enemyAttack;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform visual;

    [Header("Detection")]
    [SerializeField] private float detectRange = 5f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private Transform attackRoot;
    [SerializeField] private LayerMask playerLayer;

    [Header("Patrol")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float checkDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Transform player;
    private int facingDirection = 1;
    private bool canMove = true;
    public bool CanMove => canMove;

    private static readonly int XSpeed = Animator.StringToHash("xSpeed");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        enemyAttack = GetComponent<EnemyAttack>();
    }

    private void Update()
    {
        if (!canMove) return; // Không tiếp tục detect hoặc lật người/cập nhật target nếu đang bị thương

        DetectPlayer();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                enemyAttack.TryAttack();
                return;
            }

            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void DetectPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(
            transform.position,
            detectRange,
            playerLayer
        );

        if (playerCollider != null)
        {
            player = playerCollider.transform;
        }
        else
        {
            player = null;
        }
    }

    private void ChasePlayer()
    {
        float directionToPlayer = Mathf.Sign(player.position.x - transform.position.x);

        if (directionToPlayer != facingDirection)
        {
            Flip();
        }

        rb.linearVelocity = new Vector2(directionToPlayer * moveSpeed, rb.linearVelocity.y);
    }

    private void Patrol()
    {
        bool hasGroundAhead = Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            checkDistance,
            groundLayer
        );

        bool hasWallAhead = Physics2D.Raycast(
            wallCheck.position,
            Vector2.right * facingDirection,
            checkDistance,
            groundLayer
        );

        if (!hasGroundAhead || hasWallAhead)
        {
            Flip();
        }

        rb.linearVelocity = new Vector2(facingDirection * moveSpeed, rb.linearVelocity.y);
    }

    private void Flip()
    {
        facingDirection *= -1;

        if (visual != null)
        {
            visual.localScale = new Vector3(facingDirection, 1, 1);
        }

        if (attackRoot != null)
        {
            attackRoot.localScale = new Vector3(facingDirection, 1, 1);
        }
    }

    private void UpdateAnimator()
    {
        animator.SetFloat(XSpeed, Mathf.Abs(rb.linearVelocity.x));
    }

    public void SetCanMove(bool value)
    {
        canMove = value;

        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    public int GetFacingDirection()
    {
        return facingDirection;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (groundCheck != null)
        {
            Gizmos.DrawRay(groundCheck.position, Vector2.down * checkDistance);
        }

        if (wallCheck != null)
        {
            Gizmos.DrawRay(wallCheck.position, Vector2.right * facingDirection * checkDistance);
        }
    }
}