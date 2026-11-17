using UnityEngine;

namespace ArTiX.SoulsLike.Player
{
    [RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float maxSpeed;
        [SerializeField] private float acceleration;
        [SerializeField] private float gravity;
        [SerializeField, Min(1)] private float desceleration;

        private PlayerInput input;
        private Rigidbody body;

        private Vector3 velocity;
        private Vector3 accelerationVector;

        private void Awake()
        {
            input = GetComponent<PlayerInput>();
            body = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            UpdateVelocity();

            body.MovePosition(velocity * Time.deltaTime + body.position);
        }

        private void UpdateVelocity()
        {

        }
    }
}
