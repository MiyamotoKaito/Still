using Cysharp.Threading.Tasks;
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
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

namespace Still.Player
{
    public class GameInstaller : MonoBehaviour
    {
        public WorldStates WorldStates => _worldStates;
        public SANValueModel SANValueModel => _sanValueModel;
        public LastPosition LastPosition => _lastPosition;
        public Canvas UICanvas => _uiCanvas;
        public Image FadeImage => _fadeImage;
        public FieldOfViewModel FieldOfViewModel => _fovModel;
        [SerializeField] private PlayerConfig _config;
        [SerializeField] private Canvas _uiCanvas;
        [SerializeField] private Image _fadeImage;
        private PlayerView _playerView;
        private CameraView _cameraView;
        private StaminaView _staminaView;
        private SANView _sanView;
        private PlayerSensorView _playerSensorView;
        private Volume _volume;
        private LockedDoor _lockedDoor;
        private KeyView _keyView;
        private Light _spotLight;
        private EventManager _eventManager;
        private LastPosition _lastPosition;
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

        private bool _isGameOver;

        private void Start()
        {
            ViewInit();
            // モデルの初期化
            _playerModel = new PlayerModel();
            _staminaModel = new StaminaModel(_config.DefaultStaminaValue);
            _sanValueModel = new SANValueModel(_config.DefaultSANValue);
            _keyModel = new KeyModel(3);
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
            _keyPresenter = new KeyPresenter(_keyView, _keyModel, _eventManager);
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
            _keyView.Init();
            _lockedDoor = FindAnyObjectByType<LockedDoor>();
            _spotLight = _playerSensorView.GetComponentInChildren<Light>();
            _eventManager = FindAnyObjectByType<EventManager>();
            _lastPosition = new LastPosition();
            _lastPosition.SavePosition(_playerView.transform.position);

            Cursor.lockState = CursorLockMode.Locked;
        }
        private void Update()
        {
            _movePresenter.Update();
            _playerStatusPresenter.StatusUpdate();
            _playerFovPresenter.UpdateFOV();

            // ゲームオーバー判定
            if (_sanValueModel.CurrentSAN < 1 && !_isGameOver)
            {
                _isGameOver = true;
                GameOver().Forget();
            }
        }
        private async UniTask GameOver()
        {
            // プレイヤー入力を無効化
            _playerView.DisablePlayerInput();

            // フェードイン（暗転）
            _fadeImage.gameObject.SetActive(true);
            float elapsed = 0f;
            float duration = 2f;
            Color startColor = _fadeImage.color;
            startColor.a = 0f;
            _fadeImage.color = startColor;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                Color currentColor = _fadeImage.color;
                currentColor.a = t;
                _fadeImage.color = currentColor;
                await UniTask.Yield();
            }

            // 完全に不透明にする
            Color finalColor = _fadeImage.color;
            finalColor.a = 1f;
            _fadeImage.color = finalColor;

            // 少し待機
            await UniTask.Delay(500);

            // タイトルシーンへ
            SceneManager.LoadScene("Title");
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