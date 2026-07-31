using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TheStraw.Stress
{
    /// <summary>
    /// Translates the player's internal stress into gradually blended audiovisual feedback.
    /// </summary>
    public sealed class StressFeedbackController : MonoBehaviour
    {
        [Serializable]
        private sealed class StressAudioLayer
        {
            [SerializeField] private bool enabled = true;
            [SerializeField] private AudioSource source;
            [SerializeField] private AnimationCurve volumeByStress = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            [SerializeField, Range(0f, 1f)] private float maximumVolume = 0.2f;
            [SerializeField] private AnimationCurve pitchByStress = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            [SerializeField, Range(0.5f, 2f)] private float minimumPitch = 1f;
            [SerializeField, Range(0.5f, 2f)] private float maximumPitch = 1.1f;

            private float baselineVolume;
            private float baselinePitch;
            private bool baselineLoop;
            private bool baselineWasPlaying;
            private bool hasCachedBaseline;
            private bool startedByController;

            public void CacheBaseline()
            {
                if (source == null)
                {
                    return;
                }

                baselineVolume = source.volume;
                baselinePitch = source.pitch;
                baselineLoop = source.loop;
                baselineWasPlaying = source.isPlaying;
                hasCachedBaseline = true;
                startedByController = false;
            }

            public void Apply(float stress)
            {
                if (!hasCachedBaseline || source == null)
                {
                    return;
                }

                if (!enabled)
                {
                    Restore();
                    return;
                }

                float volumeResponse = EvaluateCurve(volumeByStress, stress);
                float targetVolume = maximumVolume * volumeResponse;
                source.volume = targetVolume;

                float pitchResponse = EvaluateCurve(pitchByStress, stress);
                source.pitch = Mathf.Lerp(minimumPitch, maximumPitch, pitchResponse);

                if (targetVolume > 0.0001f && !source.isPlaying && source.clip != null)
                {
                    source.loop = true;
                    source.Play();
                    startedByController = true;
                }
                else if (targetVolume <= 0.0001f && startedByController && source.isPlaying)
                {
                    source.Stop();
                    startedByController = false;
                }
            }

            public void Restore()
            {
                if (!hasCachedBaseline || source == null)
                {
                    return;
                }

                source.volume = baselineVolume;
                source.pitch = baselinePitch;
                source.loop = baselineLoop;

                if (startedByController && !baselineWasPlaying && source.isPlaying)
                {
                    source.Stop();
                }

                startedByController = false;
            }

            private static float EvaluateCurve(AnimationCurve curve, float stress)
            {
                return Mathf.Clamp01(curve != null ? curve.Evaluate(stress) : stress);
            }
        }

        [Header("References")]
        [SerializeField] private StressManager stressManager;
        [SerializeField] private Volume stressVolume;

        [Header("Master Response")]
        [SerializeField] private bool enableVisualFeedback = true;
        [SerializeField, Min(0.01f)] private float escalationSpeed = 0.75f;
        [SerializeField, Min(0.01f)] private float recoverySpeed = 0.35f;
        [SerializeField] private AnimationCurve masterResponse = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.2f, 0f),
            new Keyframe(0.4f, 0.15f),
            new Keyframe(0.6f, 0.4f),
            new Keyframe(0.8f, 0.72f),
            new Keyframe(1f, 1f));

        [Header("Vignette")]
        [SerializeField] private bool enableVignette = true;
        [SerializeField] private AnimationCurve vignetteByStress = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.2f, 0f),
            new Keyframe(0.4f, 0.14f),
            new Keyframe(0.6f, 0.36f),
            new Keyframe(0.8f, 0.64f),
            new Keyframe(1f, 1f));
        [SerializeField, Range(0f, 1f)] private float maximumVignette = 0.28f;

        [Header("Color Adjustments")]
        [SerializeField] private bool enableColorAdjustments = true;
        [SerializeField] private AnimationCurve desaturationByStress = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.2f, 0f),
            new Keyframe(0.4f, 0.11f),
            new Keyframe(0.6f, 0.33f),
            new Keyframe(0.8f, 0.67f),
            new Keyframe(1f, 1f));
        [SerializeField, Range(0f, 100f)] private float maximumDesaturation = 18f;
        [SerializeField] private AnimationCurve exposureByStress = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.2f, 0f),
            new Keyframe(0.4f, 0.13f),
            new Keyframe(0.6f, 0.33f),
            new Keyframe(0.8f, 0.67f),
            new Keyframe(1f, 1f));
        [SerializeField, Range(0f, 1f)] private float maximumExposureReduction = 0.15f;

        [Header("Gentle Pulse")]
        [SerializeField] private bool enablePulse = true;
        [SerializeField] private AnimationCurve pulseByStress = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.6f, 0f),
            new Keyframe(0.8f, 0.45f),
            new Keyframe(1f, 1f));
        [SerializeField, Range(0f, 0.05f)] private float maximumPulseVignette = 0.015f;
        [SerializeField, Range(0.1f, 2f)] private float minimumPulseFrequency = 0.5f;
        [SerializeField, Range(0.1f, 2f)] private float maximumPulseFrequency = 1.1f;

        [Header("Optional Internal Audio")]
        [SerializeField] private bool enableInternalAudio = true;
        [SerializeField] private StressAudioLayer heartbeat = new StressAudioLayer();
        [SerializeField] private StressAudioLayer breathing = new StressAudioLayer();

        [Header("Optional Environmental Audio")]
        [SerializeField] private bool enableEnvironmentalAudio = true;
        [SerializeField] private StressAudioLayer[] environmentalLayers = Array.Empty<StressAudioLayer>();
        [SerializeField] private AudioLowPassFilter ambienceLowPass;
        [SerializeField] private AnimationCurve ambienceMufflingByStress = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.3f, 0f),
            new Keyframe(0.6f, 0.25f),
            new Keyframe(0.8f, 0.65f),
            new Keyframe(1f, 1f));
        [SerializeField, Range(1000f, 22000f)] private float minimumAmbienceCutoff = 6500f;

        private Vignette vignette;
        private ColorAdjustments colorAdjustments;
        private float baselineVignette;
        private float baselineSaturation;
        private float baselineExposure;
        private float baselineAmbienceCutoff;
        private float targetStress;
        private float smoothedStress;
        private float pulsePhase;
        private bool hasVignette;
        private bool hasColorAdjustments;
        private bool hasAmbienceLowPass;
        private bool warnedAboutVolume;
        private bool warnedAboutVignette;
        private bool warnedAboutColorAdjustments;

        private void OnEnable()
        {
            CacheVisualOverrides();
            CacheAudioBaselines();

            if (stressManager == null)
            {
                Debug.LogWarning("StressFeedbackController requires a StressManager reference.", this);
                RestoreBaselines();
                return;
            }

            stressManager.OnStressChanged += HandleStressChanged;
            float initialStress = NormalizeStress(stressManager.CurrentStress);
            targetStress = initialStress;
            smoothedStress = initialStress;
            pulsePhase = 0f;
            ApplyFeedback(0f);
        }

        private void OnDisable()
        {
            if (stressManager != null)
            {
                stressManager.OnStressChanged -= HandleStressChanged;
            }

            RestoreBaselines();
        }

        private void Update()
        {
            float speed = targetStress > smoothedStress ? escalationSpeed : recoverySpeed;
            float blend = 1f - Mathf.Exp(-Mathf.Max(0.01f, speed) * UnityEngine.Time.deltaTime);
            smoothedStress = Mathf.Lerp(smoothedStress, targetStress, blend);
            ApplyFeedback(UnityEngine.Time.deltaTime);
        }

        private void HandleStressChanged(int stress)
        {
            targetStress = NormalizeStress(stress);
        }

        private void CacheVisualOverrides()
        {
            hasVignette = false;
            hasColorAdjustments = false;

            if (stressVolume == null || stressVolume.sharedProfile == null)
            {
                if (!warnedAboutVolume)
                {
                    Debug.LogWarning(
                        "StressFeedbackController requires a Volume with a profile for visual feedback.",
                        this);
                    warnedAboutVolume = true;
                }

                return;
            }

            VolumeProfile runtimeProfile = stressVolume.profile;
            if (runtimeProfile.TryGet(out vignette))
            {
                baselineVignette = vignette.intensity.value;
                hasVignette = true;
            }
            else if (!warnedAboutVignette)
            {
                Debug.LogWarning("The stress Volume profile is missing a Vignette override.", this);
                warnedAboutVignette = true;
            }

            if (runtimeProfile.TryGet(out colorAdjustments))
            {
                baselineSaturation = colorAdjustments.saturation.value;
                baselineExposure = colorAdjustments.postExposure.value;
                hasColorAdjustments = true;
            }
            else if (!warnedAboutColorAdjustments)
            {
                Debug.LogWarning("The stress Volume profile is missing a Color Adjustments override.", this);
                warnedAboutColorAdjustments = true;
            }
        }

        private void CacheAudioBaselines()
        {
            heartbeat?.CacheBaseline();
            breathing?.CacheBaseline();

            if (environmentalLayers != null)
            {
                foreach (StressAudioLayer layer in environmentalLayers)
                {
                    layer?.CacheBaseline();
                }
            }

            hasAmbienceLowPass = ambienceLowPass != null;
            if (hasAmbienceLowPass)
            {
                baselineAmbienceCutoff = ambienceLowPass.cutoffFrequency;
            }
        }

        private void ApplyFeedback(float deltaTime)
        {
            float response = EvaluateCurve(masterResponse, smoothedStress);
            float pulse = GetPulse(response, deltaTime);

            ApplyVisualFeedback(response, pulse);
            ApplyAudioFeedback(response);
        }

        private void ApplyVisualFeedback(float response, float pulse)
        {
            if (hasVignette)
            {
                if (enableVisualFeedback && enableVignette)
                {
                    float vignetteResponse = EvaluateCurve(vignetteByStress, response);
                    vignette.intensity.value = Mathf.Clamp01(
                        baselineVignette + vignetteResponse * maximumVignette + pulse);
                }
                else
                {
                    vignette.intensity.value = baselineVignette;
                }
            }

            if (hasColorAdjustments)
            {
                if (enableVisualFeedback && enableColorAdjustments)
                {
                    float desaturation = EvaluateCurve(desaturationByStress, response);
                    float exposure = EvaluateCurve(exposureByStress, response);
                    colorAdjustments.saturation.value = baselineSaturation - desaturation * maximumDesaturation;
                    colorAdjustments.postExposure.value = baselineExposure - exposure * maximumExposureReduction;
                }
                else
                {
                    colorAdjustments.saturation.value = baselineSaturation;
                    colorAdjustments.postExposure.value = baselineExposure;
                }
            }
        }

        private void ApplyAudioFeedback(float response)
        {
            if (enableInternalAudio)
            {
                heartbeat?.Apply(response);
                breathing?.Apply(response);
            }
            else
            {
                heartbeat?.Restore();
                breathing?.Restore();
            }

            if (enableEnvironmentalAudio)
            {
                if (environmentalLayers != null)
                {
                    foreach (StressAudioLayer layer in environmentalLayers)
                    {
                        layer?.Apply(response);
                    }
                }

                if (hasAmbienceLowPass)
                {
                    float muffling = EvaluateCurve(ambienceMufflingByStress, response);
                    ambienceLowPass.cutoffFrequency = Mathf.Lerp(
                        baselineAmbienceCutoff,
                        minimumAmbienceCutoff,
                        muffling);
                }
            }
            else
            {
                RestoreEnvironmentalAudio();
            }
        }

        private float GetPulse(float response, float deltaTime)
        {
            if (!enableVisualFeedback || !enablePulse || !enableVignette)
            {
                return 0f;
            }

            float pulseAmount = EvaluateCurve(pulseByStress, response);
            float frequency = Mathf.Lerp(minimumPulseFrequency, maximumPulseFrequency, response);
            pulsePhase = Mathf.Repeat(pulsePhase + deltaTime * frequency * Mathf.PI * 2f, Mathf.PI * 2f);
            return Mathf.Sin(pulsePhase) * maximumPulseVignette * pulseAmount;
        }

        private void RestoreBaselines()
        {
            if (hasVignette)
            {
                vignette.intensity.value = baselineVignette;
            }

            if (hasColorAdjustments)
            {
                colorAdjustments.saturation.value = baselineSaturation;
                colorAdjustments.postExposure.value = baselineExposure;
            }

            heartbeat?.Restore();
            breathing?.Restore();
            RestoreEnvironmentalAudio();
        }

        private void RestoreEnvironmentalAudio()
        {
            if (environmentalLayers != null)
            {
                foreach (StressAudioLayer layer in environmentalLayers)
                {
                    layer?.Restore();
                }
            }

            if (hasAmbienceLowPass && ambienceLowPass != null)
            {
                ambienceLowPass.cutoffFrequency = baselineAmbienceCutoff;
            }
        }

        private static float NormalizeStress(int stress)
        {
            return Mathf.Clamp01(stress / 100f);
        }

        private static float EvaluateCurve(AnimationCurve curve, float stress)
        {
            return Mathf.Clamp01(curve != null ? curve.Evaluate(stress) : stress);
        }
    }
}
