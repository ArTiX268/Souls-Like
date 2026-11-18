using UnityEngine;

namespace ArTiX.SoulsLike.Player
{
    public class PlayerVisual : MonoBehaviour
    {
        [SerializeField] private PlayerInput input;
        [SerializeField] private PlayerMovement movement;

        [SerializeField] private float rotationSpeed = 1;

        private const float MIN_MOVE_INPUT_LENGTH = 0.001f;

        private void Update()
        {
            if (input.MoveInput.sqrMagnitude > MIN_MOVE_INPUT_LENGTH)
            {
                RotateToMovement();
            }
        }

        private void RotateToMovement()
        {
            Vector3 cameraForward = UnityEngine.Camera.main.transform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
            transform.rotation =
                Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
