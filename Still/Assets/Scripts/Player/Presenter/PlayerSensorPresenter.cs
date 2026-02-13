using Still.Player.View;
using UnityEngine;
namespace Still.Player.Presenter
{
    public class PlayerSensorPresenter
    {
        private PlayerSensorView _playerSensorView;
        private PlayerView _playerView;
        private IInteractable _currentInteractable;
        public PlayerSensorPresenter(PlayerSensorView playerSensorView, PlayerView playerView)
        {
            _playerSensorView = playerSensorView;
            _playerView = playerView;
            _playerView.OnInteractEvent += HandleInteractableInteract;
            _playerSensorView.OnHitPlayerDetected += HandleHitDetected;
            _playerSensorView.OnHitLost += HandleHitLost;
        }
        private void HandleInteractableInteract()
        {
            if (_currentInteractable != null)
            {
                _currentInteractable.Interact();
            }
        }
        private void HandleHitDetected(RaycastHit hit)
        {
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                _currentInteractable = interactable;
                interactable.ShowUI();
            }
        }
        private void HandleHitLost()
        {
            if (_currentInteractable != null)
            {
                _currentInteractable.HideUI();
                _currentInteractable = null;
            }
        }
        public void Dispose()
        {
            _playerSensorView.OnHitPlayerDetected -= HandleHitDetected;
            _playerSensorView.OnHitLost -= HandleHitLost;
        }
    }
}
