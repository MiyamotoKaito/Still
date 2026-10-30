using System;
using Still.Common;
using UnityEngine;
using VContainer.Unity;

namespace Still.Player
{
    ///<summary>
    ///視線の先の調べられるオブジェクトにガイドを出し、調べる入力を中継する
    ///</summary>
    public sealed class PlayerSensorPresenter : IInitializable, IDisposable
    {
        public PlayerSensorPresenter(PlayerSensorView sensorView, PlayerView playerView)
        {
            _sensorView = sensorView;
            _playerView = playerView;
        }

        private IInteractable _current;

        private readonly PlayerSensorView _sensorView;
        private readonly PlayerView _playerView;

        ///<summary>
        ///購読を始める
        ///</summary>
        public void Initialize()
        {
            _playerView.OnInteractEvent += HandleInteract;
            _sensorView.OnHitPlayerDetected += HandleHit;
            _sensorView.OnHitPlayerLost += ClearCurrent;
        }

        ///<summary>
        ///購読をやめる
        ///</summary>
        public void Dispose()
        {
            _playerView.OnInteractEvent -= HandleInteract;
            _sensorView.OnHitPlayerDetected -= HandleHit;
            _sensorView.OnHitPlayerLost -= ClearCurrent;
        }

        private void HandleInteract()
        {
            _current?.Interact();
        }

        private void HandleHit(RaycastHit hit)
        {
            if (!hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                ClearCurrent();
                return;
            }

            // 別のオブジェクトへ視線が移ったときに前のガイドが残らないようにする
            if (_current != interactable) ClearCurrent();
            _current = interactable;
            interactable.ShowUI();
        }

        private void ClearCurrent()
        {
            _current?.HideUI();
            _current = null;
        }
    }
}
