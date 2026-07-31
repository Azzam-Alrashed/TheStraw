using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheStraw.UI
{
    /// <summary>Guarantees that Office gameplay has a pause controller after scene load.</summary>
    internal static class PauseMenuBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AddPauseMenuToOfficePlayer()
        {
            if (SceneManager.GetActiveScene().name != "Office")
            {
                return;
            }

            GameObject player = GameObject.Find("Player");
            if (player != null && player.GetComponent<PauseMenuController>() == null)
            {
                player.AddComponent<PauseMenuController>();
            }
        }
    }
}
