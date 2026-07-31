using TheStraw.Interactions;
using UnityEngine;
using UnityEngine.UI;

namespace TheStraw.Interactions
{
    public sealed class InteractionPrompt : MonoBehaviour
    {
        [SerializeField] private PlayerInteractionController interactionController;

        private GameObject promptRoot;

        private void Awake()
        {
            CreatePrompt();
            SetVisible(false);
        }

        private void OnEnable()
        {
            SetVisible(false);

            if (interactionController == null)
            {
                return;
            }

            interactionController.PromptVisibilityChanged += SetVisible;
            SetVisible(interactionController.HasInteractable);
        }

        private void OnDisable()
        {
            if (interactionController != null)
            {
                interactionController.PromptVisibilityChanged -= SetVisible;
            }
        }

        private void CreatePrompt()
        {
            promptRoot = new GameObject("Prompt Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            promptRoot.transform.SetParent(transform, false);

            RectTransform rectTransform = promptRoot.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = new Vector2(0f, -180f);
            rectTransform.sizeDelta = new Vector2(280f, 40f);

            Text text = promptRoot.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.text = "Press E to interact";
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
        }

        private void SetVisible(bool visible)
        {
            if (promptRoot != null)
            {
                promptRoot.SetActive(visible);
            }
        }
    }
}
