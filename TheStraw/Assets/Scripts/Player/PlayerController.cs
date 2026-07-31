using UnityEngine;

namespace TheStraw.Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(PlayerInputReader))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float movementSpeed = 5f;

        private Rigidbody2D body;
        private PlayerInputReader input;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            input = GetComponent<PlayerInputReader>();
        }

        private void FixedUpdate()
        {
            Vector2 direction = Vector2.ClampMagnitude(input.Movement, 1f);
            Vector2 nextPosition = body.position + direction * movementSpeed * UnityEngine.Time.fixedDeltaTime;

            body.MovePosition(nextPosition);
        }
    }
}
