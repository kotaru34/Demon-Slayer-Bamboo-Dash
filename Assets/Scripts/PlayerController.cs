using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float invulnerabilityDuration = 1f;

    private Rigidbody2D rb;
    private Vector2 input;
    private Rect movementBounds;
    private float invulnerableUntil;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        var collider = GetComponent<BoxCollider2D>();
        collider.isTrigger = false;
    }

    private void Start()
    {
        GameManager.Instance.RegisterPlayer(this);
    }

    public void SetBounds(Rect bounds)
    {
        movementBounds = bounds;
    }

    private void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input = input.normalized;
    }

    private void FixedUpdate()
    {
        Vector2 target = rb.position + input * moveSpeed * Time.fixedDeltaTime;
        target.x = Mathf.Clamp(target.x, movementBounds.xMin, movementBounds.xMax);
        target.y = Mathf.Clamp(target.y, movementBounds.yMin, movementBounds.yMax);
        rb.MovePosition(target);
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    public bool CanTakeDamage()
    {
        return Time.time >= invulnerableUntil;
    }

    public void MarkHit()
    {
        invulnerableUntil = Time.time + invulnerabilityDuration;
    }
}