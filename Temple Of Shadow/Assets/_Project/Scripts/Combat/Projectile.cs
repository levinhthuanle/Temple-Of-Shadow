using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private LayerMask enemyLayer;

    private Rigidbody2D rb;
    private float direction = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Setup(float facingDirection)
    {
        direction = facingDirection;

        if (direction < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        rb.linearVelocity = new Vector2(direction * speed, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & enemyLayer) == 0) return;

        EnemyHealth enemy = collision.GetComponent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}