using UnityEngine;

public class RangeEnemyAI : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private RangeEnemyAttack attack;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 1.8f;
    [SerializeField] private Transform visual;
    [SerializeField] private Transform attackRoot;

    [Header("Detect")]
    [SerializeField] private float detectRange = 7f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float keepDistance = 3f;
    [SerializeField] private LayerMask playerLayer;

    private Transform player;
    private int facingDirection = 1;
    private bool canMove = true;

    private static readonly int XSpeed = Animator.StringToHash("xSpeed");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        attack = GetComponent<RangeEnemyAttack>();
    }

    private void Update()
    {
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

        if (player == null)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        FacePlayer();

        if (distance <= attackRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            attack.TryAttack();
            return;
        }

        ChasePlayer();
    }

    private void DetectPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(
            transform.position,
            detectRange,
            playerLayer
        );

        player = hit != null ? hit.transform : null;
    }

    private void ChasePlayer()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    private void FacePlayer()
    {
        float directionToPlayer = Mathf.Sign(player.position.x - transform.position.x);

        if (directionToPlayer > 0 && facingDirection != 1)
            Flip();
        else if (directionToPlayer < 0 && facingDirection != -1)
            Flip();
    }

    private void Flip()
    {
        facingDirection *= -1;

        if (visual != null)
            visual.localScale = new Vector3(facingDirection, 1, 1);

        if (attackRoot != null)
            attackRoot.localScale = new Vector3(facingDirection, 1, 1);
    }

    public int GetFacingDirection()
    {
        return facingDirection;
    }

    public Transform GetPlayer()
    {
        return player;
    }

    public void SetCanMove(bool value)
    {
        canMove = value;

        if (!canMove)
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    private void UpdateAnimator()
    {
        animator.SetFloat(XSpeed, Mathf.Abs(rb.linearVelocity.x));
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}