using TheStraw.Interactions;
using TheStraw.Player;
using TheStraw.Time;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TheStraw.UI
{
    [RequireComponent(typeof(PlayerInputReader))]
    public sealed class PauseMenuController : MonoBehaviour
    {
        private const string OfficeSceneName = "Office";
        private const string MainMenuSceneName = "MainMenu";

        private PlayerInputReader input;
        private WorkdayClock workdayClock;
        private InteractionPrompt interactionPrompt;
        private Animator playerAnimator;
        private GameObject canvasRoot;
        private GameObject rootPanel;
        private GameObject settingsPanel;
        private GameObject confirmationPanel;
        private Text confirmationText;
        private Slider volumeSlider;
        private Button resumeButton;
        private Button cancelConfirmationButton;
        private AudioSettingsService audioSettings;
        private PendingAction pendingAction;
        private float animatorSpeed;

        private enum PendingAction { None, Restart, MainMenu }

        public bool IsPaused { get; private set; }

        private void Awake()
        {
            input = GetComponent<PlayerInputReader>();
            playerAnimator = GetComponent<Animator>();
            workdayClock = FindAnyObjectByType<WorkdayClock>();
            interactionPrompt = FindAnyObjectByType<InteractionPrompt>();
            audioSettings = FindAnyObjectByType<AudioSettingsService>();
            if (audioSettings == null)
            {
                audioSettings = new GameObject("Audio Settings").AddComponent<AudioSettingsService>();
            }

            CreateUi();
            canvasRoot.SetActive(false);
        }

        private void Update()
        {
            if (!input.PausePressed)
            {
                return;
            }

            if (!IsPaused)
            {
                Pause();
            }
            else if (pendingAction != PendingAction.None)
            {
                CancelConfirmation();
            }
            else if (settingsPanel.activeSelf)
            {
                CloseSettings();
            }
            else
            {
                Resume();
            }
        }

        private void OnDisable()
        {
            if (IsPaused)
            {
                RestoreGameplay();
            }
        }

        public void Resume()
        {
            if (!IsPaused)
            {
                return;
            }

            canvasRoot.SetActive(false);
            RestoreGameplay();
        }

        public void OpenSettings()
        {
            rootPanel.SetActive(false);
            settingsPanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(volumeSlider.gameObject);
        }

        public void CloseSettings()
        {
            settingsPanel.SetActive(false);
            rootPanel.SetActive(true);
            Select(resumeButton);
        }

        public void RequestRestart() => ShowConfirmation(PendingAction.Restart, "Restart the workday?");
        public void RequestMainMenu() => ShowConfirmation(PendingAction.MainMenu, "Return to the main menu?");

        public void CancelConfirmation()
        {
            pendingAction = PendingAction.None;
            confirmationPanel.SetActive(false);
            rootPanel.SetActive(true);
            Select(resumeButton);
        }

        public void ConfirmPendingAction()
        {
            if (pendingAction == PendingAction.None)
            {
                return;
            }

            string sceneName = pendingAction == PendingAction.Restart ? OfficeSceneName : MainMenuSceneName;
            RestoreGameplay();
            SceneManager.LoadScene(sceneName);
        }

        private void Pause()
        {
            IsPaused = true;
            input.GameplayInputEnabled = false;
            animatorSpeed = playerAnimator != null ? playerAnimator.speed : 1f;
            if (playerAnimator != null) playerAnimator.speed = 0f;
            if (interactionPrompt != null) interactionPrompt.gameObject.SetActive(false);
            workdayClock?.PauseClock();
            UnityEngine.Time.timeScale = 0f;
            canvasRoot.SetActive(true);
            rootPanel.SetActive(true);
            settingsPanel.SetActive(false);
            confirmationPanel.SetActive(false);
            Select(resumeButton);
        }

        private void RestoreGameplay()
        {
            UnityEngine.Time.timeScale = 1f;
            input.GameplayInputEnabled = true;
            if (playerAnimator != null) playerAnimator.speed = animatorSpeed;
            if (interactionPrompt != null) interactionPrompt.gameObject.SetActive(true);
            workdayClock?.ResumeClock();
            pendingAction = PendingAction.None;
            IsPaused = false;
        }

        private void ShowConfirmation(PendingAction action, string message)
        {
            pendingAction = action;
            rootPanel.SetActive(false);
            confirmationText.text = message;
            confirmationPanel.SetActive(true);
            Select(cancelConfirmationButton);
        }

        private void CreateUi()
        {
            if (FindAnyObjectByType<EventSystem>() == null)
            {
                GameObject eventSystem = new("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
                eventSystem.GetComponent<InputSystemUIInputModule>().actionsAsset = input.InputActions;
            }

            canvasRoot = new GameObject("Pause Menu Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasRoot.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            CanvasScaler scaler = canvasRoot.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            Image overlay = CreateImage(canvasRoot.transform, "Overlay", new Color(0.03f, 0.06f, 0.1f, 0.82f));
            Stretch(overlay.rectTransform);
            rootPanel = CreatePanel("PAUSED", canvasRoot.transform);
            resumeButton = CreateButton(rootPanel.transform, "Resume", Resume, 100f);
            CreateButton(rootPanel.transform, "Restart", RequestRestart, 30f);
            CreateButton(rootPanel.transform, "Settings", OpenSettings, -40f);
            CreateButton(rootPanel.transform, "Main Menu", RequestMainMenu, -110f);

            settingsPanel = CreatePanel("SETTINGS", canvasRoot.transform);
            Text volumeLabel = CreateText(settingsPanel.transform, "Master Volume", 24, new Vector2(0f, 55f));
            volumeLabel.alignment = TextAnchor.MiddleCenter;
            volumeSlider = CreateSlider(settingsPanel.transform, new Vector2(0f, 5f));
            volumeSlider.SetValueWithoutNotify(audioSettings.MasterVolume);
            volumeSlider.onValueChanged.AddListener(audioSettings.SetMasterVolume);
            CreateButton(settingsPanel.transform, "Back", CloseSettings, -85f);

            confirmationPanel = CreatePanel("CONFIRM", canvasRoot.transform);
            confirmationText = CreateText(confirmationPanel.transform, string.Empty, 25, new Vector2(0f, 55f));
            confirmationText.alignment = TextAnchor.MiddleCenter;
            CreateButton(confirmationPanel.transform, "Confirm", ConfirmPendingAction, -15f);
            cancelConfirmationButton = CreateButton(confirmationPanel.transform, "Cancel", CancelConfirmation, -85f);
        }

        private static GameObject CreatePanel(string title, Transform parent)
        {
            Image panel = CreateImage(parent, title + " Panel", new Color(0.1f, 0.18f, 0.28f, 0.98f));
            RectTransform rect = panel.rectTransform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(520f, 420f);
            CreateText(panel.transform, title, 40, new Vector2(0f, 155f)).alignment = TextAnchor.MiddleCenter;
            return panel.gameObject;
        }

        private static Button CreateButton(Transform parent, string label, UnityEngine.Events.UnityAction action, float y)
        {
            GameObject buttonObject = new(label + " Button", typeof(RectTransform), typeof(Image), typeof(Button), typeof(MenuButtonFocus));
            buttonObject.transform.SetParent(parent, false);
            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, y);
            rect.sizeDelta = new Vector2(330f, 52f);
            Image image = buttonObject.GetComponent<Image>();
            image.color = new Color(0.25f, 0.46f, 0.62f, 1f);
            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(action);
            CreateText(buttonObject.transform, label, 24, Vector2.zero).alignment = TextAnchor.MiddleCenter;
            return button;
        }

        private static Slider CreateSlider(Transform parent, Vector2 position)
        {
            GameObject sliderObject = new("Master Volume Slider", typeof(RectTransform), typeof(Slider));
            sliderObject.transform.SetParent(parent, false);
            RectTransform rect = sliderObject.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(330f, 28f);
            Image background = CreateImage(sliderObject.transform, "Background", new Color(0.05f, 0.08f, 0.12f, 1f));
            Stretch(background.rectTransform);
            Image fill = CreateImage(sliderObject.transform, "Fill", new Color(0.35f, 0.75f, 0.85f, 1f));
            RectTransform fillRect = fill.rectTransform;
            Stretch(fillRect);
            Image handle = CreateImage(sliderObject.transform, "Handle", Color.white);
            handle.rectTransform.sizeDelta = new Vector2(24f, 36f);
            Slider slider = sliderObject.GetComponent<Slider>();
            slider.fillRect = fillRect;
            slider.handleRect = handle.rectTransform;
            slider.targetGraphic = handle;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 1f;
            return slider;
        }

        private static Image CreateImage(Transform parent, string name, Color color)
        {
            GameObject item = new(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            item.transform.SetParent(parent, false);
            Image image = item.GetComponent<Image>();
            image.color = color;
            return image;
        }

        private static Text CreateText(Transform parent, string value, int size, Vector2 position)
        {
            GameObject item = new("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            item.transform.SetParent(parent, false);
            RectTransform rect = item.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(450f, 55f);
            Text text = item.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.text = value;
            text.fontSize = size;
            text.color = Color.white;
            return text;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        }

        private static void Select(Selectable selectable)
        {
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(selectable.gameObject);
            }
        }
    }
}
