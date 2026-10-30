using Still.Common;
using UnityEngine;
using VContainer.Unity;

namespace Still.Goap
{
    ///<summary>
    ///プレイヤーが部屋に居続けたらノックできる状態にする
    ///</summary>
    public sealed class KnockTimer : ITickable
    {
        public KnockTimer(IWorldStates worldStates, GhostConfig config)
        {
            _worldStates = worldStates;
            _config = config;
        }

        private float _timeInTheRoom;

        private readonly IWorldStates _worldStates;
        private readonly GhostConfig _config;

        ///<summary>
        ///滞在時間を数える
        ///</summary>
        public void Tick()
        {
            // 部屋を出たら数え直す（以前は出入りしても加算が続いていた）
            if (_worldStates.GetStateValue(WorldStateType.InTheRoom) != 1)
            {
                _timeInTheRoom = 0f;
                return;
            }

            _timeInTheRoom += Time.deltaTime;
            if (_timeInTheRoom < _config.KnockAfterSeconds) return;

            _timeInTheRoom = 0f;
            _worldStates.ModifyState(WorldStateType.Knock, 1);
            _worldStates.ModifyState(WorldStateType.InTheRoom, 0);
        }
    }
}
