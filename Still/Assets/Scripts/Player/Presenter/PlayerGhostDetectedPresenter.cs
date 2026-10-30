using System;
using Still.Common;
using UnityEngine;
using VContainer.Unity;

namespace Still.Player
{
    ///<summary>
    ///プレイヤーがゴーストを見たことをワールドステートへ伝える
    ///</summary>
    public sealed class PlayerGhostDetectedPresenter : IInitializable, IDisposable
    {
        public PlayerGhostDetectedPresenter(PlayerSensorView sensorView, IWorldStates worldStates)
        {
            _sensorView = sensorView;
            _worldStates = worldStates;
        }

        private readonly PlayerSensorView _sensorView;
        private readonly IWorldStates _worldStates;

        ///<summary>
        ///購読を始める
        ///</summary>
        public void Initialize() => _sensorView.OnHitDetected += HandleHit;

        ///<summary>
        ///購読をやめる
        ///</summary>
        public void Dispose() => _sensorView.OnHitDetected -= HandleHit;

        private void HandleHit(RaycastHit hit)
        {
            if (hit.collider.GetComponentInParent<IGhost>() == null) return;
            // 以前は一度検知すると自前のフラグが立ちっぱなしで、FocusMe達成後に二度と検知できなかった
            if (_worldStates.GetStateValue(WorldStateType.GhostDetected) == 1) return;

            _worldStates.ModifyState(WorldStateType.GhostDetected, 1);
        }
    }
}
