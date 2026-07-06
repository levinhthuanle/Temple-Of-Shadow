using UnityEngine;

// Kẻ địch bay (dơi): bay lượn, phát hiện player trong bán kính rồi lao tới,
// gây sát thương khi chạm. HP/hurt/die dùng chung EnemyHealth như các enemy khác.
// Cần: Rigidbody2D (GravityScale = 0) + một Collider2D IsTrigger để bắt va chạm.
public class FlyingEnemyAI : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 2.5f;   // tốc độ bay bình thường
    [SerializeField] private float diveSpeed = 5f;     // tốc độ khi bổ nhào lúc ở gần
    [SerializeField] private float diveRange = 3f;     // khoảng cách bắt đầu bổ nhào
    [SerializeField] private Transform visual;

    [Header("Detection")]
    [SerializeField] private float detectRange = 6f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Contact Damage")]
    [SerializeField] private int contactDamage = 1;
    [SerializeField] private float contactRange = 1.5f; // bán kính gây damage khi áp sát player

    private Transform player;
    private int facingDirection = 1;

    private static readonly int XSpeed = Animator.StringToHash("xSpeed");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        DetectPlayer();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            // Không có player: đứng yên lơ lửng (GravityScale = 0 nên không rơi).
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 toPlayer = (Vector2)(player.position - transform.position);
        float distance = toPlayer.magnitude;
        float speed = distance <= diveRange ? diveSpeed : moveSpeed;

        rb.linearVelocity = toPlayer.normalized * speed;
        FaceMoveDirection(toPlayer.x);

        TryContactDamage();
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

    private void FaceMoveDirection(float dirX)
    {
        if (Mathf.Abs(dirX) < 0.01f) return;

        int newFacing = dirX > 0 ? 1 : -1;
        if (newFacing == facingDirection) return;

        facingDirection = newFacing;

        if (visual != null)
        {
            visual.localScale = new Vector3(facingDirection, 1, 1);
        }
    }

    private void UpdateAnimator()
    {
        if (animator != null)
        {
            animator.SetFloat(XSpeed, Mathf.Abs(rb.linearVelocity.x));
        }
    }

    // Gây damage bằng OverlapCircle (không phụ thuộc va chạm vật lý / layer matrix,
    // nên vẫn hoạt động sau khi tắt va chạm Enemy↔Player). PlayerHealth đã có i-frame
    // nên không cần cooldown riêng.
    private void TryContactDamage()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, contactRange, playerLayer);
        if (hit == null) return;

        PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(contactDamage);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.DrawWireSphere(transform.position, diveRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, contactRange);
    }
}
