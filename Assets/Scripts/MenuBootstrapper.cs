using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuBootstrapper : MonoBehaviour
{
    private void Start()
    {
        var manager = FindObjectOfType<GameManager>();
        if (manager == null)
        {
            var managerObject = new GameObject("GameManager");
            manager = managerObject.AddComponent<GameManager>();
        }

        BuildMenu(manager);
    }

    private void BuildMenu(GameManager manager)
    {
        var canvasObject = new GameObject("Canvas");
        var canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        if (FindObjectOfType<EventSystem>() == null)
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        var title = CreateText("Title", canvas.transform, "Demon Slayer: Bamboo Dash", TextAnchor.UpperCenter);
        title.fontSize = 28;
        var titleRect = title.rectTransform;
        titleRect.anchoredPosition = new Vector2(0f, -40f);

        var startButton = CreateButton("StartButton", canvas.transform, "START GAME");
        startButton.onClick.AddListener(manager.LoadGame);
        var startRect = startButton.GetComponent<RectTransform>();
        startRect.anchoredPosition = new Vector2(0f, -120f);

        var exitButton = CreateButton("ExitButton", canvas.transform, "EXIT");
        exitButton.onClick.AddListener(manager.Quit);
        var exitRect = exitButton.GetComponent<RectTransform>();
        exitRect.anchoredPosition = new Vector2(0f, -180f);
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

        var rect = text.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(320f, 50f);
        rect.anchoredPosition = Vector2.zero;
        return text;
    }

    private Button CreateButton(string name, Transform parent, string label)
    {
        var buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);
        var image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.95f, 0.77f, 0.3f);

        var rect = buttonObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200f, 50f);

        var button = buttonObj.AddComponent<Button>();

        var text = CreateText("Label", buttonObj.transform, label, TextAnchor.MiddleCenter);
        text.color = Color.black;

        return button;
    }
}