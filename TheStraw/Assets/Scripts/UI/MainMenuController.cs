using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public sealed class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject creditsPopup;
    [SerializeField] private Selectable startButton;
    [SerializeField] private Selectable backButton;

    private bool acceptsMenuInput;

    private void Start()
    {
        var canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.pixelPerfect = true;

        if (Camera.main != null)
        {
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
            Camera.main.backgroundColor = new Color(0.94f, 0.89f, 0.78f, 1f);
        }

        BuildMissingMenuElements();
        SelectStart();
        StartCoroutine(EnableMenuInputNextFrame());
    }

    private void Update()
    {
        if (creditsPopup.activeSelf && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseCredits();
        }
    }

    public void StartGame()
    {
        if (!acceptsMenuInput)
        {
            return;
        }

        SceneManager.LoadScene("Office");
    }

    public void OpenCredits()
    {
        creditsPopup.SetActive(true);
        EventSystem.current.SetSelectedGameObject(backButton.gameObject);
    }

    public void CloseCredits()
    {
        creditsPopup.SetActive(false);
        SelectStart();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("Quit requested. Application.Quit() is ignored in the Unity Editor.");
#else
        Application.Quit();
#endif
    }

    private void SelectStart()
    {
        if (EventSystem.current != null && startButton != null)
        {
            EventSystem.current.SetSelectedGameObject(startButton.gameObject);
        }
    }

    private IEnumerator EnableMenuInputNextFrame()
    {
        yield return null;
        acceptsMenuInput = true;
    }

    private void BuildMissingMenuElements()
    {
        var canvasTransform = GetComponent<Canvas>().transform;
        CreateBackground(canvasTransform);
        CreateTitle(canvasTransform, "Title", "THE STRAW", 112f, FontStyles.Bold, new Vector2(0f, -260f), new Vector2(1300f, 150f), new Color(0.16f, 0.14f, 0.12f, 1f));
        CreateTitle(canvasTransform, "Subtitle", "that broke the camel's back", 36f, FontStyles.Italic, new Vector2(0f, -365f), new Vector2(1000f, 60f), new Color(0.35f, 0.3f, 0.25f, 1f));

        var menuButtons = new GameObject("Menu Buttons", typeof(RectTransform)).transform;
        menuButtons.SetParent(canvasTransform, false);
        var menuRect = (RectTransform)menuButtons;
        menuRect.anchorMin = menuRect.anchorMax = new Vector2(0.5f, 0.5f);
        menuRect.anchoredPosition = new Vector2(0f, -110f);
        menuRect.sizeDelta = new Vector2(500f, 330f);

        startButton = CreateButton(menuButtons, "Start Button", "Start", new Vector2(0f, -55f), StartGame);
        CreateButton(menuButtons, "Credits Button", "Credits", new Vector2(0f, -150f), OpenCredits);
        CreateButton(menuButtons, "Quit Button", "Quit", new Vector2(0f, -245f), QuitGame);
        creditsPopup = CreateCreditsPopup(canvasTransform);
    }

    private static void CreateBackground(Transform parent)
    {
        var background = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        background.transform.SetParent(parent, false);
        var rect = (RectTransform)background.transform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        var image = background.GetComponent<Image>();
        image.color = new Color(0.94f, 0.89f, 0.78f, 1f);
        image.raycastTarget = false;
    }

    private static void CreateTitle(Transform parent, string objectName, string content, float fontSize, FontStyles style, Vector2 position, Vector2 size, Color color)
    {
        var textObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);
        var rect = (RectTransform)textObject.transform;
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        var text = textObject.GetComponent<TextMeshProUGUI>();
        text.text = content;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.alignment = TextAlignmentOptions.Center;
        text.color = color;
    }

    private Button CreateButton(Transform parent, string objectName, string label, Vector2 position, UnityEngine.Events.UnityAction action)
    {
        var buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);
        var rect = (RectTransform)buttonObject.transform;
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 1f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(420f, 80f);

        var image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.16f, 0.14f, 0.12f, 1f);
        var button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.colors = new ColorBlock
        {
            normalColor = Color.white,
            highlightedColor = new Color(0.74f, 0.29f, 0.18f, 1f),
            pressedColor = new Color(0.44f, 0.13f, 0.08f, 1f),
            selectedColor = new Color(0.74f, 0.29f, 0.18f, 1f),
            disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f),
            colorMultiplier = 1f,
            fadeDuration = 0.05f
        };
        button.onClick.AddListener(action);

        var labelObject = new GameObject(label + " Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(buttonObject.transform, false);
        var labelText = labelObject.GetComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.fontSize = 34f;
        labelText.fontStyle = FontStyles.Bold;
        labelText.alignment = TextAlignmentOptions.Center;
        labelText.color = new Color(0.94f, 0.89f, 0.78f, 1f);
        ((RectTransform)labelObject.transform).anchorMin = Vector2.zero;
        ((RectTransform)labelObject.transform).anchorMax = Vector2.one;
        ((RectTransform)labelObject.transform).sizeDelta = Vector2.zero;
        return button;
    }

    private GameObject CreateCreditsPopup(Transform canvasTransform)
    {
        var popup = new GameObject("Credits Popup", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        popup.transform.SetParent(canvasTransform, false);
        var popupRect = (RectTransform)popup.transform;
        popupRect.anchorMin = Vector2.zero;
        popupRect.anchorMax = Vector2.one;
        popupRect.sizeDelta = Vector2.zero;
        popup.GetComponent<Image>().color = new Color(0.08f, 0.07f, 0.06f, 0.78f);

        var panel = new GameObject("Credits Panel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(popup.transform, false);
        var panelRect = (RectTransform)panel.transform;
        panelRect.anchorMin = panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(700f, 500f);
        panel.GetComponent<Image>().color = new Color(0.94f, 0.89f, 0.78f, 1f);

        var credits = new GameObject("Credits Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        credits.transform.SetParent(panel.transform, false);
        var creditsText = credits.GetComponent<TextMeshProUGUI>();
        creditsText.text = "Azzam Alrashed\nAmmar Alrashed\nAnas Alhumaid\n\nArt assets by LimeZu";
        creditsText.fontSize = 34f;
        creditsText.alignment = TextAlignmentOptions.Center;
        creditsText.color = new Color(0.16f, 0.14f, 0.12f, 1f);
        var creditsRect = (RectTransform)credits.transform;
        creditsRect.anchorMin = new Vector2(0f, 0.3f);
        creditsRect.anchorMax = new Vector2(1f, 0.95f);
        creditsRect.sizeDelta = new Vector2(-50f, 0f);

        backButton = CreateButton(panel.transform, "Back Button", "Back", new Vector2(0f, 60f), CloseCredits);
        var backRect = (RectTransform)backButton.transform;
        backRect.anchorMin = backRect.anchorMax = new Vector2(0.5f, 0f);
        backRect.anchoredPosition = new Vector2(0f, 65f);
        popup.SetActive(false);
        return popup;
    }
}
