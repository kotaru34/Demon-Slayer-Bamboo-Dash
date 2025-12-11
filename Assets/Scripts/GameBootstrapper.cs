using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private Vector2 playAreaSize = new Vector2(14f, 8f);

    private void Start()
    {
        var manager = FindObjectOfType<GameManager>();
        if (manager == null)
        {
            var managerObject = new GameObject("GameManager");
            manager = managerObject.AddComponent<GameManager>();
        }
        manager.ResetState();

        BuildWorld(manager);
        SetupUI(manager);
    }

    private void BuildWorld(GameManager manager)
    {
        var backgroundSprite = Resources.Load<Sprite>("Sprites/background");
        var playerSprite = Resources.Load<Sprite>("Sprites/player");
        var enemySprite = Resources.Load<Sprite>("Sprites/demon-grid-39x50");
        var collectibleFrames = Resources.LoadAll<Sprite>("Sprites/collectibles-grid-50x50");
        var decorationSprites = LoadDecorations();

        CreateBackground(backgroundSprite);
        var player = CreatePlayer(playerSprite);
        CreateEnemy(enemySprite);

        var generatorObject = new GameObject("LevelGenerator");
        var generator = generatorObject.AddComponent<LevelGenerator>();
        generator.Configure(decorationSprites, collectibleFrames, playAreaSize);
        generator.Generate();

        manager.SetCollectSound(GameManager.CreateChime());
        manager.SetMusic(GameManager.CreateLoopingDrone());
    }

    private void SetupUI(GameManager manager)
    {
        var canvasObject = new GameObject("Canvas");
        var canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        if (FindObjectOfType<EventSystem>() == null)
        {
            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            eventSystem.transform.SetParent(canvasObject.transform);
        }

        var hudPanel = new GameObject("HUD");
        hudPanel.transform.SetParent(canvas.transform, false);
        var hudRect = hudPanel.AddComponent<RectTransform>();
        hudRect.anchorMin = new Vector2(0f, 1f);
        hudRect.anchorMax = new Vector2(0f, 1f);
        hudRect.pivot = new Vector2(0f, 1f);
        hudRect.anchoredPosition = new Vector2(15f, -15f);

        var scoreText = CreateText("ScoreText", hudPanel.transform, "Score: 0", TextAnchor.UpperLeft);
        var healthText = CreateText("HealthText", hudPanel.transform, "Health: 3", TextAnchor.LowerLeft);
        healthText.rectTransform.anchoredPosition = new Vector2(0f, -22f);

        var gameOverPanel = CreateGameOverPanel(canvas.transform, manager);
        manager.RegisterUI(scoreText, healthText, gameOverPanel.panel, gameOverPanel.restartButton);
    }

    private (GameObject panel, Button restartButton) CreateGameOverPanel(Transform parent, GameManager manager)
    {
        var panelObject = new GameObject("GameOverPanel");
        panelObject.transform.SetParent(parent, false);
        var image = panelObject.AddComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0.65f);

        var rect = panelObject.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var title = CreateText("GameOverText", panelObject.transform, "Game Over", TextAnchor.MiddleCenter);
        title.fontSize = 28;
        title.color = Color.white;

        var restartObj = new GameObject("RestartButton");
        restartObj.transform.SetParent(panelObject.transform, false);
        var restartImage = restartObj.AddComponent<Image>();
        restartImage.color = new Color(0.95f, 0.77f, 0.3f);

        var restartRect = restartObj.GetComponent<RectTransform>();
        restartRect.sizeDelta = new Vector2(140f, 40f);
        restartRect.anchoredPosition = new Vector2(0f, -40f);

        var restartButton = restartObj.AddComponent<Button>();
        restartButton.onClick.AddListener(manager.Restart);

        var restartLabel = CreateText("RestartLabel", restartObj.transform, "Restart", TextAnchor.MiddleCenter);
        restartLabel.color = Color.black;

        return (panelObject, restartButton);
    }

    private Text CreateText(string name, Transform parent, string content, TextAnchor alignment)
    {
        var textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        var text = textObj.AddComponent<Text>();
        text.text = content;
        text.alignment = alignment;
        text.color = Color.white;
        text.fontSize = 18;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.raycastTarget = false;

        var rect = text.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200f, 30f);
        rect.anchoredPosition = Vector2.zero;
        return text;
    }

    private Sprite[] LoadDecorations()
    {
        return new[]
        {
            Resources.Load<Sprite>("Sprites/bambooHighWithLeaves-grid-50x100"),
            Resources.Load<Sprite>("Sprites/bambooSmall-grid-50x50"),
            Resources.Load<Sprite>("Sprites/bushes-grid-60x60"),
            Resources.Load<Sprite>("Sprites/grass-grid-80x80")
        };
    }

    private void CreateBackground(Sprite sprite)
    {
        if (sprite == null)
        {
            return;
        }

        var background = new GameObject("Background");
        var renderer = background.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = -5;
    }

    private PlayerController CreatePlayer(Sprite sprite)
    {
        var playerObject = new GameObject("Player");
        playerObject.tag = "Player";
        var renderer = playerObject.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = 2;

        var rb = playerObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        playerObject.AddComponent<BoxCollider2D>();
        var controller = playerObject.AddComponent<PlayerController>();
        controller.SetBounds(new Rect(-playAreaSize.x * 0.5f, -playAreaSize.y * 0.5f, playAreaSize.x, playAreaSize.y));

        return controller;
    }

    private void CreateEnemy(Sprite sprite)
    {
        var enemyObject = new GameObject("Enemy");
        var renderer = enemyObject.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = 2;

        var rb = enemyObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        enemyObject.AddComponent<BoxCollider2D>();
        var controller = enemyObject.AddComponent<EnemyController>();
        enemyObject.transform.position = new Vector2(2f, 0f);
    }
}