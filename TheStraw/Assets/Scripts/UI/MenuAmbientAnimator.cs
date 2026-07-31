using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class MenuAmbientAnimator : MonoBehaviour
{
    [SerializeField] private GameObject creditsPopup;
    [SerializeField] private Image windowGlow;
    [SerializeField] private RectTransform[] dustPixels;
    [SerializeField] private RectTransform selectionArrow;

    private Vector2[] dustOrigins;
    private float nextFlickerTime;

    private void Awake()
    {
        dustOrigins = new Vector2[dustPixels.Length];
        for (var i = 0; i < dustPixels.Length; i++)
        {
            dustOrigins[i] = dustPixels[i].anchoredPosition;
        }
    }

    private void Update()
    {
        var popupOpen = creditsPopup.activeSelf;
        selectionArrow.gameObject.SetActive(!popupOpen);
        if (popupOpen)
        {
            return;
        }

        AnimateGlow();
        AnimateDust();
        AnimateSelectionArrow();
    }

    private void AnimateGlow()
    {
        if (Time.unscaledTime >= nextFlickerTime)
        {
            var color = windowGlow.color;
            color.a = color.a > 0.07f ? 0.045f : 0.09f;
            windowGlow.color = color;
            nextFlickerTime = Time.unscaledTime + (color.a > 0.07f ? 0.12f : 1.35f);
        }
    }

    private void AnimateDust()
    {
        var step = Mathf.FloorToInt(Time.unscaledTime * 2f);
        for (var i = 0; i < dustPixels.Length; i++)
        {
            var x = (step + i * 2) % 7;
            var y = ((step / 2) + i * 3) % 5;
            dustPixels[i].anchoredPosition = dustOrigins[i] + new Vector2(x, y);
        }
    }

    private void AnimateSelectionArrow()
    {
        var selected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
        if (selected != null && selected.TryGetComponent<RectTransform>(out var selectedRect))
        {
            selectionArrow.position = new Vector3(selectionArrow.position.x, selectedRect.position.y, selectionArrow.position.z);
        }

        selectionArrow.gameObject.SetActive(Mathf.FloorToInt(Time.unscaledTime * 3f) % 2 == 0);
    }
}
