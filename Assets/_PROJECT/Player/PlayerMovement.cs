using UnityEngine;

namespace ArTiX.SoulsLike.Player
{
    [RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
    public class PlayerMovement : MonoBehaviour
    {
        // REFERENCES
        private PlayerInput input;
        private Rigidbody body;

        // PARAMETERS
        [SerializeField] private float maxSpeed;
        [SerializeField, Tooltip("The time to reach max speed.")] private float acceleration;
        [SerializeField] private float desceleration;

        private const float MIN_MOVE_INPUT_LENGTH = 0.001f;

        private Vector3 horizontalVelocity;
        private Vector3 horizontalAcceleration;

        [SerializeField] private LayerMask groundLayer;
        private const float GROUND_DETECTION = 0.05f;

        private bool wantsToJump;
        private bool isGrounded;

        private void Awake()
        {
            input = GetComponent<PlayerInput>();
            body = GetComponent<Rigidbody>();

            input.OnJump += OnJump;
        }

        private void Update()
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, GROUND_DETECTION, groundLayer);
        }

        private void FixedUpdate()
        {
            Move();

            if (wantsToJump && isGrounded)
            {
                Jump();
                wantsToJump = false;
            }
        }

        private void Move()
        {
            Vector2 moveInput = input.MoveInput;
            Transform cameraTransform = UnityEngine.Camera.main.transform;
            Vector3 directionVector = cameraTransform.forward * moveInput.y + cameraTransform.right * moveInput.x;
            directionVector.y = 0;
            directionVector.Normalize();

            float smoothing = input.MoveInput.sqrMagnitude > MIN_MOVE_INPUT_LENGTH ? acceleration : desceleration;
            horizontalVelocity = Vector3.SmoothDamp(horizontalVelocity, directionVector * maxSpeed, ref horizontalAcceleration, smoothing);

            body.MovePosition(horizontalVelocity * Time.deltaTime + body.position);
        }

        private void OnJump() => wantsToJump = true;

        private void Jump()
        {

        }
    }
}
