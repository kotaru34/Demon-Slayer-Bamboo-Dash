using UnityEngine;

public class Collectible : MonoBehaviour
{
  public int value = 1;

  private void OnTriggerEnter2D(Collider2D other)
  {
    if (!other.CompareTag("Player")) return;

    var gm = GameManager.Instance != null ? GameManager.Instance : FindObjectOfType<GameManager>();
    if (gm != null) gm.UpdateScore(value);

    if (CollectibleSpawner.Instance != null)
      CollectibleSpawner.Instance.NotifyCollected();

    Destroy(gameObject);
  }
}
