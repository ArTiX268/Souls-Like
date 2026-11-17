using ArTiX.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ArTiX.SoulsLike.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    [ExecuteInEditMode]
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform followTarget;

        [Header("Input")]
        [SerializeField] private InputActionReference lookInput;
        [SerializeField] private InputActionReference lockInput;
        [SerializeField, Min(0)] private float xSensitivity;
        [SerializeField, Min(0)] private float ySensitivity;
        [SerializeField] private bool inverseY;

        [Space(5), Header("Parameters")]
        [SerializeField, Min(0)] private float distance;
        [SerializeField, Tooltip("Relative at the pivot point of the camera target.")] private Vector3 anchorOffset;
        [SerializeField] private float maxPitch;

        [SerializeField, Tooltip("How responsive the camera is. Each value represents the time for the camera to catch up with its target position.")]
        private Vector3 damping;

        [Space(10), Header("Debug")]
        [SerializeField] private Color targetGuizmoColor;
        [SerializeField] private Color anchorGuizmoColor;

        private Vector3 anchorPosition;
        private Vector3 cameraAnchorVelocity;

        private Quaternion AnchorRotation => Quaternion.Euler(targetPitch, targetYaw, 0);
        private float targetYaw;
        private float targetPitch;

        private bool isLocked;
        private Transform lockedTarget;

        private void LateUpdate()
        {
            if (followTarget == null)
            {
                ErrorMessage.MissingReferenceWarning(nameof(followTarget), nameof(CameraController), gameObject);
                return;
            }

            if (Application.isPlaying)
            {
                Vector2 rotationInput = lookInput.action.ReadValue<Vector2>();
                rotationInput.x *= xSensitivity;
                rotationInput.y *= ySensitivity;

                targetYaw += rotationInput.x;
                targetPitch += rotationInput.y * (inverseY ? 1 : -1);
                targetPitch = Mathf.Clamp(targetPitch, -maxPitch, maxPitch);
            }

            transform.position = CalculateTargetPosition();
            transform.LookAt(isLocked ? lockedTarget.position : anchorPosition, Vector3.up);
        }

        private void OnDrawGizmosSelected()
        {
            if (followTarget == null)
            {
                ErrorMessage.MissingReferenceWarning(nameof(followTarget), nameof(CameraController), gameObject);
                return;
            }
            Gizmos.color = targetGuizmoColor;
            Gizmos.DrawSphere(followTarget.position, .1f);

            Gizmos.color = anchorGuizmoColor;
            Gizmos.DrawSphere(anchorPosition, .1f);

            Gizmos.color = anchorGuizmoColor;
            Gizmos.DrawLine(followTarget.position, anchorPosition);

            Gizmos.color = anchorGuizmoColor;
            Gizmos.DrawLine(anchorPosition, transform.position);
        }

        private Vector3 CalculateTargetPosition()
        {
            UpdateAnchorPosition();
            Vector3 targetPosition = anchorPosition + AnchorRotation * Vector3.back * distance;
            return targetPosition;
        }

        private void UpdateAnchorPosition()
        {
            Quaternion dampingModifier = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            anchorPosition = Utilities.BetterVectorSmoothDamp(
                a: anchorPosition,
                b: followTarget.position + anchorOffset,
                currentVelocity: ref cameraAnchorVelocity,
                smoothTime: dampingModifier * damping,
                maxSpeed: Vector3.one * float.MaxValue);
        }
    }
}
