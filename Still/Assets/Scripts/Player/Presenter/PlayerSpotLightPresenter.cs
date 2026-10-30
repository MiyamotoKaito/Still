using System;
using R3;
using Still.Common;
using VContainer.Unity;

namespace Still.Player
{
    ///<summary>
    ///懐中電灯の入力と点灯状態を中継する
    ///</summary>
    public sealed class PlayerSpotLightPresenter : IInitializable, IDisposable
    {
        public PlayerSpotLightPresenter(PlayerView playerView, PlayerSensorView sensorView, SpotLightEntity spotLight, IAudioService audio)
        {
            _playerView = playerView;
            _sensorView = sensorView;
            _spotLight = spotLight;
            _audio = audio;
        }

        private IDisposable _subscription;

        private readonly PlayerView _playerView;
        private readonly PlayerSensorView _sensorView;
        private readonly SpotLightEntity _spotLight;
        private readonly IAudioService _audio;

        ///<summary>
        ///購読を始める
        ///</summary>
        public void Initialize()
        {
            _playerView.OnLightToggleEvent += HandleToggle;
            _subscription = _spotLight.IsOn.Subscribe(isOn => _sensorView.SpotLight.enabled = isOn);
        }

        ///<summary>
        ///購読をやめる
        ///</summary>
        public void Dispose()
        {
            _playerView.OnLightToggleEvent -= HandleToggle;
            _subscription?.Dispose();
        }

        private void HandleToggle()
        {
            _spotLight.Toggle();
            _audio.PlaySe("Light");
        }
    }
}
