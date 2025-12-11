using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private int decorationCount = 80;
    [SerializeField] private int collectibleCount = 10;
    [SerializeField] private Vector2 areaSize = new Vector2(14f, 8f);
    [SerializeField] private Sprite[] decorationSprites;
    [SerializeField] private Sprite[] collectibleFrames;

    public void Configure(Sprite[] decorations, Sprite[] collectibles, Vector2 bounds)
    {
        decorationSprites = decorations;
        collectibleFrames = collectibles;
        areaSize = bounds;
    }

    public void Generate()
    {
        SpawnDecorations();
        SpawnCollectibles();
    }

    private void SpawnDecorations()
    {
        if (decorationSprites == null || decorationSprites.Length == 0)
        {
            return;
        }

        for (int i = 0; i < decorationCount; i++)
        {
            var sprite = decorationSprites[Random.Range(0, decorationSprites.Length)];
            var decor = new GameObject($"Decor_{i}");
            decor.transform.SetParent(transform);
            var renderer = decor.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = -2;
            decor.transform.position = RandomPosition();
        }
    }

    private void SpawnCollectibles()
    {
        for (int i = 0; i < collectibleCount; i++)
        {
            var collectible = new GameObject($"Collectible_{i}");
            collectible.transform.SetParent(transform);
            var renderer = collectible.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = 1;

            var collider = collectible.AddComponent<CircleCollider2D>();
            collider.radius = 0.2f;
            collider.isTrigger = true;

            var collectibleScript = collectible.AddComponent<Collectible>();
            collectibleScript.ConfigureAnimation(collectibleFrames, 6f);

            collectible.transform.position = RandomPosition();
        }
    }

    private Vector2 RandomPosition()
    {
        float x = Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f);
        float y = Random.Range(-areaSize.y * 0.5f, areaSize.y * 0.5f);
        return new Vector2(x, y);
    }
}