using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ArTiX.SoulsLike.Player
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private InputActionReference moveInput;
        [SerializeField] private InputActionReference jumpInput;

        public Vector2 MoveInput { get; private set; }
        public event Action OnJump;

        private void Awake()
        {
            jumpInput.action.performed += OnJumpInput;
        }

        private void Update()
        {
            MoveInput = moveInput.action.ReadValue<Vector2>();
        }

        private void OnJumpInput(InputAction.CallbackContext ctxt)
        {
            OnJump?.Invoke();
        }
    }
}
