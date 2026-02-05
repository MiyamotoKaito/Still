using Still.GOAP.WorldState;
using Still.Player.Model;
using Still.Player.Presenter;
using Still.Player.View;
using UnityEngine;
using VContainer;

namespace Still.Player
{
    public class PlayerManager : MonoBehaviour
    {
        public SANValueModel SANValueModel => _sanValueModel;

        [SerializeField] private PlayerConfig _config;

        private PlayerView _playerView;
        private CameraView _cameraView;
        private StaminaView _staminaView;
        private SANView _sanView;
        [Inject] private WorldStates _worldStates;

        private PlayerMovePresenter _movePresenter;
        private PlayerStatusPresenter _playerStatusPresenter;
        private PlayerFearLevelPresenter _playerFearLevelPresenter;

        private PlayerModel _playerModel;
        private StaminaModel _staminaModel;
        private SANValueModel _sanValueModel;

        private void Start()
        {
            ViewInit();
            // モデルの初期化
            _playerModel = new PlayerModel();
            _staminaModel = new StaminaModel(_config.DefaultStaminaValue);
            _sanValueModel = new SANValueModel(_config.DefaultSANValue);
            // プレゼンターの初期化
            _movePresenter = new PlayerMovePresenter(_playerView, _playerModel, _config, _cameraView, _staminaModel);
            _playerStatusPresenter = new PlayerStatusPresenter(_config, _playerView, _staminaView, _sanView, _staminaModel, _sanValueModel);
            _playerFearLevelPresenter = new PlayerFearLevelPresenter(_sanValueModel, _worldStates);

            _playerView.EnablePlayerInput();
        }
        private void ViewInit()
        {
            _playerView = FindAnyObjectByType<PlayerView>();
            _cameraView = FindAnyObjectByType<CameraView>();
            _staminaView = FindAnyObjectByType<StaminaView>();
            _sanView = FindAnyObjectByType<SANView>();
        }
        private void Update()
        {
            _movePresenter.Update();
            _playerStatusPresenter.StatusUpdate();
        }
        private void OnDestroy()
        {
            _playerView.Dispose();
        }
    }
}