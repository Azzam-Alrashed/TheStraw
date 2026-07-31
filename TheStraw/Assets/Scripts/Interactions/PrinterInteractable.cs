using UnityEngine;

namespace TheStraw.Interactions
{
    public sealed class PrinterInteractable : MonoBehaviour, IInteractable
    {
        public bool CanInteract => isActiveAndEnabled;
        public Vector2 InteractionPosition => transform.position;

        private void OnEnable()
        {
            InteractableRegistry.Register(this);
        }

        private void OnDisable()
        {
            InteractableRegistry.Unregister(this);
        }

        public void Interact()
        {
            Debug.Log("Printer interacted");
        }
    }
}
