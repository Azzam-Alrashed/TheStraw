using UnityEngine;
using UnityEngine.InputSystem;
using TheStraw.UI;

namespace TheStraw.Player
{
    public sealed class PlayerInputReader : MonoBehaviour
    {
        private const string MoveActionName = "Player/Move";
        private const string InteractActionName = "Player/Interact";
        private const string PauseActionName = "Player/Pause";

        [SerializeField] private InputActionAsset inputActions;

        private InputAction moveAction;
        private InputAction interactAction;
        private InputAction pauseAction;

        public bool GameplayInputEnabled { get; set; } = true;
        public InputActionAsset InputActions => inputActions;

        public Vector2 Movement => GameplayInputEnabled ? moveAction?.ReadValue<Vector2>() ?? Vector2.zero : Vector2.zero;
        public bool InteractPressed => GameplayInputEnabled && interactAction != null && interactAction.WasPressedThisFrame();
        /// <summary>
        /// Returns whether the pause command was pressed. The device checks are a deliberate
        /// fallback for platform UI paths that can consume an action-map binding.
        /// </summary>
        public bool PausePressed => (pauseAction != null && pauseAction.WasPressedThisFrame())
            || (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            || (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame);

        private void Awake()
        {
            moveAction = inputActions.FindAction(MoveActionName, true);
            interactAction = inputActions.FindAction(InteractActionName, true);
            pauseAction = inputActions.FindAction(PauseActionName, true);

            // The player is the Office gameplay entry point. Ensure the pause controller is
            // present even when a scene was created before the pause-menu component existed.
            if (GetComponent<PauseMenuController>() == null)
            {
                gameObject.AddComponent<PauseMenuController>();
            }
        }

        private void OnEnable()
        {
            moveAction?.Enable();
            interactAction?.Enable();
            pauseAction?.Enable();
        }

        private void OnDisable()
        {
            moveAction?.Disable();
            interactAction?.Disable();
            pauseAction?.Disable();
        }
    }
}
