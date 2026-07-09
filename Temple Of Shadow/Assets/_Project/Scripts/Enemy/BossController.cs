using System.Collections;
using UnityEngine;

// Boss lai (hybrid) 3 phase theo % máu, đọc HP từ EnemyHealth (dùng chung để player
// vẫn đánh trúng như enemy thường):
//   Phase 1 ( > 66% ): cận chiến — đuổi theo và chém.
//   Phase 2 (33–66%): tầm xa — giữ khoảng cách và bắn loạt đạn.
//   Phase 3 ( < 33% ): kết hợp cả hai + nộ (di chuyển/đánh nhanh hơn).
// Mọi đòn đánh do code hẹn giờ (không phụ thuộc animation event / relay).
[RequireComponent(typeof(EnemyHealth))]
public class BossController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private EnemyHealth health;

    [Header("Refs")]
    [SerializeField] private Transform visual;
    [SerializeField] private Transform attackRoot;      // chứa AttackPoint/ShootPoint, lật theo hướng
    [SerializeField] private Transform attackPoint;      // tâm vùng chém cận chiến
    [SerializeField] private Transform shootPoint;       // điểm bắn đạn
    [SerializeField] private GameObject projectilePrefab; // EnemyProjectile (vd ZombieProjectile)

    [Header("Detection")]
    [SerializeField] private float detectRange = 20f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float meleeRange = 5f;      // vào tầm này thì chém (phải >= bán kính 2 collider cộng lại)
    [SerializeField] private float keepDistance = 6f;    // phase tầm xa cố giữ khoảng cách này

    [Header("Melee")]
    [SerializeField] private float meleeRadius = 2.5f;
    [SerializeField] private int meleeDamage = 2;
    [SerializeField] private float meleeCooldown = 1.5f;

    [Header("Ranged")]
    [SerializeField] private float rangedCooldown = 2.5f;
    [SerializeField] private int volleyCount = 3;
    [SerializeField] private float volleyInterval = 0.2f;

    [Header("Enrage (Phase 3)")]
    [SerializeField] private float enrageSpeedMult = 1.5f;
    [SerializeField] private float enrageCooldownMult = 0.5f; // giảm cooldown còn 50%

    private Transform player;
    private int facingDirection = 1;
    private float nextMeleeTime;
    private float nextRangedTime;
    private bool isFiringVolley;

    private static readonly int XSpeed = Animator.StringToHash("xSpeed");
    private static readonly int Attack = Animator.StringToHash("attack");
    private static readonly int ThrowHash = Animator.StringToHash("throw");

    private enum Phase { Melee, Ranged, Enraged }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        health = GetComponent<EnemyHealth>();
    }

    private void Update()
    {
        if (health.IsDeadYet()) return;

        DetectPlayer();
        UpdateAnimator();

        if (player == null) return;

        Phase phase = GetPhase();
        float distance = Vector2.Distance(transform.position, player.position);

        // Tấn công theo phase (di chuyển xử lý ở FixedUpdate).
        switch (phase)
        {
            case Phase.Melee:
                if (distance <= meleeRange) TryMelee(1f);
                break;

            case Phase.Ranged:
                TryRanged(1f);
                break;

            case Phase.Enraged:
                if (distance <= meleeRange) TryMelee(enrageCooldownMult);
                else TryRanged(enrageCooldownMult);
                break;
        }
    }

    private void FixedUpdate()
    {
        if (health.IsDeadYet() || player == null || isFiringVolley)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        FacePlayer();

        Phase phase = GetPhase();
        float distance = Vector2.Distance(transform.position, player.position);
        float speed = phase == Phase.Enraged ? moveSpeed * enrageSpeedMult : moveSpeed;
        float dirToPlayer = Mathf.Sign(player.position.x - transform.position.x);

        switch (phase)
        {
            case Phase.Melee:
                // Đuổi tới khi vào tầm chém.
                if (distance > meleeRange)
                    rb.linearVelocity = new Vector2(dirToPlayer * speed, rb.linearVelocity.y);
                else
                    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                break;

            case Phase.Ranged:
                // Giữ khoảng cách: quá gần thì lùi, quá xa thì tiến, vừa thì đứng bắn.
                if (distance < keepDistance - 0.5f)
                    rb.linearVelocity = new Vector2(-dirToPlayer * speed, rb.linearVelocity.y);
                else if (distance > keepDistance + 0.5f)
                    rb.linearVelocity = new Vector2(dirToPlayer * speed, rb.linearVelocity.y);
                else
                    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                break;

            case Phase.Enraged:
                // Áp sát để đổi qua lại giữa chém và bắn.
                if (distance > meleeRange)
                    rb.linearVelocity = new Vector2(dirToPlayer * speed, rb.linearVelocity.y);
                else
                    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                break;
        }
    }

    private Phase GetPhase()
    {
        float pct = (float)health.GetCurrentHp() / health.GetMaxHp();

        if (pct > 0.66f) return Phase.Melee;
        if (pct > 0.33f) return Phase.Ranged;
        return Phase.Enraged;
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

    private void FacePlayer()
    {
        int dir = player.position.x >= transform.position.x ? 1 : -1;
        if (dir == facingDirection) return;

        facingDirection = dir;

        if (visual != null)
            visual.localScale = new Vector3(facingDirection, 1, 1);

        if (attackRoot != null)
            attackRoot.localScale = new Vector3(facingDirection, 1, 1);
    }

    private void TryMelee(float cooldownMult)
    {
        if (Time.time < nextMeleeTime) return;
        nextMeleeTime = Time.time + meleeCooldown * cooldownMult;

        if (animator != null) animator.SetTrigger(Attack); // chỉ để hiển thị

        Vector3 center = attackPoint != null ? attackPoint.position : transform.position;

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, meleeRadius, playerLayer);
        foreach (Collider2D hit in hits)
        {
            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(meleeDamage);
        }

        SoundManager.Instance?.PlaySFX("enemy_attack");
    }

    private void TryRanged(float cooldownMult)
    {
        if (Time.time < nextRangedTime) return;
        if (projectilePrefab == null) return;

        nextRangedTime = Time.time + rangedCooldown * cooldownMult;
        StartCoroutine(FireVolley());
    }

    private IEnumerator FireVolley()
    {
        isFiringVolley = true;
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        if (animator != null) animator.SetTrigger(ThrowHash); // chỉ để hiển thị

        for (int i = 0; i < volleyCount; i++)
        {
            if (health.IsDeadYet()) break;

            Vector3 spawn = shootPoint != null ? shootPoint.position : transform.position;
            GameObject projectile = Instantiate(projectilePrefab, spawn, Quaternion.identity);

            EnemyProjectile enemyProjectile = projectile.GetComponent<EnemyProjectile>();
            if (enemyProjectile != null)
                enemyProjectile.Setup(facingDirection);

            SoundManager.Instance?.PlaySFX("enemy_attack");

            yield return new WaitForSeconds(volleyInterval);
        }

        isFiringVolley = false;
    }

    private void UpdateAnimator()
    {
        if (animator != null)
            animator.SetFloat(XSpeed, Mathf.Abs(rb.linearVelocity.x));
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, keepDistance);
        Gizmos.color = Color.red;

        Vector3 center = attackPoint != null ? attackPoint.position : transform.position;
        Gizmos.DrawWireSphere(center, meleeRadius);
    }
}
