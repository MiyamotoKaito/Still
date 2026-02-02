using Still.Player.Model;
using Still.Player.View;
using UnityEngine;

namespace Still.Player.Presenter
{
    public class PlayerPresenter
    {
        private readonly IPlayerController _playerView;
        private readonly CameraView _cameraView;
        private readonly PlayerModel _model;
        private readonly PlayerConfig _config;

        public PlayerPresenter(IPlayerController playerView, PlayerModel model, PlayerConfig config, CameraView cameraView)
        {
            _playerView = playerView;
            _cameraView = cameraView;
            _model = model;
            _config = config;
        }

        public void Update()
        {
            var cameraForward = new Vector3(_cameraView.transform.forward.x, 0, _cameraView.transform.forward.z).normalized;
            Vector3 cameraRight = new Vector3(_cameraView.transform.right.x, 0, _cameraView.transform.right.z).normalized;

            var input = _playerView.CurrentMoveValue;
            var dir = cameraForward * input.y + cameraRight * input.x;

            _model.SetMoveSpeed(_playerView.IsDash ? _config.PlayerMaxSpeed : _config.PlayerDefaultSpeed);

            _playerView.Move(dir, _model.CurrentSpeed);
        }
    }
}