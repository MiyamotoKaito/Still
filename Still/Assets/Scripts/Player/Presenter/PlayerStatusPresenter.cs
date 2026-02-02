using Still.Player.Model;
using Still.Player.View;
using UnityEngine;
namespace Still.Player.Presenter
{
    public class PlayerStatusPresenter
    {
        private readonly PlayerView _playerView;
        private readonly StaminaView _staminaView;
        private readonly StaminaModel _staminaModel;
        private readonly SANValueModel _sanValueModel;

        public PlayerStatusPresenter(PlayerView playerView, StaminaView staminaView, StaminaModel staminaModel, SANValueModel sanValueModel)
        {
            _playerView = playerView;
            _staminaView = staminaView;
            _staminaModel = staminaModel;
            _sanValueModel = sanValueModel;
        }

        public void StatusUpdate()
        {
            if (_playerView.IsDash)
            {
                _staminaModel.ModifyStamina(-10 * Time.deltaTime);
                _staminaView.UpdateStamina(_staminaModel.CurrentStamina);
            }
            else if(_staminaModel.CurrentStamina < 100 && !_playerView.IsDash)
            {
                _staminaModel.ModifyStamina(5 * Time.deltaTime);
                _staminaView.UpdateStamina(_staminaModel.CurrentStamina);
            }
        }
    }
}