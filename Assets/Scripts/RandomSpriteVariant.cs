using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RandomSpriteVariant : MonoBehaviour
{
  public Sprite[] variants;

  public bool randomizeOnAwake = true;

  private SpriteRenderer sr;

  private void Awake()
  {
    sr = GetComponent<SpriteRenderer>();
    if (randomizeOnAwake) Randomize();
  }

  public void Randomize()
  {
    if (sr == null) sr = GetComponent<SpriteRenderer>();
    if (variants == null || variants.Length == 0) return;

    sr.sprite = variants[Random.Range(0, variants.Length)];
  }
}
