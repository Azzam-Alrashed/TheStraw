using System;
using TheStraw.Player;
using UnityEngine;

namespace TheStraw.Interactions
{
    [RequireComponent(typeof(PlayerInputReader))]
    public sealed class PlayerInteractionController : MonoBehaviour
    {
        [SerializeField] private Vector2 feetOffset = new(0f, -0.4f);
        [SerializeField, Min(0f)] private float interactionRadius = 0.75f;

        private PlayerInputReader input;

        private IInteractable currentInteractable;

        public IInteractable CurrentInteractable => IsValid(currentInteractable) ? currentInteractable : null;
        public bool HasInteractable => CurrentInteractable != null;
        public event Action<bool> PromptVisibilityChanged;

        private void Awake()
        {
            input = GetComponent<PlayerInputReader>();
        }

        private void Update()
        {
            SetCurrentInteractable(InteractableRegistry.GetClosest(GetInteractionOrigin(), interactionRadius));

            IInteractable interactable = CurrentInteractable;
            if (interactable != null && input.InteractPressed)
            {
                interactable.Interact();
            }
        }

        private Vector2 GetInteractionOrigin()
        {
            return (Vector2)transform.position + feetOffset;
        }

        private void SetCurrentInteractable(IInteractable interactable)
        {
            if (ReferenceEquals(currentInteractable, interactable))
            {
                return;
            }

            currentInteractable = interactable;
            PromptVisibilityChanged?.Invoke(CurrentInteractable != null);
        }

        private static bool IsValid(IInteractable interactable)
        {
            return interactable is UnityEngine.Object unityObject && unityObject != null && interactable.CanInteract;
        }
    }
}
