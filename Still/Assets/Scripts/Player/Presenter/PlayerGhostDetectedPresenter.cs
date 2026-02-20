using Still.Enum.WorldStates;
using Still.GOAP.Agent;
using Still.GOAP.WorldState;
using UnityEngine;
namespace Still.Player.Presenter
{
    public class PlayerGhostDetectedPresenter
    {
        private readonly PlayerSensorView _playerSensorView;
        private readonly WorldStates _worldStates;
        private int _ghostDetectedValue;

        public PlayerGhostDetectedPresenter(PlayerSensorView playerSensorView, WorldStates worldStates)
        {
            _playerSensorView = playerSensorView;
            _worldStates = worldStates;
            Initialize();
        }

        private void Initialize()
        {
            _playerSensorView.OnHitDetected += HandleGhostDetected;
        }

        private void HandleGhostDetected(RaycastHit hit)
        {
            if (hit.collider.TryGetComponent<GAgent>(out var agent))
            {
                if (_ghostDetectedValue == 1) return;

                _ghostDetectedValue = 1;
                _worldStates.ModifyState(WorldStateType.GhostDetected.ToString(), _ghostDetectedValue);
                Debug.Log("ゴーストを検知した");
            }
        }
        public void Dispose()
        {
            _playerSensorView.OnHitDetected -= HandleGhostDetected;
        }
    }
}