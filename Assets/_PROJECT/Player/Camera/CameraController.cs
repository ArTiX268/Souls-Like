using ArTiX.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ArTiX.SoulsLike.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    [ExecuteInEditMode]
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform target;

        [Header("Input")]
        [SerializeField] private InputActionReference lookInput;
        [SerializeField, Min(0)] private float xSensitivity;
        [SerializeField, Min(0)] private float ySensitivity;
        [SerializeField] private bool inverseY;

        [Header("Parameters"), Space(5)]
        [SerializeField, Min(0)] private float distance;
        [SerializeField, Tooltip("Relative at the pivot point of the camera target.")] private Vector3 anchorOffset;
        [SerializeField] private float maxPitch;

        [SerializeField, Tooltip("How responsive the camera is. Each value represents the time for the camera to catch up with its target position.")]
        private Vector3 damping;

        [Header("Debug"), Space(10)]
        [SerializeField] private Color targetGuizmoColor;
        [SerializeField] private Color anchorGuizmoColor;

        private Vector3 anchorPosition;
        private Vector3 cameraAnchorVelocity;

        private Quaternion AnchorRotation => Quaternion.Euler(targetPitch, targetYaw, 0);
        private float targetYaw;
        private float targetPitch;

        private void LateUpdate()
        {
            if (target == null)
            {
                ErrorMessage.MissingReferenceWarning(nameof(target), nameof(CameraController), gameObject);
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
            transform.LookAt(anchorPosition, Vector3.up);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (target == null)
            {
                ErrorMessage.MissingReferenceWarning(nameof(target), nameof(CameraController), gameObject);
                return;
            }
            Gizmos.color = targetGuizmoColor;
            Gizmos.DrawSphere(target.position, .1f);

            Gizmos.color = anchorGuizmoColor;
            Gizmos.DrawSphere(anchorPosition, .1f);

            Gizmos.color = anchorGuizmoColor;
            Gizmos.DrawLine(target.position, anchorPosition);

            Gizmos.color = anchorGuizmoColor;
            Gizmos.DrawLine(anchorPosition, transform.position);
        }

#endif

        private Vector3 CalculateTargetPosition()
        {
            UpdateAnchorPosition();
            Vector3 targetPosition = anchorPosition + AnchorRotation * Vector3.back * distance;
            return targetPosition;
        }

        private void UpdateAnchorPosition()
        {
            anchorPosition = Utilities.BetterVectorSmoothDamp(
                a: anchorPosition,
                b: target.position + anchorOffset,
                currentVelocity: ref cameraAnchorVelocity,
                smoothTime: damping,
                maxSpeed: Vector3.one * float.MaxValue);
        }
    }
}
