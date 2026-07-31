using UnityEngine;
using UnityEngine.Audio;

namespace TheStraw.UI
{
    /// <summary>Persists the global master volume and applies it to the configured mixer.</summary>
    public sealed class AudioSettingsService : MonoBehaviour
    {
        private const string MasterVolumeKey = "Settings.MasterVolume";
        private const string MasterVolumeParameter = "MasterVolume";

        [SerializeField] private AudioMixer audioMixer;

        public float MasterVolume { get; private set; } = 1f;

        private void Awake()
        {
            Load();
        }

        public void Load()
        {
            SetMasterVolume(PlayerPrefs.GetFloat(MasterVolumeKey, 1f), false);
        }

        public void SetMasterVolume(float normalizedVolume)
        {
            SetMasterVolume(normalizedVolume, true);
        }

        private void SetMasterVolume(float normalizedVolume, bool save)
        {
            MasterVolume = Mathf.Clamp01(normalizedVolume);
            float decibels = MasterVolume <= 0.0001f ? -80f : Mathf.Log10(MasterVolume) * 20f;

            if (audioMixer != null)
            {
                audioMixer.SetFloat(MasterVolumeParameter, decibels);
            }

            // There are no routed audio sources yet. This makes the setting functional until
            // future scene audio is assigned to the exposed mixer parameter.
            AudioListener.volume = MasterVolume;

            if (save)
            {
                PlayerPrefs.SetFloat(MasterVolumeKey, MasterVolume);
                PlayerPrefs.Save();
            }
        }
    }
}
