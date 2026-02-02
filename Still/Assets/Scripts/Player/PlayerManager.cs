using Still.Player.Model;
using Still.Player.Presenter;
using Still.Player.View;
using UnityEngine;

namespace Still.Player
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private PlayerView _playerView;
        [SerializeField] private CameraView _cameraView;
        [SerializeField] private PlayerConfig _config;

        private PlayerPresenter _presenter;
        private PlayerModel _model;

        private void Awake()
        {
            _model = new PlayerModel();
            _presenter = new PlayerPresenter(_playerView, _model, _config, _cameraView);
            _playerView.EnablePlayerInput();
        }

        private void Update()
        {
            _presenter.Update();
        }
        private void OnDestroy()
        {
            _playerView.Dispose();
        }
    }
}