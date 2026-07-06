using System.Collections;
using UnityEngine;

// Kẻ địch cảm tử (bomber): đi tuần, phát hiện player thì lao tới; khi tới đủ gần
// sẽ châm ngòi (fuse) rồi tự nổ gây sát thương vùng, sau đó tự chết qua EnemyHealth
// để tái dùng luôn animation chết + rơi coin (không lặp lại logic chết).
[RequireComponent(typeof(EnemyHealth))]
public class BomberEnemyAI : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private EnemyHealth health;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Transform visual;

    [Header("Detection")]
    [SerializeField] private float detectRange = 6f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Patrol")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float checkDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Explosion")]
    [SerializeField] private float fuseDistance = 4f;    // khoảng cách bắt đầu châm ngòi (phải >= bán kính 2 collider cộng lại)
    [SerializeField] private float fuseTime = 0.5f;      // thời gian trước khi nổ
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private int explosionDamage = 3;

    private Transform player;
    private int facingDirection = 1;
    private bool isFusing;

    private static readonly int XSpeed = Animator.StringToHash("xSpeed");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        health = GetComponent<EnemyHealth>();
    }

    private void Update()
    {
        if (isFusing || health.IsDeadYet()) return;

        DetectPlayer();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (isFusing || health.IsDeadYet())
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);

            if (distance <= fuseDistance)
            {
                StartCoroutine(FuseAndExplode());
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

        if (direction != facingDirection)
        {
            Flip();
        }

        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    private void Patrol()
    {
        // Không gán groundCheck/wallCheck thì chỉ đứng yên (tránh NRE).
        if (groundCheck == null || wallCheck == null)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

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
    }

    private IEnumerator FuseAndExplode()
    {
        isFusing = true;
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        // Nhấp nháy cảnh báo trong lúc châm ngòi (dùng trigger hurt của art placeholder).
        if (animator != null)
        {
            animator.SetTrigger("hurt");
        }

        yield return new WaitForSeconds(fuseTime);

        // Nếu đã bị giết trong lúc châm ngòi thì thôi.
        if (health.IsDeadYet()) yield break;

        Explode();
    }

    private void Explode()
    {
        // Sát thương vùng lên tất cả player trong bán kính.
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            explosionRadius,
            playerLayer
        );

        foreach (Collider2D hit in hits)
        {
            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(explosionDamage);
            }
        }

        SoundManager.Instance?.PlaySFX("enemy_death");

        // Tự chịu sát thương chí mạng để tái dùng luồng chết chuẩn (anim + rơi coin + destroy).
        health.TakeDamage(9999);
    }

    private void UpdateAnimator()
    {
        if (animator != null)
        {
            animator.SetFloat(XSpeed, Mathf.Abs(rb.linearVelocity.x));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);

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
