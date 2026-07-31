using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject creditsPopup;
    [SerializeField] private Selectable startButton;
    [SerializeField] private Selectable backButton;

    private bool acceptsMenuInput;

    private void Start()
    {
        creditsPopup.SetActive(false);
        mainMenuPanel.SetActive(true);
        Select(startButton);
        StartCoroutine(EnableInputNextFrame());
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
        if (acceptsMenuInput)
        {
            SceneManager.LoadScene("Office");
        }
    }

    public void OpenCredits()
    {
        creditsPopup.SetActive(true);
        mainMenuPanel.SetActive(false);
        Select(backButton);
    }

    public void CloseCredits()
    {
        creditsPopup.SetActive(false);
        mainMenuPanel.SetActive(true);
        Select(startButton);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("Quit requested. Application.Quit() is ignored in the Unity Editor.");
#else
        Application.Quit();
#endif
    }

    private static void Select(Selectable selectable)
    {
        if (EventSystem.current != null && selectable != null)
        {
            EventSystem.current.SetSelectedGameObject(selectable.gameObject);
        }
    }

    private IEnumerator EnableInputNextFrame()
    {
        yield return null;
        acceptsMenuInput = true;
    }
}
