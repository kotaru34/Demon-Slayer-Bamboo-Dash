using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private float patrolSpeed = 1.5f;
    [SerializeField] private float patrolDistance = 3f;
    [SerializeField] private float knockbackForce = 6f;

    private Vector2 origin;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void Start()
    {
        origin = rb.position;
    }

    private void FixedUpdate()
    {
        float offset = Mathf.Sin(Time.time * patrolSpeed) * patrolDistance;
        Vector2 position = origin + new Vector2(offset, 0f);
        rb.MovePosition(position);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var player = collision.collider.GetComponent<PlayerController>();
        if (player == null)
        {
            return;
        }

        if (!player.CanTakeDamage())
        {
            return;
        }

        Vector2 direction = (player.transform.position - transform.position).normalized;
        player.ApplyKnockback(direction, knockbackForce);
        player.MarkHit();
        GameManager.Instance.ApplyDamage(1);
    }
}