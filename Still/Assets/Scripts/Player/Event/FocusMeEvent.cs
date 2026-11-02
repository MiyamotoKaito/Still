using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Still.Common;
using UnityEngine;
using VContainer;

namespace Still.Player
{
    ///<summary>
    ///プレイヤーの視線をゴーストに釘付けにしてズームする
    ///</summary>
    [Serializable]
    public class FocusMeEvent : IEvent
    {
        ///<summary>演出が終わったか</summary>
        public bool IsFinished => _isFinished;

        [SerializeField] private float _focusDuration;
        [SerializeField] private float _focusFOV;

        private FieldOfViewEntity _fov;
        private IPlayerAvatar _player;
        private CameraView _cameraView;
        private bool _isFinished;
        private CancellationTokenSource _playCts;

        ///<summary>
        ///依存を受け取る
        ///</summary>
        [Inject]
        public void Construct(FieldOfViewEntity fov, IPlayerAvatar player, CameraView cameraView)
        {
            _fov = fov;
            _player = player;
            _cameraView = cameraView;
        }

        ///<summary>
        ///演出を開始する
        ///</summary>
        public void Play()
        {
            // 以前は2回目以降も完了フラグが立ったままで、演出が即終了扱いになっていた
            _isFinished = false;
            // 前の演出が残っていると両方が視野角を書き換え、ズームしたまま戻らなくなる
            _playCts?.Cancel();
            _playCts?.Dispose();
            _playCts = CancellationTokenSource.CreateLinkedTokenSource(_cameraView.destroyCancellationToken);
            PlayAsync(_playCts.Token).Forget();
        }

        private async UniTaskVoid PlayAsync(CancellationToken ct)
        {
            _cameraView.SetLookInputEnabled(false);
            _player.LockInput();

            await ZoomAsync(_focusFOV, ct);
            await UniTask.Delay(TimeSpan.FromSeconds(_focusDuration), cancellationToken: ct);
            // 開始時の値はダッシュ中や前の演出の途中の値のことがあるので、常に既定の視野角へ戻す
            await ZoomAsync(_fov.DefaultFov, ct);

            _player.UnlockInput();
            _cameraView.SetLookInputEnabled(true);
            _isFinished = true;
        }

        private async UniTask ZoomAsync(float target, CancellationToken ct)
        {
            float from = _fov.Fov.CurrentValue;
            float elapsed = 0f;
            while (elapsed < _focusDuration)
            {
                elapsed += Time.deltaTime;
                _fov.SetFov(Mathf.Lerp(from, target, elapsed / _focusDuration));
                await UniTask.Yield(ct);
            }
            _fov.SetFov(target);
        }
    }
}
