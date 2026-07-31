using System;
using UnityEngine;

namespace TheStraw.Stress
{
    /// <summary>
    /// Stores and publishes the player's stress level for gameplay systems.
    /// </summary>
    public sealed class StressManager : MonoBehaviour
    {
        [SerializeField, Range(0, 100)] private int startingStress = 0;

        /// <summary>
        /// Gets the current stress value, clamped between 0 and 100.
        /// </summary>
        public int CurrentStress { get; private set; }

        /// <summary>
        /// Raised when the current stress value changes.
        /// </summary>
        public event Action<int> OnStressChanged;

        /// <summary>
        /// Raised when stress transitions from below the maximum value to 100.
        /// </summary>
        public event Action OnMaxStressReached;

        private void Awake()
        {
            SetStress(startingStress);
        }

        /// <summary>
        /// Increases stress by a non-negative amount.
        /// </summary>
        /// <param name="amount">The amount of stress to add.</param>
        public void AddStress(int amount)
        {
            if (!IsValidAmount(amount, nameof(AddStress)))
            {
                return;
            }

            SetStress(amount > 100 - CurrentStress ? 100 : CurrentStress + amount);
        }

        /// <summary>
        /// Reduces stress by a non-negative amount.
        /// </summary>
        /// <param name="amount">The amount of stress to remove.</param>
        public void ReduceStress(int amount)
        {
            if (!IsValidAmount(amount, nameof(ReduceStress)))
            {
                return;
            }

            SetStress(amount > CurrentStress ? 0 : CurrentStress - amount);
        }

        /// <summary>
        /// Sets stress to the supplied value, clamped between 0 and 100.
        /// </summary>
        /// <param name="value">The requested stress value.</param>
        public void SetStress(int value)
        {
            int previousStress = CurrentStress;
            int clampedStress = Mathf.Clamp(value, 0, 100);

            if (previousStress == clampedStress)
            {
                return;
            }

            CurrentStress = clampedStress;
            OnStressChanged?.Invoke(CurrentStress);

            if (previousStress < 100 && CurrentStress == 100)
            {
                OnMaxStressReached?.Invoke();
            }
        }

        private static bool IsValidAmount(int amount, string methodName)
        {
            if (amount >= 0)
            {
                return true;
            }

            Debug.LogWarning($"{methodName} requires a non-negative amount. Received {amount}.");
            return false;
        }
    }
}
