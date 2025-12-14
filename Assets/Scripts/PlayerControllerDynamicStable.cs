using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControllerDynamicStable : MonoBehaviour
{
  public float speed = 7f;

  Rigidbody2D rb;
  Vector2 input;
  PlayerHitReaction hit;

  void Awake()
  {
    rb = GetComponent<Rigidbody2D>();
    hit = GetComponent<PlayerHitReaction>();

    rb.bodyType = RigidbodyType2D.Dynamic;
    rb.gravityScale = 0f;
    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    rb.interpolation = RigidbodyInterpolation2D.Interpolate;
}

  void Update()
  {
    if (hit != null && hit.IsStunned)
    {
      input = Vector2.zero;
      return;
    }

    int x = 0, y = 0;
    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))  x -= 1;
    if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) x += 1;
    if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))  y -= 1;
    if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))    y += 1;

    input = new Vector2(x, y);
    if (input.sqrMagnitude > 1f) input.Normalize();
  }

  void FixedUpdate()
  {
    if (hit != null && hit.IsKnockback) return;

    rb.velocity = input * speed;
  }
}
