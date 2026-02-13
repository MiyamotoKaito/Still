using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Still.Player.View
{
    public class PlayerView : MonoBehaviour, IPlayerController, IDisposable
    {
        public Vector2 CurrentMoveValue => _currentMoveValue;
        public bool IsDash => _isDash;
        public event Action OnInteractEvent;
        public event Action OnDashEvent;
        public event Action OnDashCancelEvent;
        public event Action OnLightToggleEvent;

        private PlayerInputActions _actions;
        private Rigidbody _rigidbody;
        private Vector2 _currentMoveValue;
        private bool _isDash;

        private void Awake()
        {
            _actions = new PlayerInputActions();
            _rigidbody = GetComponent<Rigidbody>();
        }
        public void EnablePlayerInput()
        {
            _actions.Player.Move.performed += OnInputMove;
            _actions.Player.Move.canceled += OnInputMove;
            _actions.Player.Dash.performed += OnInputDash;
            _actions.Player.Dash.canceled += OnInputDash;
            _actions.Player.Interact.started += OnInputInteract;
            _actions.Player.Toggle.started += OnInputLightToggle;
            _actions.Player.Enable();
        }
        public void DisablePlayerInput()
        {
            _actions.Player.Move.performed -= OnInputMove;
            _actions.Player.Move.canceled -= OnInputMove;
            _actions.Player.Dash.performed -= OnInputDash;
            _actions.Player.Dash.canceled -= OnInputDash;
            _actions.Player.Interact.started -= OnInputInteract;
            _actions.Player.Toggle.started -= OnInputLightToggle;
            _actions.Player.Disable();
        }
        private void OnInputMove(InputAction.CallbackContext context)
        {
            if (context.performed)
                _currentMoveValue = context.ReadValue<Vector2>();
            else if (context.canceled)
                _currentMoveValue = Vector2.zero;
        }
        private void OnInputDash(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnDashEvent?.Invoke();
                _isDash = true;
            }
            else if (context.canceled)
            {
                OnDashCancelEvent?.Invoke();
                _isDash = false;
            }
        }
        private void OnInputInteract(InputAction.CallbackContext context)
        {
            OnInteractEvent?.Invoke();
        }
        private void OnInputLightToggle(InputAction.CallbackContext context)
        {
            OnLightToggleEvent?.Invoke();
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