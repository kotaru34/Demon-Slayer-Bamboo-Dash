using UnityEngine;

public class EnemyMeleeAttack2D : MonoBehaviour
{
  public int damage = 1;

  [Header("Attack")]
  public float attackRange = 1.1f;
  public float attackCooldown = 0.9f;

  [Header("Line of sight")]
  public LayerMask obstaclesMask;
  public float losSkin = 0.06f; // to avoid start inside collider

  Transform player;
  float nextAttackTime = 0f;

  void Start()
  {
    FindPlayer();
  }

  void FixedUpdate()
  {
    if (player == null) { FindPlayer(); return; }

    Vector2 a = transform.position;
    Vector2 b = player.position;

    float dist = Vector2.Distance(a, b);
    if (dist > attackRange) return;
    if (Time.time < nextAttackTime) return;

    if (HasWallBetween(a, b)) return;

    var hit = player.GetComponent<PlayerHitReaction>();
    if (hit != null)
    {
      hit.TakeHit(a, damage);
      nextAttackTime = Time.time + attackCooldown;
    }
  }

  void FindPlayer()
  {
    var p = GameObject.FindGameObjectWithTag("Player");
    if (p != null) player = p.transform;
  }

  bool HasWallBetween(Vector2 a, Vector2 b)
  {
    Vector2 dir = (b - a);
    float dist = dir.magnitude;
    if (dist <= 0.001f) return false;
    dir /= dist;

    // raycasting
    var hit1 = Physics2D.Raycast(a + dir * losSkin, dir, dist - 2f * losSkin, obstaclesMask);
    if (hit1.collider != null) return true;

    var hit2 = Physics2D.Raycast(b - dir * losSkin, -dir, dist - 2f * losSkin, obstaclesMask);
    return hit2.collider != null;
  }
}
