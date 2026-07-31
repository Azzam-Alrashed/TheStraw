using UnityEngine;
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

    private void Start()
    {
        BuildMissingMenuElements();
        SelectStart();
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

    private void BuildMissingMenuElements()
    {
        var canvasTransform = GetComponent<Canvas>().transform;
        var menuButtons = canvasTransform.Find("Menu Buttons");

        if (startButton == null)
        {
            startButton = menuButtons.Find("Start Button").GetComponent<Button>();
        }

        if (menuButtons.Find("Credits Button") == null)
        {
            CreateButton(menuButtons, "Credits Button", "Credits", new Vector2(0f, -150f), OpenCredits);
        }

        if (menuButtons.Find("Quit Button") == null)
        {
            CreateButton(menuButtons, "Quit Button", "Quit", new Vector2(0f, -245f), QuitGame);
        }

        if (creditsPopup == null)
        {
            creditsPopup = CreateCreditsPopup(canvasTransform);
        }
    }

    private Button CreateButton(Transform parent, string objectName, string label, Vector2 position, UnityEngine.Events.UnityAction action)
    {
        var buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(Image), typeof(Button));
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

        var labelObject = new GameObject(label + " Label", typeof(RectTransform), typeof(TextMeshProUGUI));
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
        var popup = new GameObject("Credits Popup", typeof(RectTransform), typeof(Image));
        popup.transform.SetParent(canvasTransform, false);
        var popupRect = (RectTransform)popup.transform;
        popupRect.anchorMin = Vector2.zero;
        popupRect.anchorMax = Vector2.one;
        popupRect.sizeDelta = Vector2.zero;
        popup.GetComponent<Image>().color = new Color(0.08f, 0.07f, 0.06f, 0.78f);

        var panel = new GameObject("Credits Panel", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(popup.transform, false);
        var panelRect = (RectTransform)panel.transform;
        panelRect.anchorMin = panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(700f, 500f);
        panel.GetComponent<Image>().color = new Color(0.94f, 0.89f, 0.78f, 1f);

        var credits = new GameObject("Credits Text", typeof(RectTransform), typeof(TextMeshProUGUI));
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
