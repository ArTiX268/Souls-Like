using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ArTiX.SoulsLike.Player
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private InputActionReference moveInput;
        [SerializeField] private InputActionReference jumpInput;

        public Vector2 MoveInput
        {
            get
            {
                return moveInput.action.ReadValue<Vector2>();
            }
        }
        public event Action OnJump;

        private void Awake()
        {
            jumpInput.action.performed += OnJumpInput;
        }

        private void OnJumpInput(InputAction.CallbackContext ctxt)
        {
            OnJump?.Invoke();
        }
    }
}
