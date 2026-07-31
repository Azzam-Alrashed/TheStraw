using System;
using UnityEngine;

namespace TheStraw.Time
{
    /// <summary>
    /// Advances a configurable in-game workday independently of gameplay systems and UI.
    /// </summary>
    public sealed class WorkdayClock : MonoBehaviour
    {
        [SerializeField, Range(0, 23)] private int startHour = 9;
        [SerializeField, Range(1, 24)] private int endHour = 17;
        [SerializeField, Min(0.01f)] private float workdayDuration = 300f;
        [SerializeField] private bool playOnAwake = true;

        private float elapsedSeconds;
        private int displayedMinute;
        private bool hasStarted;
        private bool hasFinished;

        /// <summary>
        /// Gets the displayed in-game time as decimal hours, rounded down to the current minute.
        /// </summary>
        public float CurrentTime { get; private set; }

        /// <summary>
        /// Gets the elapsed proportion of the configured workday, clamped between 0 and 1.
        /// </summary>
        public float NormalizedProgress { get; private set; }

        /// <summary>
        /// Gets whether the workday is currently advancing.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Raised when the displayed in-game minute changes.
        /// </summary>
        public event Action<float> OnTimeChanged;

        /// <summary>
        /// Raised when <see cref="StartClock" /> begins a valid workday.
        /// </summary>
        public event Action OnWorkdayStarted;

        /// <summary>
        /// Raised once when the workday reaches its configured end time.
        /// </summary>
        public event Action OnWorkdayFinished;

        private void Awake()
        {
            InitializeAtStartTime();
        }

        private void Start()
        {
            if (playOnAwake)
            {
                StartClock();
            }
        }

        private void Update()
        {
            if (!IsRunning)
            {
                return;
            }

            elapsedSeconds += UnityEngine.Time.deltaTime;
            NormalizedProgress = Mathf.Clamp01(elapsedSeconds / workdayDuration);
            SetDisplayedMinute(GetMinuteAtProgress(NormalizedProgress));

            if (NormalizedProgress < 1f)
            {
                return;
            }

            IsRunning = false;
            hasFinished = true;
            OnWorkdayFinished?.Invoke();
        }

        /// <summary>
        /// Restarts the clock at the configured start time and begins a valid workday.
        /// </summary>
        public void StartClock()
        {
            ResetClock();

            if (!HasValidConfiguration())
            {
                return;
            }

            hasStarted = true;
            IsRunning = true;
            OnWorkdayStarted?.Invoke();
        }

        /// <summary>
        /// Pauses an active workday without changing its current time.
        /// </summary>
        public void PauseClock()
        {
            if (!IsRunning)
            {
                return;
            }

            IsRunning = false;
        }

        /// <summary>
        /// Continues a valid, paused, unfinished workday without raising the start event.
        /// </summary>
        public void ResumeClock()
        {
            if (IsRunning || !hasStarted || hasFinished)
            {
                return;
            }

            if (!HasValidConfiguration())
            {
                return;
            }

            IsRunning = true;
        }

        /// <summary>
        /// Returns the clock to the configured start time and leaves it stopped.
        /// </summary>
        public void ResetClock()
        {
            InitializeAtStartTime();
        }

        private void InitializeAtStartTime()
        {
            elapsedSeconds = 0f;
            NormalizedProgress = 0f;
            IsRunning = false;
            hasStarted = false;
            hasFinished = false;
            SetDisplayedMinute(startHour * 60);
        }

        private int GetMinuteAtProgress(float progress)
        {
            int workdayMinutes = (endHour - startHour) * 60;
            return startHour * 60 + Mathf.FloorToInt(progress * workdayMinutes);
        }

        private void SetDisplayedMinute(int minute)
        {
            if (displayedMinute == minute)
            {
                return;
            }

            displayedMinute = minute;
            CurrentTime = displayedMinute / 60f;
            OnTimeChanged?.Invoke(CurrentTime);
        }

        private bool HasValidConfiguration()
        {
            bool hasValidHours = startHour is >= 0 and <= 23
                && endHour is >= 1 and <= 24
                && endHour > startHour;

            if (hasValidHours && workdayDuration > 0f)
            {
                return true;
            }

            Debug.LogWarning(
                "WorkdayClock requires a start hour from 0 to 23, an end hour from 1 to 24 that is later than the start hour, and a positive workday duration.",
                this);
            return false;
        }
    }
}
