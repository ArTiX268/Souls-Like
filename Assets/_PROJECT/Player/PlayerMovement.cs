using UnityEngine;

namespace ArTiX.SoulsLike.Player
{
    [RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float maxSpeed;
        [SerializeField, Tooltip("The time to reach max speed.")] private float acceleration;
        [SerializeField] private float desceleration;

        [SerializeField] private float angularSpeed;

        private PlayerInput input;
        private Rigidbody body;

        private Vector3 horizontalVelocity;
        private Vector3 horizontalAcceleration;

        private bool RotateToCameraDirection => input.MoveInput.sqrMagnitude > 0.001f;

        private void Awake()
        {
            input = GetComponent<PlayerInput>();
            body = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            Transform camera = UnityEngine.Camera.main.transform;
            Move(camera);

            if (RotateToCameraDirection)
                Rotate(camera);
        }

        private void Move(Transform cameraTransform)
        {
            Vector2 moveInput = input.MoveInput;
            Vector3 directionVector = cameraTransform.forward * moveInput.y + cameraTransform.right * moveInput.x;
            directionVector.y = 0;
            directionVector.Normalize();

            float smoothing = input.MoveInput.sqrMagnitude > 0.001f ? acceleration : desceleration;
            horizontalVelocity = Vector3.SmoothDamp(horizontalVelocity, directionVector * maxSpeed, ref horizontalAcceleration, smoothing);

            body.MovePosition(horizontalVelocity * Time.deltaTime + body.position);
        }

        private void Rotate(Transform cameraTransform)
        {
            Vector3 cameraForward = cameraTransform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();
            body.MoveRotation(Quaternion.Slerp(
                a: body.rotation,
                b: Quaternion.LookRotation(cameraForward, Vector3.up),
                t: Time.deltaTime * angularSpeed)
            );
        }
    }
}
