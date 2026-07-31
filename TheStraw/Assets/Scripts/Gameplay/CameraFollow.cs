using UnityEngine;

namespace TheStraw.Gameplay
{
    public sealed class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField, Min(0f)] private float smoothTime = 0.15f;

        private Vector3 velocity;

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            Vector3 targetPosition = new Vector3(
                target.position.x,
                target.position.y,
                transform.position.z);
            transform.position = smoothTime > 0f
                ? Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime)
                : targetPosition;
        }
    }
}
