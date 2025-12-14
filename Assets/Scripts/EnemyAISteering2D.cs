using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyAISteering2D : MonoBehaviour
{
    [Header("Speeds")]
    public float patrolSpeed = 2f;
    public float chaseSpeed  = 3.5f;

    [Header("Patrol")]
    public float patrolRadius = 6f;
    public float repickTime   = 2.5f;
    public float waypointTolerance = 0.4f;

    [Header("Detection")]
    public float detectionRadius = 6f;
    public float forgetAfter     = 1.5f;

    [Header("Steering / Avoidance")]
    public LayerMask obstacleMask;
    public float lookAhead = 1.2f;
    public float radiusInflate = 0.02f; // range to the walls
    public int samples = 11;            // number of directions to search (odd: 9/11/13)
    public float maxAngle = 100f;       // degrees to the left/right of the desired direction
    public float turnLerp = 0.25f;      // smoothness of the turn

    [Header("Anti-stuck")]
    public float stuckCheckTime = 0.4f;
    public float stuckMoveEps   = 0.04f;

    Rigidbody2D rb;
    Collider2D col;
    Transform player;

    Vector2 spawnPoint;
    Vector2 patrolTarget;
    float nextPickTime;

    float lastSeenTime = -999f;

    // steering state
    Vector2 moveDir = Vector2.right;
    float sideBias = 1f; // 1 = right / -1 = left

    // stuck detection
    Vector2 lastPos;
    float stuckTimer = 0f;
    float stuckWindow = 0f;

    void Awake()
    {
      rb = GetComponent<Rigidbody2D>();
      col = GetComponent<Collider2D>();

      rb.gravityScale = 0f;
      rb.constraints = RigidbodyConstraints2D.FreezeRotation;
      rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
      rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Start()
    {
      spawnPoint = rb.position;
      PickPatrolTarget();

      var p = GameObject.FindGameObjectWithTag("Player");
      if (p != null) player = p.transform;

      lastPos = rb.position;
    }

    void FixedUpdate()
  {
    if (player == null)
    {
      var p = GameObject.FindGameObjectWithTag("Player");
      if (p != null) player = p.transform;
    }

    bool chasing = false;
    Vector2 desired;

    if (player != null)
    {
      float dist = Vector2.Distance(rb.position, player.position);
      if (dist <= detectionRadius) lastSeenTime = Time.time;
      chasing = (Time.time - lastSeenTime) <= forgetAfter;

      desired = ((Vector2)player.position - rb.position).normalized;
    }
    else
    {
      chasing = false;
      desired = Vector2.zero;
    }

    if (!chasing)
    {
      if (Time.time >= nextPickTime || Vector2.Distance(rb.position, patrolTarget) <= waypointTolerance)
        PickPatrolTarget();

      desired = (patrolTarget - rb.position).normalized;
    }

    float speed = chasing ? chaseSpeed : patrolSpeed;

    Vector2 pos = rb.position;
    stuckWindow += Time.fixedDeltaTime;

    if (stuckWindow >= stuckCheckTime)
    {
      float moved = Vector2.Distance(pos, lastPos);
      lastPos = pos;
      stuckWindow = 0f;

      bool tryingToMove = desired.sqrMagnitude > 0.01f;
      if (tryingToMove && moved < stuckMoveEps)
      {
        stuckTimer += stuckCheckTime;
        if (stuckTimer >= stuckCheckTime)
        {
          sideBias *= -1f;
          stuckTimer = 0f;

          if (!chasing) PickPatrolTarget();
        }
      }
      else
      {
        stuckTimer = 0f;
      }
    }

    Vector2 steered = ComputeSteeredDirection(desired);

    moveDir = Vector2.Lerp(moveDir, steered, turnLerp);

    rb.velocity = moveDir * speed;
  }

  void PickPatrolTarget()
  {
    patrolTarget = spawnPoint + Random.insideUnitCircle * patrolRadius;
    nextPickTime = Time.time + repickTime;
  }

  Vector2 ComputeSteeredDirection(Vector2 desired)
  {
    if (desired == Vector2.zero)
      return moveDir.sqrMagnitude > 0.001f ? moveDir.normalized : Vector2.right;

    // enemy radius
    float r = 0.25f;
    if (col != null)
    {
      var e = col.bounds.extents;
      r = Mathf.Min(e.x, e.y) + radiusInflate;
    }

    int n = Mathf.Max(3, samples);
    if (n % 2 == 0) n += 1; // odd
    int half = n / 2;

    float bestScore = -999f;
    Vector2 bestDir = desired;

    for (int i = -half; i <= half; i++)
    {
      float t = (half == 0) ? 0f : (i / (float)half);  // -1..1
      float ang = t * maxAngle;

      Vector2 cand = Rotate(desired, ang);

      float clearance = Clearance(rb.position, cand, r); // 0..lookAhead
      float clearance01 = Mathf.Clamp01(clearance / lookAhead);

      // rating: we want to move towards desired, but with good clearance
      float align = Vector2.Dot(cand, desired); // -1..1
      float side = Mathf.Sin(ang * Mathf.Deg2Rad) * sideBias; // good

      float score = align * 1.2f + clearance01 * 1.0f + side * 0.08f;

      // scheisse
      if (clearance < 0.08f) score -= 2.0f;

      if (score > bestScore)
      {
        bestScore = score;
        bestDir = cand;
      }
    }

    // step back if that bad
    if (bestScore < -0.5f)
      return (-desired).normalized;

    return bestDir.normalized;
  }

  float Clearance(Vector2 origin, Vector2 dir, float radius)
  {
    if (dir == Vector2.zero) return 0f;

    var hit = Physics2D.CircleCast(origin, radius, dir, lookAhead, obstacleMask);
    if (hit.collider == null) return lookAhead;
    return hit.distance;
  }

  static Vector2 Rotate(Vector2 v, float degrees)
  {
    float rad = degrees * Mathf.Deg2Rad;
    float cs = Mathf.Cos(rad);
    float sn = Mathf.Sin(rad);
    return new Vector2(v.x * cs - v.y * sn, v.x * sn + v.y * cs);
  }
}
