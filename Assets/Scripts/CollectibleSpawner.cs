using System.Collections;
using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
  public static CollectibleSpawner Instance { get; private set; }

  [Header("What to spawn")]
  public GameObject[] collectiblePrefabs;
  public int desiredCount = 15;
  public float respawnDelay = 0.5f;

  [Header("Where to spawn")]
  public WorldBoundsFromSprite boundsSource;
  public LayerMask blockingMask;
  public float checkRadius = 0.35f;
  public float edgePadding = 0.6f;

  [Header("Avoid player")]
  public Transform player;
  public float minDistanceFromPlayer = 2.0f;

  [Header("Parent")]
  public Transform parentForSpawned;

  void Awake()
  {
      Instance = this;
  }

  void Start()
  {
    if (player == null)
    {
      var p = GameObject.FindGameObjectWithTag("Player");
      if (p != null) player = p.transform;
    }

    EnsureCount();
  }

  public void NotifyCollected()
  {
    StartCoroutine(RespawnAfterDelay());
  }

  IEnumerator RespawnAfterDelay()
  {
    // fix to make it work always
    yield return new WaitForSecondsRealtime(respawnDelay);
    EnsureCount();
  }

  void EnsureCount()
  {
    if (collectiblePrefabs == null || collectiblePrefabs.Length == 0) return;
    if (boundsSource == null) return;

    int current = FindObjectsOfType<Collectible>(false).Length;
    int need = Mathf.Max(0, desiredCount - current);

    for (int i = 0; i < need; i++)
      SpawnOne();
  }

  void SpawnOne()
  {
    Rect r = boundsSource.WorldRect;

    for (int attempt = 0; attempt < 80; attempt++)
    {
      float x = Random.Range(r.xMin + edgePadding, r.xMax - edgePadding);
      float y = Random.Range(r.yMin + edgePadding, r.yMax - edgePadding);
      Vector3 pos = new Vector3(x, y, 0f);

      if (player != null && Vector2.Distance(pos, player.position) < minDistanceFromPlayer)
        continue;

      if (Physics2D.OverlapCircle(pos, checkRadius, blockingMask) != null)
        continue;

      var prefab = collectiblePrefabs[Random.Range(0, collectiblePrefabs.Length)];
      var obj = Instantiate(prefab, pos, Quaternion.identity, parentForSpawned);

      var variant = obj.GetComponent<RandomSpriteVariant>();
      if (variant != null) variant.Randomize();

      return;
    }

    // test runtime
    Debug.LogWarning("CollectibleSpawner: couldn't find place to spawn.");
  }
}
