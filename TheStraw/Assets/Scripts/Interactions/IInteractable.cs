using UnityEngine;

namespace TheStraw.Interactions
{
    public interface IInteractable
    {
        bool CanInteract { get; }
        Vector2 InteractionPosition { get; }
        void Interact();
    }
}
