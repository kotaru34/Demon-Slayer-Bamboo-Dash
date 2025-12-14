using UnityEngine;

public class LevelGenerator2D : MonoBehaviour
{
  public Transform player;
  public Camera mainCamera;

  public Vector2 playAreaSize = new Vector2(50f, 50f);

  public GameObject[] decorPrefabs;
  public GameObject[] obstaclePrefabs;
  public GameObject[] enemyPrefabs;

  public int decorCount = 150;
  public int obstacleCount = 25;
  public int enemyCount = 10;
  public Vector2 playerSpawnHint = Vector2.zero;
  public float playerSafeRadius = 4f;
  public float spawnCheckRadius = 0.45f;
  public int maxAttemptsPerObject = 40;

  public LayerMask blockingMask;

  public Transform decorParent;
  public Transform obstaclesParent;
  public Transform enemiesParent;

  public bool disableDecorColliders = true;
  public bool forceObstaclesStatic = true;
  public bool autoBindCameraToPlayer = true;

  private void Awake()
  {
    if (player == null)
    {
      GameObject p = GameObject.FindGameObjectWithTag("Player");
      if (p != null) player = p.transform;
    }

    if (mainCamera == null) mainCamera = Camera.main;

    EnsureParents();
  }

  private void Start()
  {
    GenerateLevel();
  }

  public void GenerateLevel()
  {
    SpawnBatch(obstaclePrefabs, obstacleCount, obstaclesParent, avoidBlocking: true, avoidPlayerRadius: playerSafeRadius);
    SpawnBatch(enemyPrefabs, enemyCount, enemiesParent, avoidBlocking: true, avoidPlayerRadius: playerSafeRadius);
    SpawnBatch(decorPrefabs, decorCount, decorParent, avoidBlocking: true, avoidPlayerRadius: 0f);

    if (player != null)
    {
      Vector3 safePos = FindFreePosition(playerSpawnHint, radius: 6f, attempts: 120, checkR: spawnCheckRadius, mask: blockingMask);
      player.position = new Vector3(safePos.x, safePos.y, 0f);
    }

    if (autoBindCameraToPlayer && mainCamera != null && player != null)
    {
      var follow = mainCamera.GetComponent<CameraFollowClamp2D>();
      if (follow != null) follow.target = player;

      if (Mathf.Approximately(mainCamera.transform.position.z, 0f))
      {
        var cp = mainCamera.transform.position;
        mainCamera.transform.position = new Vector3(cp.x, cp.y, -10f);
      }
    }
  }

  private void SpawnBatch(GameObject[] prefabs, int count, Transform parent, bool avoidBlocking, float avoidPlayerRadius)
  {
    if (prefabs == null || prefabs.Length == 0 || count <= 0) return;

    for (int i = 0; i < count; i++)
    {
      bool spawned = false;

      for (int attempt = 0; attempt < maxAttemptsPerObject; attempt++)
      {
        Vector3 pos = RandomPositionInArea();

        Vector2 playerCenter = (player != null) ? (Vector2)player.position : playerSpawnHint;
        if (avoidPlayerRadius > 0f && Vector2.Distance((Vector2)pos, playerCenter) < avoidPlayerRadius)
          continue;

        if (avoidBlocking)
        {
          if (Physics2D.OverlapCircle(pos, spawnCheckRadius, blockingMask) != null)
            continue;
        }

        GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
        GameObject obj = Instantiate(prefab, pos, Quaternion.identity, parent);

        var variant = obj.GetComponent<RandomSpriteVariant>();
        if (variant != null) variant.Randomize();

        if (disableDecorColliders && IsOneOf(prefabs, decorPrefabs))
        {
            DisableAllColliders(obj);
        }

        if (forceObstaclesStatic && IsOneOf(prefabs, obstaclePrefabs))
        {
            ForceStaticObstacle(obj);
        }

        spawned = true;
        break;
      }

      if (!spawned) { }
    }
  }

  private Vector3 FindFreePosition(Vector2 center, float radius, int attempts, float checkR, LayerMask mask)
  {
    for (int i = 0; i < attempts; i++)
    {
      Vector2 p = center + Random.insideUnitCircle * radius;
      if (Physics2D.OverlapCircle(p, checkR, mask) == null)
          return new Vector3(p.x, p.y, 0f);
    }
    return new Vector3(center.x, center.y, 0f);
  }

  private Vector3 RandomPositionInArea()
  {
    float x = Random.Range(-playAreaSize.x / 2f, playAreaSize.x / 2f);
    float y = Random.Range(-playAreaSize.y / 2f, playAreaSize.y / 2f);
    return new Vector3(x, y, 0f);
  }

  private void EnsureParents()
  {
    if (decorParent == null) decorParent = CreateOrFind("Decor");
    if (obstaclesParent == null) obstaclesParent = CreateOrFind("Obstacles");
    if (enemiesParent == null) enemiesParent = CreateOrFind("Enemies");
  }

  private Transform CreateOrFind(string name)
  {
    Transform t = transform.Find(name);
    if (t != null) return t;

    GameObject go = new GameObject(name);
    go.transform.SetParent(transform);
    go.transform.localPosition = Vector3.zero;
    return go.transform;
  }

  private void DisableAllColliders(GameObject obj)
  {
    var cols = obj.GetComponentsInChildren<Collider2D>(true);
    foreach (var c in cols) c.enabled = false;
  }

  private void ForceStaticObstacle(GameObject obj)
  {
    var rb = obj.GetComponent<Rigidbody2D>();
    if (rb != null) rb.bodyType = RigidbodyType2D.Static;
  }

  private bool IsOneOf(GameObject[] pickedFrom, GameObject[] group)
  {
    return pickedFrom == group;
  }
}