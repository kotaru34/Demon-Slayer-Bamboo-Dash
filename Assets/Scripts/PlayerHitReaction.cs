using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHitReaction : MonoBehaviour
{
  public float knockbackSpeed = 10f;
  public float knockbackTime = 0.12f;
  public float stunTime = 0.15f;
  public float invulTime = 0.8f;

  public bool IsKnockback => knockTimer > 0f;
  public bool IsStunned => stunTimer > 0f;

  Rigidbody2D rb;
  float knockTimer = 0f;
  float stunTimer = 0f;
  float invulTimer = 0f;
  Vector2 knockVel;

  void Awake()
  {
    rb = GetComponent<Rigidbody2D>();
  }

  void Update()
  {
    if (invulTimer > 0f) invulTimer -= Time.deltaTime;
    if (stunTimer > 0f) stunTimer -= Time.deltaTime;
  }

  void FixedUpdate()
  {
    if (knockTimer > 0f)
    {
      knockTimer -= Time.fixedDeltaTime;
      rb.velocity = knockVel;
      if (knockTimer <= 0f) rb.velocity = Vector2.zero;
    }
  }

  public void TakeHit(Vector2 fromPosition, int damage)
  {
    if (invulTimer > 0f) return;

    invulTimer = invulTime;
    stunTimer = stunTime;

    var gm = FindObjectOfType<GameManager>();
    if (gm != null) gm.TakeDamage(damage);

    Vector2 dir = ((Vector2)transform.position - fromPosition).normalized;
    if (dir == Vector2.zero) dir = Vector2.up;

    knockVel = dir * knockbackSpeed;
    knockTimer = knockbackTime;
  }
}
