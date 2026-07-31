using UnityEngine;

namespace TheStraw.Player
{
    [RequireComponent(typeof(Animator), typeof(PlayerInputReader))]
    public sealed class PlayerAnimator : MonoBehaviour
    {
        private const string DirectionParameter = "Direction";
        private const string IsMovingParameter = "IsMoving";
        private const float MovementThreshold = 0.001f;

        private enum FacingDirection
        {
            Down,
            Up,
            Left,
            Right
        }

        private Animator animator;
        private PlayerInputReader input;
        private FacingDirection facingDirection = FacingDirection.Down;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            input = GetComponent<PlayerInputReader>();
        }

        private void Update()
        {
            Vector2 movement = input.Movement;
            bool isMoving = movement.sqrMagnitude > MovementThreshold * MovementThreshold;

            if (isMoving)
            {
                UpdateFacingDirection(movement);
            }

            animator.SetInteger(DirectionParameter, (int)facingDirection);
            animator.SetBool(IsMovingParameter, isMoving);
        }

        private void UpdateFacingDirection(Vector2 movement)
        {
            float horizontal = Mathf.Abs(movement.x);
            float vertical = Mathf.Abs(movement.y);

            if (horizontal > vertical)
            {
                facingDirection = movement.x > 0f ? FacingDirection.Right : FacingDirection.Left;
            }
            else if (vertical > horizontal)
            {
                facingDirection = movement.y > 0f ? FacingDirection.Up : FacingDirection.Down;
            }
        }
    }
}
