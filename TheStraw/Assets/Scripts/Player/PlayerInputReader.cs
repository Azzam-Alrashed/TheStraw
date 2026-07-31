using UnityEngine;
using UnityEngine.InputSystem;

namespace TheStraw.Player
{
    public sealed class PlayerInputReader : MonoBehaviour
    {
        private const string MoveActionName = "Player/Move";

        [SerializeField] private InputActionAsset inputActions;

        private InputAction moveAction;

        public Vector2 Movement => moveAction?.ReadValue<Vector2>() ?? Vector2.zero;

        private void Awake()
        {
            moveAction = inputActions.FindAction(MoveActionName, true);
        }

        private void OnEnable()
        {
            moveAction?.Enable();
        }

        private void OnDisable()
        {
            moveAction?.Disable();
        }
    }
}
