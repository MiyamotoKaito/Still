using Still.Player.Model;
using Still.Player.View;
using UnityEngine;
namespace Still.Player.Presenter
{
    public class PlayerStatusPresenter
    {
        private readonly PlayerView _playerView;
        private readonly StaminaView _staminaView;
        private readonly SANView _sanView;
        private readonly StaminaModel _staminaModel;
        private readonly SANValueModel _sanValueModel;

        public PlayerStatusPresenter(PlayerConfig config, PlayerView playerView, StaminaView staminaView, SANView sanView, StaminaModel staminaModel, SANValueModel sanValueModel)
        {
            _playerView = playerView;
            _staminaView = staminaView;
            _sanView = sanView;
            _staminaModel = staminaModel;
            _sanValueModel = sanValueModel;

            _staminaModel.SetMaxStamina(config.DefaultStaminaValue);
            _staminaView.UpdateStamina(_staminaModel.CurrentStamina);
            _sanValueModel.SetMaxSAN(config.DefaultSANValue);
            _sanView.UpdateSAN(_sanValueModel.CurrentSAN);
        }

        public void StatusUpdate()
        {
            if (_playerView.IsDash)
            {
                _staminaModel.ModifyStamina(-10 * Time.deltaTime);
                _staminaView.UpdateStamina(_staminaModel.CurrentStamina);
                _sanValueModel.ModifySAN(-2 * Time.deltaTime);
                _sanView.UpdateSAN(_sanValueModel.CurrentSAN);
            }
            else if (_staminaModel.CurrentStamina < _staminaModel.MaxStamina && !_playerView.IsDash)
            {
                _staminaModel.ModifyStamina(5 * Time.deltaTime);
                _staminaView.UpdateStamina(_staminaModel.CurrentStamina);
            }
        }
    }
}