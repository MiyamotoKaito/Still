using Still.Common;
using UnityEngine;
using VContainer.Unity;

namespace Still.Goap
{
    ///<summary>
    ///ゴーストの視界をワールドステート（PlayerVisible）に反映する
    ///</summary>
    public sealed class GhostSightTicker : ITickable
    {
        public GhostSightTicker(GhostSensorView sensor, GhostConfig config, IWorldStates worldStates)
        {
            _sensor = sensor;
            _config = config;
            _worldStates = worldStates;
        }

        private float _lostTimer;

        private readonly GhostSensorView _sensor;
        private readonly GhostConfig _config;
        private readonly IWorldStates _worldStates;

        ///<summary>
        ///毎フレーム視界を確認する
        ///</summary>
        public void Tick()
        {
            bool isVisible = _worldStates.GetStateValue(WorldStateType.PlayerVisible) == 1;

            if (_sensor.CanSeePlayer(_config.GhostFovLength))
            {
                _lostTimer = 0f;
                if (!isVisible) _worldStates.ModifyState(WorldStateType.PlayerVisible, 1);
                return;
            }

            if (!isVisible) return;

            // 一瞬視線が切れただけで追跡をやめないよう、一定時間見えなかったら見失ったとする
            _lostTimer += Time.deltaTime;
            if (_lostTimer < _config.LoseSightSeconds) return;

            _lostTimer = 0f;
            _worldStates.ModifyState(WorldStateType.PlayerVisible, 0);
        }
    }
}
