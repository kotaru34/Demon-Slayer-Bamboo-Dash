using UnityEngine;

[ExecuteAlways]
public class WorldBoundsFromSprite : MonoBehaviour
{
  public SpriteRenderer background;
  public float wallThickness = 1f;

  public string wallsLayerName = "Obstacles";

  public Rect WorldRect { get; private set; }

  private Transform wallsRoot;

  void OnEnable()
  {
    if (background == null) background = GetComponent<SpriteRenderer>();
    Build();
  }

  void OnValidate()
  {
    if (background == null) background = GetComponent<SpriteRenderer>();
    Build();
  }

  public void Build()
  {
    if (background == null || background.sprite == null) return;

    Bounds b = background.bounds;
    WorldRect = Rect.MinMaxRect(b.min.x, b.min.y, b.max.x, b.max.y);

    if (wallsRoot == null)
    {
      var existing = transform.Find("WorldWalls");
      wallsRoot = existing != null ? existing : new GameObject("WorldWalls").transform;
      wallsRoot.SetParent(transform, false);
    }

    int wallLayer = LayerMask.NameToLayer(wallsLayerName);
    if (wallLayer == -1) wallLayer = 0;

    CreateOrUpdateWall("Left", new Vector2(WorldRect.xMin - wallThickness * 0.5f, WorldRect.center.y),
      new Vector2(wallThickness, WorldRect.height + wallThickness * 2f), wallLayer);
    CreateOrUpdateWall("Right", new Vector2(WorldRect.xMax + wallThickness * 0.5f, WorldRect.center.y),
      new Vector2(wallThickness, WorldRect.height + wallThickness * 2f), wallLayer);
    CreateOrUpdateWall("Bottom",new Vector2(WorldRect.center.x, WorldRect.yMin - wallThickness * 0.5f),
      new Vector2(WorldRect.width + wallThickness * 2f, wallThickness), wallLayer);
    CreateOrUpdateWall("Top", new Vector2(WorldRect.center.x, WorldRect.yMax + wallThickness * 0.5f),
      new Vector2(WorldRect.width + wallThickness * 2f, wallThickness), wallLayer);
  }

  void CreateOrUpdateWall(string name, Vector2 pos, Vector2 size, int layer)
  {
    Transform t = wallsRoot.Find(name);
    if (t == null)
    {
      var go = new GameObject(name);
      go.transform.SetParent(wallsRoot, false);
      t = go.transform;
      go.AddComponent<BoxCollider2D>();
    }

    t.gameObject.layer = layer;
    t.position = new Vector3(pos.x, pos.y, 0f);

    var col = t.GetComponent<BoxCollider2D>();
    col.isTrigger = false;
    col.size = size;
    col.offset = Vector2.zero;
  }
}
