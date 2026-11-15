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

        [SerializeField, Tooltip("How responsive the camera is. Each value represents the time for the camera to catch up with its target position.")] private Vector3 damping;

        [Header("Debug"), Space(10)]
        [SerializeField] private Color targetGuizmoColor;
        [SerializeField] private Color anchorGuizmoColor;

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
                targetPitch = Mathf.Clamp(targetPitch, -45, 45);
            }

            transform.position = CalculateTargetPosition();
            transform.LookAt(GetAnchorPosition(), Vector3.up);
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
            Gizmos.DrawSphere(GetAnchorPosition(), .1f);

            Gizmos.color = anchorGuizmoColor;
            Gizmos.DrawLine(target.position, GetAnchorPosition());

            Gizmos.color = anchorGuizmoColor;
            Gizmos.DrawLine(GetAnchorPosition(), transform.position);
        }

#endif

        private Vector3 CalculateTargetPosition()
        {
            Vector3 targetPosition = GetAnchorPosition();
            targetPosition += AnchorRotation * Vector3.back * distance;
            return targetPosition;
        }

        private Vector3 GetAnchorPosition() => target.position + anchorOffset;
    }
}
