using Still.Camera.Model;
using Still.GOAP.WorldState;
using Still.Object.Door.Model;
using Still.Object.Door.View;
using Still.Object.Key;
using Still.Object.Key.Model;
using Still.Player.Model;
using Still.Player.Presenter;
using Still.Player.View;
using UnityEngine;
using UnityEngine.Rendering;
using VContainer;

namespace Still.Player
{
    public class GameInstaller : MonoBehaviour
    {
        public WorldStates WorldStates => _worldStates;
        public SANValueModel SANValueModel => _sanValueModel;

        [SerializeField] private PlayerConfig _config;

        private PlayerView _playerView;
        private CameraView _cameraView;
        private StaminaView _staminaView;
        private SANView _sanView;
        private PlayerSensorView _playerSensorView;
        private Volume _volume;
        private LockedDoor _lockedDoor;
        private KeyView _keyView;
        private Light _spotLight;
        [Inject] private WorldStates _worldStates;

        private PlayerMovePresenter _movePresenter;
        private PlayerStatusPresenter _playerStatusPresenter;
        private PlayerFearLevelPresenter _playerFearLevelPresenter;
        private PlayerSensorPresenter _playerSensorPresenter;
        private DepthOfFieldPresenter _dofPresenter;
        private LockedDoorPresenter _lockedDoorPresenter;
        private KeyPresenter _keyPresenter;
        private PlayerFovPresenter _playerFovPresenter;
        private PlayerGhostDetectedPresenter _playerGhostDetectedPresenter;
        private PlayerSpotLightPresenter _playerSpotLightPresenter;

        private PlayerModel _playerModel;
        private StaminaModel _staminaModel;
        private SANValueModel _sanValueModel;
        private LockedDoorModel _lockedDoorModel;
        private KeyModel _keyModel;
        private FieldOfViewModel _fovModel;
        private SpotLightModel _spotLightModel;

        private void Start()
        {
            ViewInit();
            // モデルの初期化
            _playerModel = new PlayerModel();
            _staminaModel = new StaminaModel(_config.DefaultStaminaValue);
            _sanValueModel = new SANValueModel(_config.DefaultSANValue);
            _keyModel = new KeyModel();
            _lockedDoorModel = new LockedDoorModel();
            _fovModel = new FieldOfViewModel(_config.DefaultFOV);
            _spotLightModel = new SpotLightModel();

            // プレゼンターの初期化
            _movePresenter = new PlayerMovePresenter(_playerView, _playerModel, _config, _cameraView, _staminaModel);
            _playerStatusPresenter = new PlayerStatusPresenter(_config, _playerView, _staminaView, _sanView, _staminaModel, _sanValueModel);
            _playerFearLevelPresenter = new PlayerFearLevelPresenter(_sanValueModel, _worldStates);
            _playerSensorPresenter = new PlayerSensorPresenter(_playerSensorView, _playerView);
            _dofPresenter = new DepthOfFieldPresenter(_playerSensorView, _volume);
            _lockedDoorPresenter = new LockedDoorPresenter(_lockedDoorModel, _lockedDoor, _keyView);
            _keyPresenter = new KeyPresenter(_keyView, _keyModel);
            _playerFovPresenter = new PlayerFovPresenter(_playerView, _cameraView, _fovModel);
            _playerGhostDetectedPresenter = new PlayerGhostDetectedPresenter(_playerSensorView, _worldStates);
            _playerSpotLightPresenter = new PlayerSpotLightPresenter(_playerView, _spotLight, _spotLightModel);

            _playerView.EnablePlayerInput();
        }
        private void ViewInit()
        {
            _playerView = FindAnyObjectByType<PlayerView>();
            _cameraView = FindAnyObjectByType<CameraView>();
            _staminaView = FindAnyObjectByType<StaminaView>();
            _sanView = FindAnyObjectByType<SANView>();
            _playerSensorView = FindAnyObjectByType<PlayerSensorView>();
            _volume = FindAnyObjectByType<Volume>();
            _keyView = FindAnyObjectByType<KeyView>();
            _lockedDoor = FindAnyObjectByType<LockedDoor>();
            _spotLight = _playerSensorView.GetComponentInChildren<Light>();

            Cursor.lockState = CursorLockMode.Locked;
        }
        private void Update()
        {
            _movePresenter.Update();
            _playerStatusPresenter.StatusUpdate();
            _playerFovPresenter.UpdateFOV();
        }
        private void OnDestroy()
        {
            _playerView.Dispose();
            _playerFearLevelPresenter.Dispose();
            _playerSensorPresenter.Dispose();
            _dofPresenter.Dispose();
            _lockedDoorPresenter.Dispose();
            _keyPresenter.Dispose();
            _playerFovPresenter.Dispose();
            _playerGhostDetectedPresenter.Dispose();
            _playerSpotLightPresenter.Dispose();
        }
    }
}