using UnityEngine;
using UnityEngine.InputSystem;

namespace TheStraw.Player
{
    public sealed class PlayerInputReader : MonoBehaviour
    {
        private const string MoveActionName = "Player/Move";
        private const string InteractActionName = "Player/Interact";

        [SerializeField] private InputActionAsset inputActions;

        private InputAction moveAction;
        private InputAction interactAction;

        public Vector2 Movement => moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
        public bool InteractPressed => interactAction != null && interactAction.WasPressedThisFrame();

        private void Awake()
        {
            moveAction = inputActions.FindAction(MoveActionName, true);
            interactAction = inputActions.FindAction(InteractActionName, true);
        }

        private void OnEnable()
        {
            moveAction?.Enable();
            interactAction?.Enable();
        }

        private void OnDisable()
        {
            moveAction?.Disable();
            interactAction?.Disable();
        }
    }
}
