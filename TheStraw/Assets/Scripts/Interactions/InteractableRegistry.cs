using System.Collections.Generic;
using UnityEngine;

namespace TheStraw.Interactions
{
    public static class InteractableRegistry
    {
        private static readonly List<IInteractable> Interactables = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset()
        {
            Interactables.Clear();
        }

        public static void Register(IInteractable interactable)
        {
            if (interactable is not Object unityObject || unityObject == null || Interactables.Contains(interactable))
            {
                return;
            }

            Interactables.Add(interactable);
        }

        public static void Unregister(IInteractable interactable)
        {
            Interactables.Remove(interactable);
        }

        public static IInteractable GetClosest(Vector2 position, float radius)
        {
            IInteractable closest = null;
            float closestDistanceSqr = radius * radius;

            for (int index = Interactables.Count - 1; index >= 0; index--)
            {
                IInteractable candidate = Interactables[index];
                if (candidate is not Object unityObject || unityObject == null)
                {
                    Interactables.RemoveAt(index);
                    continue;
                }

                if (!candidate.CanInteract)
                {
                    continue;
                }

                float distanceSqr = (candidate.InteractionPosition - position).sqrMagnitude;
                if (distanceSqr <= closestDistanceSqr)
                {
                    closest = candidate;
                    closestDistanceSqr = distanceSqr;
                }
            }

            return closest;
        }
    }
}
