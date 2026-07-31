using TheStraw.Time;
using TMPro;
using UnityEngine;

namespace TheStraw.UI
{
    /// <summary>
    /// Displays a <see cref="WorkdayClock" /> value as readable 12-hour time.
    /// </summary>
    public sealed class WorkdayClockDisplay : MonoBehaviour
    {
        [SerializeField] private WorkdayClock workdayClock;
        [SerializeField] private TMP_Text timeLabel;

        private int lastDisplayedMinute = int.MinValue;

        private void OnEnable()
        {
            lastDisplayedMinute = int.MinValue;

            if (workdayClock == null)
            {
                Debug.LogWarning("WorkdayClockDisplay requires a WorkdayClock reference.", this);
                return;
            }

            if (timeLabel == null)
            {
                Debug.LogWarning("WorkdayClockDisplay requires a TMP_Text reference.", this);
                return;
            }

            workdayClock.OnTimeChanged += DisplayTime;
            DisplayTime(workdayClock.CurrentTime);
        }

        private void OnDisable()
        {
            if (workdayClock != null)
            {
                workdayClock.OnTimeChanged -= DisplayTime;
            }
        }

        private void DisplayTime(float decimalHours)
        {
            if (timeLabel == null)
            {
                return;
            }

            int totalMinutes = Mathf.Clamp(Mathf.RoundToInt(decimalHours * 60f), 0, 24 * 60);
            if (totalMinutes == lastDisplayedMinute)
            {
                return;
            }

            lastDisplayedMinute = totalMinutes;

            int hour24 = totalMinutes / 60 % 24;
            int minute = totalMinutes % 60;
            int hour12 = hour24 % 12;
            if (hour12 == 0)
            {
                hour12 = 12;
            }

            string period = hour24 < 12 ? "AM" : "PM";
            timeLabel.text = $"{hour12}:{minute:00} {period}";
        }
    }
}
