using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace Still.Player
{
    ///<summary>
    ///視野角をカメラへ反映し、ダッシュ中は広げる
    ///</summary>
    public sealed class PlayerFovPresenter : IInitializable, IDisposable
    {
        private const float TransitionSeconds = 0.5f;

        public PlayerFovPresenter(PlayerView playerView, CameraView cameraView, FieldOfViewEntity fov, PlayerConfig config)
        {
            _playerView = playerView;
            _cameraView = cameraView;
            _fov = fov;
            _config = config;
        }

        private IDisposable _subscription;
        private CancellationTokenSource _transitionCts;

        private readonly PlayerView _playerView;
        private readonly CameraView _cameraView;
        private readonly FieldOfViewEntity _fov;
        private readonly PlayerConfig _config;

        ///<summary>
        ///購読を始める
        ///</summary>
        public void Initialize()
        {
            _subscription = _fov.Fov.Subscribe(_cameraView.SetFieldOfView);
            _playerView.OnDashEvent += HandleDash;
            _playerView.OnDashCancelEvent += HandleDashCancel;
        }

        ///<summary>
        ///購読をやめる
        ///</summary>
        public void Dispose()
        {
            // 以前はラムダで購読していたため解除できていなかった
            _playerView.OnDashEvent -= HandleDash;
            _playerView.OnDashCancelEvent -= HandleDashCancel;
            _subscription?.Dispose();
            CancelTransition();
        }

        private void HandleDash() => StartTransition(_config.DashFOV);

        private void HandleDashCancel() => StartTransition(_fov.DefaultFov);

        private void StartTransition(float target)
        {
            // 連打で複数の補間が同時に走って値が揺れないよう、前の補間を止める
            CancelTransition();
            _transitionCts = new CancellationTokenSource();
            TransitionAsync(target, _transitionCts.Token).Forget();
        }

        private void CancelTransition()
        {
            _transitionCts?.Cancel();
            _transitionCts?.Dispose();
            _transitionCts = null;
        }

        private async UniTaskVoid TransitionAsync(float target, CancellationToken ct)
        {
            float from = _fov.Fov.CurrentValue;
            float elapsed = 0f;
            while (elapsed < TransitionSeconds)
            {
                elapsed += Time.deltaTime;
                _fov.SetFov(Mathf.Lerp(from, target, elapsed / TransitionSeconds));
                await UniTask.Yield(ct);
            }
            _fov.SetFov(target);
        }
    }
}
