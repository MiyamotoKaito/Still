using System;
using Still.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Still.Player
{
    ///<summary>
    ///プレイヤーの体と入力の受け口
    ///</summary>
    public class PlayerView : MonoBehaviour, IPlayerController, IPlayerAvatar
    {
        public event Action OnInteractEvent;
        public event Action OnDashEvent;
        public event Action OnDashCancelEvent;
        public event Action OnLightToggleEvent;

        ///<summary>移動入力</summary>
        public Vector2 CurrentMoveValue => _currentMoveValue;
        ///<summary>ダッシュボタンを押しているか</summary>
        public bool IsDash => _isDash;
        ///<summary>プレイヤーのTransform</summary>
        public Transform Transform => transform;

        private PlayerInputActions _actions;
        private Rigidbody _rigidbody;
        private Vector2 _currentMoveValue;
        private bool _isDash;

        ///<summary>
        ///指定方向へ移動させる
        ///</summary>
        public void Move(Vector3 direction, float speed)
        {
            _rigidbody.linearVelocity = direction * speed;
        }

        ///<summary>
        ///指定位置へ瞬間移動する
        ///</summary>
        public void Warp(Vector3 position)
        {
            _rigidbody.position = position;
            transform.position = position;
        }

        ///<summary>
        ///入力を止め、押しっぱなしの状態も解除する
        ///</summary>
        public void LockInput()
        {
            _actions.Player.Disable();
            _currentMoveValue = Vector2.zero;
            if (!_isDash) return;

            _isDash = false;
            OnDashCancelEvent?.Invoke();
        }

        ///<summary>
        ///入力を再開する
        ///</summary>
        public void UnlockInput()
        {
            _actions.Player.Enable();
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _actions = new PlayerInputActions();
            // 購読は一度だけにし、ロック／解除はアクションマップの有効化で切り替える（二重購読を防ぐ）
            _actions.Player.Move.performed += OnInputMove;
            _actions.Player.Move.canceled += OnInputMove;
            _actions.Player.Dash.performed += OnInputDash;
            _actions.Player.Dash.canceled += OnInputDash;
            _actions.Player.Interact.started += OnInputInteract;
            _actions.Player.Toggle.started += OnInputLightToggle;
        }

        private void OnDestroy()
        {
            _actions.Player.Move.performed -= OnInputMove;
            _actions.Player.Move.canceled -= OnInputMove;
            _actions.Player.Dash.performed -= OnInputDash;
            _actions.Player.Dash.canceled -= OnInputDash;
            _actions.Player.Interact.started -= OnInputInteract;
            _actions.Player.Toggle.started -= OnInputLightToggle;
            _actions.Dispose();
        }

        private void OnInputMove(InputAction.CallbackContext context)
        {
            _currentMoveValue = context.canceled ? Vector2.zero : context.ReadValue<Vector2>();
        }

        private void OnInputDash(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _isDash = true;
                OnDashEvent?.Invoke();
            }
            else if (context.canceled)
            {
                _isDash = false;
                OnDashCancelEvent?.Invoke();
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
    }
}
