using Still.Player.Model;
using Still.Player.View;
using UnityEngine;

namespace Still.Player.Presenter
{
    public class PlayerMovePresenter
    {
        private readonly IPlayerController _playerView;
        private readonly CameraView _cameraView;
        private readonly PlayerModel _model;
        private readonly PlayerConfig _config;
        private readonly StaminaModel _staminaModel;

        public PlayerMovePresenter(IPlayerController playerView,
            PlayerModel model,
            PlayerConfig config,
            CameraView cameraView,
            StaminaModel staminaModel)
        {
            _playerView = playerView;
            _cameraView = cameraView;
            _model = model;
            _config = config;
            _staminaModel = staminaModel;
        }

        public void Update()
        {
            var cameraForward = new Vector3(_cameraView.transform.forward.x, 0, _cameraView.transform.forward.z).normalized;
            Vector3 cameraRight = new Vector3(_cameraView.transform.right.x, 0, _cameraView.transform.right.z).normalized;

            var input = _playerView.CurrentMoveValue;
            var dir = cameraForward * input.y + cameraRight * input.x;

            if (CanDash() && _playerView.IsDash)
            {
                _model.SetMoveSpeed( _config.PlayerMaxSpeed);
            }
            else
            {
                _model.SetMoveSpeed( _config.PlayerDefaultSpeed);
            }

            _playerView.Move(dir, _model.CurrentSpeed);
        }
        /// <summary>
        /// ダッシュできるか？
        /// </summary>
        /// <returns></returns>
        private bool CanDash()
        {
            return _staminaModel.CurrentStamina > 0;
        }
    }
}