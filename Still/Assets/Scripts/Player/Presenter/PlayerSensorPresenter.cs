using Still.Player.View;
using UnityEngine;
namespace Still.Object.Door
{
    public class PlayerSensorPresenter
    {
        private PlayerSensorView _playerSensorView;
        private PlayerView _playerView;
        public PlayerSensorPresenter(PlayerSensorView playerSensorView, PlayerView playerView)
        {
            _playerSensorView = playerSensorView;
            _playerView = playerView;
            _playerSensorView.OnHitDetected += HandleHitDetected;
        }

        private void HandleHitDetected(RaycastHit hit)
        {
            if (hit.collider.TryGetComponent<DoorView>(out var doorView))
            {
                if (_playerView.IsInteract)
                doorView.IsOpen();
            }
        }

        public void Dispose()
        {
            _playerSensorView.OnHitDetected -= HandleHitDetected;
        }
    }
}
