using Still.Player.Model;
using Still.Player.View;
using UnityEngine;
namespace Still.Player.Presenter
{
    public class PlayerSpotLightPresenter
    {
        private PlayerView _view;
        private Light _spotLight;
        private SpotLightModel _model;
        public PlayerSpotLightPresenter(PlayerView view, Light light, SpotLightModel model)
        {
            _view = view;
            _spotLight = light;
            _model = model;

            _view.OnLightToggleEvent += HandleSpotLight;
        }

        private void HandleSpotLight()
        {
            _model.SwitchToggle();
            AudioManager.Instance.PlaySE("Light");
            _spotLight.enabled = _model.IsOn;
        }
        public void Dispose()
        {
            _view.OnLightToggleEvent -= HandleSpotLight;
        }
    }
}