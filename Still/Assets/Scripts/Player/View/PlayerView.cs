using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Still.Player.View
{
    public class PlayerView : MonoBehaviour, IPlayerController, IDisposable
    {
        public Vector2 CurrentMoveValue => _currentMoveValue;
        public bool IsDash => _isDash;
        public bool IsInteract => _isInteract;

        private PlayerInputActions _actions;
        private Rigidbody _rigidbody;
        private Vector2 _currentMoveValue;
        private bool _isDash;
        private bool _isInteract;

        private void Awake()
        {
            _actions = new PlayerInputActions();
            _rigidbody = GetComponent<Rigidbody>();
        }
        public void EnablePlayerInput()
        {
            _actions.Player.Move.performed += OnInputMove;
            _actions.Player.Move.canceled += OnInputMove;
            _actions.Player.Dash.performed += OnDashPerformed;
            _actions.Player.Dash.canceled += OnDashPerformed;
            _actions.Player.Interact.performed += OnInteractPerformed;
            _actions.Player.Interact.canceled += OnInteractPerformed;
            _actions.Player.Enable();
        }

        public void DisablePlayerInput()
        {
            _actions.Player.Move.performed -= OnInputMove;
            _actions.Player.Move.canceled -= OnInputMove;
            _actions.Player.Dash.performed -= OnDashPerformed;
            _actions.Player.Dash.canceled -= OnDashPerformed;
            _actions.Player.Interact.performed -= OnInteractPerformed;
            _actions.Player.Interact.canceled -= OnInteractPerformed;
            _actions.Player.Disable();
        }

        private void OnInputMove(InputAction.CallbackContext context)
        {
            if (context.performed)
                _currentMoveValue = context.ReadValue<Vector2>();
            else if (context.canceled)
                _currentMoveValue = Vector2.zero;
        }

        private void OnDashPerformed(InputAction.CallbackContext context)
        {
            if (context.performed)
                _isDash = true;
            else if (context.canceled)
                _isDash = false;
        }
        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            if (context.performed)
                _isInteract = true;
            else if (context.canceled)
                _isInteract = false;
        }
        public void Move(Vector3 direction, float speed)
        {
            _rigidbody.linearVelocity = direction * speed;
        }

        public void Dispose()
        {
            DisablePlayerInput();
            _actions.Dispose();
        }
    }
}