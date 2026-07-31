using UnityEngine;
using UnityEngine.InputSystem;

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
        public bool PausePressed => pauseAction != null && pauseAction.WasPressedThisFrame();

        private void Awake()
        {
            moveAction = inputActions.FindAction(MoveActionName, true);
            interactAction = inputActions.FindAction(InteractActionName, true);
            pauseAction = inputActions.FindAction(PauseActionName, true);
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
