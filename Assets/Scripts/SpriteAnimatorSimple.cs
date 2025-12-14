using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimatorSimple : MonoBehaviour
{
  public Sprite[] frames;
  public float fps = 8f;
  public bool loop = true;
  public bool randomStartFrame = true;

  SpriteRenderer sr;
  int index = 0;
  float timer = 0f;

  void Awake()
  {
    sr = GetComponent<SpriteRenderer>();
    if (frames != null && frames.Length > 0)
    {
      index = randomStartFrame ? Random.Range(0, frames.Length) : 0;
      sr.sprite = frames[index];
    }
  }

  void Update()
  {
    if (frames == null || frames.Length <= 1) return;

    timer += Time.deltaTime;
    float frameTime = 1f / Mathf.Max(1f, fps);

    while (timer >= frameTime)
    {
      timer -= frameTime;

      index++;
      if (index >= frames.Length)
      {
        if (loop) index = 0;
        else { index = frames.Length - 1; enabled = false; }
      }

      sr.sprite = frames[index];
    }
  }
}
