using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemySpriteAnimator : MonoBehaviour
{
  [Header("Frames")]
  public Sprite idleFrame;
  public Sprite[] walkFrames;
  [Header("Settings")]
  public float walkFps = 10f;
  public float moveThreshold = 0.05f;

  SpriteRenderer sr;
  Rigidbody2D rb;

  int walkIndex = 0;
  float timer = 0f;

  void Awake()
  {
    sr = GetComponent<SpriteRenderer>();
    rb = GetComponent<Rigidbody2D>();

    if (idleFrame != null)
      sr.sprite = idleFrame;
  }

  void Update()
  {
    bool moving = rb != null && rb.velocity.sqrMagnitude > (moveThreshold * moveThreshold);

    if (!moving)
    {
      timer = 0f;
      walkIndex = 0;
      if (idleFrame != null) sr.sprite = idleFrame;
      return;
    }

    if (walkFrames == null || walkFrames.Length == 0) return;

    if (rb != null && Mathf.Abs(rb.velocity.x) > 0.01f)
    sr.flipX = rb.velocity.x < 0f;
    
    timer += Time.deltaTime;
    float frameTime = 1f / Mathf.Max(1f, walkFps);

    while (timer >= frameTime)
    {
      timer -= frameTime;
      walkIndex = (walkIndex + 1) % walkFrames.Length;
      sr.sprite = walkFrames[walkIndex];
    }
  }
}